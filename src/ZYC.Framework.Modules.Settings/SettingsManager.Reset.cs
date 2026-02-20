using System.Reflection;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;
using ZYC.Framework.Modules.Secrets.Abstractions;

namespace ZYC.Framework.Modules.Settings;

public partial class SettingsManager
{
    public async Task ResetStatesAsync()
    {
        await Task.CompletedTask;

        var states = LifetimeScope.Resolve<IState[]>();
        foreach (var state in states)
        {
            if (state.GetType().ExistAttribute<SkipResetAttribute>())
            {
                continue;
            }

            ResetObject(state);
        }
    }

    public async Task ResetConfigsAsync()
    {
        await Task.CompletedTask;

        var config = Configs.ToArray();

        foreach (var c in config)
        {
            if (c.GetType().ExistAttribute<SkipResetAttribute>())
            {
                continue;
            }

            ResetObject(c);
        }
    }

    public async Task ResetSecretsAsync()
    {
        await Task.CompletedTask;

        var secretsManager = LifetimeScope.Resolve<ISecretsManager>();
        var secrets = secretsManager.GetSecretsConfigs();
        foreach (var s in secrets)
        {
            if (s.GetType().ExistAttribute<SkipResetAttribute>())
            {
                continue;
            }

            ResetObject(s);
        }
    }

    /// <summary>
    ///     Reset property value to new object instance's property value.
    /// </summary>
    private static void ResetObject(object obj)
    {
        var type = obj.GetType();
        var defaultInstance = Activator.CreateInstance(type);

        if (defaultInstance == null)
        {
            return;
        }

        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);

        foreach (var prop in props)
        {
            if (prop.ExistAttribute<SkipResetAttribute>())
            {
                continue;
            }

            var defaultValue = prop.GetValue(defaultInstance);
            prop.SetValue(obj, defaultValue);
        }
    }
}