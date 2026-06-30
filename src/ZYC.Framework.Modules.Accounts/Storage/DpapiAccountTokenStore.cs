using System.IO;
using System.Security.Cryptography;
using System.Text;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Accounts.Abstractions;

namespace ZYC.Framework.Modules.Accounts.Storage;

[RegisterSingleInstanceAs(typeof(IAccountTokenStore))]
internal class DpapiAccountTokenStore : IAccountTokenStore
{
    public DpapiAccountTokenStore(IAppContext appContext)
    {
        RootDirectory = Path.Combine(appContext.GetSettingsDirectory(), "accounts");
    }

    private string RootDirectory { get; }

    public async Task<byte[]?> GetAsync(string providerId, string key, CancellationToken cancellationToken)
    {
        var path = GetPath(providerId, key);
        if (!File.Exists(path))
        {
            return null;
        }

        var protectedPayload = await File.ReadAllBytesAsync(path, cancellationToken);
        try
        {
            return ProtectedData.Unprotect(protectedPayload, GetEntropy(providerId, key), DataProtectionScope.CurrentUser);
        }
        catch (CryptographicException)
        {
            File.Delete(path);
            return null;
        }
    }

    public async Task SetAsync(string providerId, string key, byte[] payload, CancellationToken cancellationToken)
    {
        var path = GetPath(providerId, key);
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var protectedPayload = ProtectedData.Protect(payload, GetEntropy(providerId, key), DataProtectionScope.CurrentUser);
        await File.WriteAllBytesAsync(path, protectedPayload, cancellationToken);
    }

    public Task RemoveAsync(string providerId, string key, CancellationToken cancellationToken)
    {
        var path = GetPath(providerId, key);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    public Task ClearProviderAsync(string providerId, CancellationToken cancellationToken)
    {
        var providerDirectory = Path.Combine(RootDirectory, Sanitize(providerId));
        if (Directory.Exists(providerDirectory))
        {
            Directory.Delete(providerDirectory, true);
        }

        return Task.CompletedTask;
    }

    private string GetPath(string providerId, string key)
    {
        return Path.Combine(RootDirectory, Sanitize(providerId), $"{Sanitize(key)}.bin");
    }

    private static byte[] GetEntropy(string providerId, string key)
    {
        return Encoding.UTF8.GetBytes($"ZYC.Framework.Accounts:{providerId}:{key}");
    }

    private static string Sanitize(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var chars = value.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray();
        return new string(chars);
    }
}
