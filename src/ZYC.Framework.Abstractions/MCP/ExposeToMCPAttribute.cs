namespace ZYC.Framework.Abstractions.MCP;

/// <summary>
///     Specifies that an interface or method should be exposed to the Model Context Protocol (MCP).
///     This attribute allows for hierarchical configuration where method-level settings can override interface-level
///     defaults.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
public class ExposeToMCPAttribute : Attribute
{
    /// <summary>
    ///     Gets or sets the custom name for the tool in MCP.
    ///     If null, the default naming convention (usually the method name) will be used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets whether the logic should be executed on the UI thread.
    ///     Note: This property is the backing value; use <see cref="RequiresUIThread" /> to ensure the explicit set flag is
    ///     triggered.
    /// </summary>
    public bool InvokeOnUIThread { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the UI thread requirement was explicitly configured at this level.
    ///     Used by the attribute resolver to determine if it should override settings from a higher level (e.g., an
    ///     interface).
    /// </summary>
    public bool IsExplicitlySet { get; private set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the execution requires the UI thread.
    ///     Setting this property automatically updates <see cref="IsExplicitlySet" /> to true.
    /// </summary>
    public bool RequiresUIThread
    {
        get => InvokeOnUIThread;
        set
        {
            InvokeOnUIThread = value;
            IsExplicitlySet = true;
        }
    }
}