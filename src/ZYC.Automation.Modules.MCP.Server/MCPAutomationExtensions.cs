using System.Reflection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Namotion.Reflection;
using ZYC.Automation.Abstractions.MCP;
using ZYC.Automation.Modules.MCP.Server.Abstractions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MCPAutoToolDiscoveryExtensions
{
    public static IMcpServerBuilder AddAutoDiscoveredTools(
        this IMcpServerBuilder builder,
        IEnumerable<Assembly>? assemblies = null)
    {
        assemblies ??= AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic);

        var tools = new List<McpServerTool>();
        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var asm in assemblies)
        {
            foreach (var itf in SafeGetTypes(asm)
                         .Where(t => t is { IsInterface: true, IsGenericTypeDefinition: false }))
            {
                var interfaceAttr = itf.GetCustomAttribute<ExposeToMCPAttribute>(true);
                if (interfaceAttr is null)
                {
                    continue;
                }

                var interfacePrefix = !string.IsNullOrWhiteSpace(interfaceAttr.Name)
                    ? interfaceAttr.Name!
                    : TrimInterfaceName(itf.Name);

                foreach (var method in itf.GetMethods())
                {
                    if (method.IsSpecialName)
                    {
                        continue;
                    }

                    if (method.GetCustomAttribute<MCPIgnoreAttribute>(true) is not null)
                    {
                        continue;
                    }

                    if (method.ContainsGenericParameters)
                    {
                        continue;
                    }

                    var methodAttr = method.GetCustomAttribute<ExposeToMCPAttribute>(true);

                    var baseName = !string.IsNullOrWhiteSpace(methodAttr?.Name)
                        ? methodAttr.Name!
                        : $"{interfacePrefix}_{method.Name}";

                    var toolName = MakeUnique(baseName, usedNames);
                    var requiresUIThread = methodAttr?.IsExplicitlySet == true
                        ? methodAttr.RequiresUIThread
                        : interfaceAttr.InvokeOnUIThread;

                    var description = method.GetXmlDocsSummary();

                    var options = new McpServerToolCreateOptions
                    {
                        Name = toolName,
                        Description = description
                    };


                    var interfaceType = itf;
                    // ReSharper disable once ConvertToLocalFunction
                    Func<RequestContext<CallToolRequestParams>, object> factory =
                        ctx => ctx.Services!.GetRequiredService(interfaceType);

                    var tool = McpServerTool.Create(method, factory, options);

                    if (requiresUIThread)
                    {
                        tool = new UIThreadDelegatingMCPTool(tool);
                    }

                    tools.Add(tool);
                }
            }
        }

        if (tools.Count > 0)
        {
            builder.WithTools(tools);
        }

        return builder;
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly asm)
    {
        try
        {
            return asm.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
        catch
        {
            return Array.Empty<Type>();
        }
    }

    private static string TrimInterfaceName(string name)
    {
        if (name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]))
        {
            return name.Substring(1);
        }

        return name;
    }


    private static string MakeUnique(string baseName, HashSet<string> used)
    {
        var name = baseName;
        var i = 2;
        while (!used.Add(name))
        {
            name = $"{baseName}__{i++}";
        }

        return name;
    }


    private static string ResolveToolName(
        Type iface,
        MethodInfo method,
        ExposeToMCPAttribute? ifaceAttr,
        ExposeToMCPAttribute? methodAttr)
    {
        if (!string.IsNullOrWhiteSpace(methodAttr?.Name))
        {
            return SanitizeToolName(methodAttr.Name!);
        }

        var prefix =
            !string.IsNullOrWhiteSpace(ifaceAttr?.Name)
                ? ifaceAttr.Name!
                : TrimLeadingI(iface.Name);

        return SanitizeToolName($"{prefix}_{method.Name}");
    }

    private static string TrimLeadingI(string interfaceName)
    {
        return interfaceName.Length >= 2 && interfaceName[0] == 'I' && char.IsUpper(interfaceName[1])
            ? interfaceName[1..]
            : interfaceName;
    }

    private static string SanitizeToolName(string name)
    {
        Span<char> buf = stackalloc char[name.Length];
        var j = 0;

        foreach (var ch in name)
        {
            var ok = char.IsLetterOrDigit(ch) || ch is '_' or '-' or '.';
            buf[j++] = ok ? ch : '_';
        }

        var s = new string(buf[..j]).Trim('_');
        return string.IsNullOrWhiteSpace(s) ? "tool" : s;
    }

    private static string MakeUniqueName(string baseName, HashSet<string> usedNames)
    {
        var name = baseName;
        var i = 2;
        while (!usedNames.Add(name))
        {
            name = $"{baseName}_{i++}";
        }

        return name;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
        catch
        {
            return Array.Empty<Type>();
        }
    }

    private sealed class UIThreadDelegatingMCPTool : DelegatingMcpServerTool
    {
        public UIThreadDelegatingMCPTool(McpServerTool innerTool) : base(innerTool)
        {
        }

        public override IReadOnlyList<object> Metadata { get; } = new List<object>();

        public override async ValueTask<CallToolResult> InvokeAsync(
            RequestContext<CallToolRequestParams> request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dispatcher = request.Services!.GetService<IUIDispatcher>();
            if (dispatcher is null || dispatcher.CheckAccess())
            {
                return await base.InvokeAsync(request, cancellationToken);
            }

            return await dispatcher.InvokeAsync(() => base.InvokeAsync(request, cancellationToken).AsTask());
        }
    }
}