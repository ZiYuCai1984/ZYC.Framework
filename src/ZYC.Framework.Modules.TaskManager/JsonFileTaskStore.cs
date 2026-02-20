using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager;

[RegisterSingleInstanceAs(typeof(ITaskStore))]
internal sealed class JsonFileTaskStore : ITaskStore
{
    private readonly SemaphoreSlim _ioLock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _path;

    public JsonFileTaskStore(IAppContext appContext)
    {
        _path = Path.Combine(appContext.GetMainAppDirectory(), "tasks.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<IReadOnlyList<TaskRecord>> LoadAllAsync(CancellationToken ct)
    {
        await _ioLock.WaitAsync(ct);
        try
        {
            if (!File.Exists(_path))
            {
                return Array.Empty<TaskRecord>();
            }

            var json = await File.ReadAllTextAsync(_path, ct);
            if (string.IsNullOrWhiteSpace(json))
            {
                return Array.Empty<TaskRecord>();
            }

            var list = JsonSerializer.Deserialize<List<TaskRecord>>(json, _jsonOptions);
            if (list == null)
            {
                return [];
            }

            return list.ToArray();
        }
        finally
        {
            _ioLock.Release();
        }
    }

    public async Task SaveAllAsync(IReadOnlyList<TaskRecord> records, CancellationToken ct)
    {
        await _ioLock.WaitAsync(ct);
        try
        {
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var temp = _path + ".tmp";
            var json = JsonSerializer.Serialize(records, _jsonOptions);
            await File.WriteAllTextAsync(temp, json, ct);

            // atomic-ish replace
            File.Move(temp, _path, true);
        }
        finally
        {
            _ioLock.Release();
        }
    }
}