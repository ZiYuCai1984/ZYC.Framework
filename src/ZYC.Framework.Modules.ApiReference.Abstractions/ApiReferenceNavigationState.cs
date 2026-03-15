using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.ApiReference.Abstractions;

/// <summary>
///     Represents the navigation state for the API Reference component.
///     This state is typically used to track the current location within the API documentation.
/// </summary>
public class ApiReferenceNavigationState : IState
{
    /// <summary>
    ///     Gets or sets the uniform resource identifier (URI) of the current API reference page.
    ///     Defaults to an empty string.
    /// </summary>
    public string Uri { get; set; } = "";
}