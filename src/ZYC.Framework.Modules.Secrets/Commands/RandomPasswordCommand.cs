using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Secrets.Abstractions;
using ZYC.Framework.Modules.Secrets.UI;

namespace ZYC.Framework.Modules.Secrets.Commands;

[Register]
internal sealed class RandomPasswordCommand : CommandBase, IViewSetter<PasswordGeneratorView>
{
    public RandomPasswordCommand(
        PasswordGeneratorOptionsState passwordGeneratorOptions,
        IAppLogger<RandomPasswordCommand> logger)
    {
        PasswordGeneratorOptions = passwordGeneratorOptions;
        Logger = logger;
    }

    private PasswordGeneratorOptionsState PasswordGeneratorOptions { get; }

    private IAppLogger<RandomPasswordCommand> Logger { get; }

    public PasswordGeneratorView? View { get; set; }
    public bool Disposing { get; set; }

    protected override void InternalExecute(object? parameter)
    {
        if (View == null)
        {
            DebuggerTools.Break();
            return;
        }

        var count = Math.Max(1, PasswordGeneratorOptions.Count);

        var list = new List<string>(count);

        try
        {
            for (var i = 0; i < count; i++)
            {
                var pwd = GenerateOne(PasswordGeneratorOptions);
                list.Add(pwd);
            }
        }
        catch (Exception e)
        {
            MessageBoxTools.Error(e);
            Logger.Error(e);
        }

        View.UpdatePasswords(list);
    }

    private static string GenerateOne(PasswordGeneratorOptionsState options)
    {
        var password = PasswordGeneratorTools.GeneratePassword(
            options.PasswordCharOptions,
            options.Length);

        return password;
    }
}