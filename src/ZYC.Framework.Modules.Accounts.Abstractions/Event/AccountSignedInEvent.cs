namespace ZYC.Framework.Modules.Accounts.Abstractions.Event;

/// <summary>
///     Published after an account signs in.
/// </summary>
public class AccountSignedInEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AccountSignedInEvent" /> class.
    /// </summary>
    /// <param name="session">The signed-in session.</param>
    public AccountSignedInEvent(AccountSession session)
    {
        Session = session;
    }

    /// <summary>
    ///     Gets the signed-in session.
    /// </summary>
    public AccountSession Session { get; }
}
