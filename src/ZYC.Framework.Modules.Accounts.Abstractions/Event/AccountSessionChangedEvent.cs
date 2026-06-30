namespace ZYC.Framework.Modules.Accounts.Abstractions.Event;

/// <summary>
///     Published when the active account session changes.
/// </summary>
public class AccountSessionChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AccountSessionChangedEvent" /> class.
    /// </summary>
    /// <param name="session">The current account session.</param>
    public AccountSessionChangedEvent(AccountSession? session)
    {
        Session = session;
    }

    /// <summary>
    ///     Gets the current account session.
    /// </summary>
    public AccountSession? Session { get; }
}
