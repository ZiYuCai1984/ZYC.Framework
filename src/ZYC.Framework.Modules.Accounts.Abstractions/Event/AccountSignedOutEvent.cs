namespace ZYC.Framework.Modules.Accounts.Abstractions.Event;

/// <summary>
///     Published after an account signs out.
/// </summary>
public class AccountSignedOutEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AccountSignedOutEvent" /> class.
    /// </summary>
    /// <param name="providerId">The signed-out provider id.</param>
    public AccountSignedOutEvent(string providerId)
    {
        ProviderId = providerId;
    }

    /// <summary>
    ///     Gets the signed-out provider id.
    /// </summary>
    public string ProviderId { get; }
}
