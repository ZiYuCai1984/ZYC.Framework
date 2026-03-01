namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Defines a component capable of handling a drop operation.
/// </summary>
public interface IDropHandler
{
    /// <summary>
    ///     Gets the execution order of the handler.
    ///     Lower values indicate higher priority during resolution.
    /// </summary>
    int Order { get; }

    /// <summary>
    ///     Determines whether this handler can process the specified drop payload
    ///     within the given context.
    /// </summary>
    /// <param name="payload">The parsed drop data.</param>
    /// <param name="context">The contextual information of the drop operation.</param>
    /// <returns>
    ///     <c>true</c> if this handler can handle the drop; otherwise, <c>false</c>.
    /// </returns>
    bool CanHandle(DropPayload payload, DropContext context);

    /// <summary>
    ///     Executes the drop handling logic asynchronously.
    /// </summary>
    /// <param name="payload">The parsed drop data.</param>
    /// <param name="context">The contextual information of the drop operation.</param>
    /// <returns>A task representing the asynchronous handling operation.</returns>
    Task HandleAsync(DropPayload payload, DropContext context);
}