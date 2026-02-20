namespace ZYC.Framework.Modules.Update.Abstractions;

/// <summary>
///     Represents the current state of the update workflow, including the latest status,
///     an optional available product update, and an optional error.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="UpdateContext" /> is typically produced by update operations (check/download/apply)
///         and can be used to drive UI state, task state, and error reporting.
///     </para>
///     <para>
///         When an operation fails, <see cref="Exception" /> may be populated while <see cref="UpdateStatus" />
///         indicates a faulted/canceled state.
///     </para>
/// </remarks>
public class UpdateContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdateContext" /> class.
    /// </summary>
    /// <param name="updateStatus">
    ///     The update workflow status.
    /// </param>
    /// <param name="newProduct">
    ///     The available product update, if any.
    /// </param>
    /// <param name="exception">
    ///     The exception associated with a faulted operation, if any.
    /// </param>
    public UpdateContext(
        UpdateStatus updateStatus,
        NewProduct? newProduct = null,
        Exception? exception = null)
    {
        UpdateStatus = updateStatus;
        NewProduct = newProduct;
        Exception = exception;
    }

    /// <summary>
    ///     Gets the product update that is available to download/apply, if any.
    /// </summary>
    public NewProduct? NewProduct { get; }

    /// <summary>
    ///     Gets the current status of the update workflow.
    /// </summary>
    public UpdateStatus UpdateStatus { get; }

    /// <summary>
    ///     Gets the exception captured during the last operation, if any.
    /// </summary>
    public Exception? Exception { get; }
}