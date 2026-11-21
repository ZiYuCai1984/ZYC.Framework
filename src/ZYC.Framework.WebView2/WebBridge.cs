using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Web;

namespace ZYC.Framework.WebView2;

/// <summary>
///     Dispatches calls that a hosted page sends with <c>postMessage</c> to interfaces marked with
///     <see cref="ExposeToWebAttribute" />, resolved from the dependency injection container.
///     <para>
///         Request: <c>{ "bridge": "call", "id": 1, "service": "GreetingService", "method": "greetAsync", "args": [] }</c>.
///         Reply: <c>{ "bridge": "result", "id": 1, "result": ... }</c> or
///         <c>{ "bridge": "error", "id": 1, "error": "..." }</c>.
///     </para>
/// </summary>
/// <remarks>
///     !WARNING Only pass messages from pages you trust (check <c>CoreWebView2WebMessageReceivedEventArgs.Source</c>):
///     every exposed interface becomes callable by the page !!
/// </remarks>
public class WebBridge
{
    private Dictionary<string, Type>? _services;

    public WebBridge(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
        Logger = lifetimeScope.Resolve<IAppLogger<WebBridge>>();
    }

    /// <summary>
    ///     Gets the JSON options used for arguments and results: camelCase names and enums as strings.
    /// </summary>
    public static JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private ILifetimeScope LifetimeScope { get; }

    private IAppLogger<WebBridge> Logger { get; }

    /// <summary>
    ///     Handles a web message. Returns the reply to post back with <c>PostWebMessageAsJson</c>,
    ///     or <c>null</c> when the message is not a bridge call.
    /// </summary>
    public async Task<string?> HandleAsync(string webMessageAsJson)
    {
        JsonNode? request;
        try
        {
            request = JsonNode.Parse(webMessageAsJson);
        }
        catch (JsonException)
        {
            return null;
        }

        if (request is not JsonObject message
            || message["bridge"]?.GetValueKind() != JsonValueKind.String
            || message["bridge"]!.GetValue<string>() != "call")
        {
            return null;
        }

        var id = message["id"]?.DeepClone();
        try
        {
            var result = await InvokeAsync(
                message["service"]?.GetValue<string>() ?? string.Empty,
                message["method"]?.GetValue<string>() ?? string.Empty,
                message["args"] as JsonArray ?? new JsonArray());

            return new JsonObject
            {
                ["bridge"] = "result",
                ["id"] = id,
                ["result"] = JsonSerializer.SerializeToNode(result, SerializerOptions)
            }.ToJsonString(SerializerOptions);
        }
        catch (Exception e)
        {
            Logger.Warn($"Web bridge call failed: {e}");

            return new JsonObject
            {
                ["bridge"] = "error",
                ["id"] = id,
                ["error"] = e.Message
            }.ToJsonString(SerializerOptions);
        }
    }

    private async Task<object?> InvokeAsync(string serviceName, string methodName, JsonArray args)
    {
        var services = _services ??= FindServices();
        if (!services.TryGetValue(serviceName, out var serviceType))
        {
            throw new InvalidOperationException($"The service '{serviceName}' is not exposed to the web.");
        }

        var methods = serviceType.GetMethods()
            .Where(m => !m.IsSpecialName
                        && !m.ContainsGenericParameters
                        && string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (methods.Length != 1)
        {
            throw new InvalidOperationException(methods.Length == 0
                ? $"The service '{serviceName}' has no method '{methodName}'."
                : $"The method '{serviceName}.{methodName}' is overloaded; overloads are not supported.");
        }

        var method = methods[0];
        var arguments = BindArguments(method, args);
        var target = LifetimeScope.Resolve(serviceType);

        object? returned;
        try
        {
            returned = method.Invoke(target, arguments);
        }
        catch (TargetInvocationException e) when (e.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }

        return await UnwrapAsync(returned, method.ReturnType);
    }

    private static object?[] BindArguments(MethodInfo method, JsonArray args)
    {
        var parameters = method.GetParameters();
        var values = new object?[parameters.Length];
        var argIndex = 0;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            //!WARNING Pages cannot pass a CancellationToken, so the generated TypeScript leaves it out !!
            if (parameter.ParameterType == typeof(CancellationToken))
            {
                values[i] = CancellationToken.None;
                continue;
            }

            if (argIndex < args.Count)
            {
                values[i] = args[argIndex++].Deserialize(parameter.ParameterType, SerializerOptions);
            }
            else if (parameter.HasDefaultValue)
            {
                values[i] = parameter.DefaultValue;
            }
            else
            {
                throw new ArgumentException($"Missing argument '{parameter.Name}' for '{method.Name}'.");
            }
        }

        return values;
    }

    private static async Task<object?> UnwrapAsync(object? returned, Type returnType)
    {
        if (returned is ValueTask valueTask)
        {
            await valueTask;
            return null;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            returned = returnType.GetMethod(nameof(ValueTask<int>.AsTask))!.Invoke(returned, null);
            returnType = typeof(Task<>).MakeGenericType(returnType.GetGenericArguments());
        }

        if (returned is not Task task)
        {
            return returned;
        }

        await task;

        //!WARNING Check the declared type: a non-generic Task is often a Task<VoidTaskResult> at runtime !!
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetProperty(nameof(Task<int>.Result))!.GetValue(task)
            : null;
    }

    private static Dictionary<string, Type> FindServices()
    {
        var services = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
        {
            foreach (var type in GetLoadableTypes(assembly))
            {
                if (type is { IsInterface: true, IsGenericTypeDefinition: false }
                    && type.IsDefined(typeof(ExposeToWebAttribute), false))
                {
                    services.TryAdd(ExposeToWebAttribute.GetServiceName(type), type);
                }
            }
        }

        return services;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.OfType<Type>();
        }
        catch
        {
            return [];
        }
    }
}
