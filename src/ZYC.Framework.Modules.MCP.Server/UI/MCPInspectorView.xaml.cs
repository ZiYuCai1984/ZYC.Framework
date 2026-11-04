using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.MCP.Server.Abstractions;
using ZYC.Framework.Modules.MCP.Server.Abstractions.Event;

namespace ZYC.Framework.Modules.MCP.Server.UI;

[Register]
internal partial class MCPInspectorView
{
    private const string InspectorClientName = "ZYC.Framework.MCP.Inspector";

    private string _argumentsText = string.Empty;
    private McpClient? _client;
    private bool _isBusy;
    private string _resultContentText = string.Empty;
    private string _resultRawText = string.Empty;
    private string _resultSummary = string.Empty;
    private string _searchText = string.Empty;
    private MCPToolItem? _selectedTool;
    private bool _showRawResult;
    private MCPServiceStatus _status = MCPServiceStatus.Stopped();
    private ICollectionView? _toolsView;

    public MCPInspectorView(
        IMCPServerManager mcpServerManager,
        MCPServerConfig mcpServerConfig,
        IEventAggregator eventAggregator,
        IToastManager toastManager,
        IAppLogger<MCPInspectorView> logger)
    {
        MCPServerManager = mcpServerManager;
        MCPServerConfig = mcpServerConfig;
        EventAggregator = eventAggregator;
        ToastManager = toastManager;
        Logger = logger;
    }

    private IMCPServerManager MCPServerManager { get; }

    private MCPServerConfig MCPServerConfig { get; }

    private IEventAggregator EventAggregator { get; }

    private IToastManager ToastManager { get; }

    private IAppLogger<MCPInspectorView> Logger { get; }

    public ObservableCollection<MCPToolItem> Tools { get; } = new();

    public ICollectionView ToolsView => _toolsView ??= CreateToolsView();

    public string ToolsSummary => $"{Tools.Count} tools";

    public string StatusText => _status.ServiceStatus.ToString();

    public string? LastError => _status.Exception?.Message;

    public string Endpoint => $"http://127.0.0.1:{MCPServerConfig.Port}";

    public bool IsRunning => _status.ServiceStatus == ServiceStatus.Running;

    public bool CanStart => _status.ServiceStatus == ServiceStatus.Stopped;

    public bool CanStop => IsRunning;

    public bool CanRefresh => IsRunning && !IsBusy;

    public bool CanExecuteTool => IsRunning && !IsBusy && SelectedTool is not null;

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanRefresh));
            OnPropertyChanged(nameof(CanExecuteTool));
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            ToolsView.Refresh();
        }
    }

    public MCPToolItem? SelectedTool
    {
        get => _selectedTool;
        set
        {
            if (ReferenceEquals(_selectedTool, value))
            {
                return;
            }

            _selectedTool = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanExecuteTool));

            ResetArguments();
            SetResult(string.Empty, string.Empty, string.Empty);
        }
    }

    public string ArgumentsText
    {
        get => _argumentsText;
        set
        {
            _argumentsText = value;
            OnPropertyChanged();
        }
    }

    public bool ShowRawResult
    {
        get => _showRawResult;
        set
        {
            _showRawResult = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ResultText));
        }
    }

    public string ResultSummary
    {
        get => _resultSummary;
        private set
        {
            _resultSummary = value;
            OnPropertyChanged();
        }
    }

    public string ResultText => ShowRawResult ? _resultRawText : _resultContentText;

    protected override void InternalOnLoaded()
    {
        base.InternalOnLoaded();

        CompositeDisposable.Add(
            EventAggregator.Subscribe<MCPServiceStatusChangedEvent>(OnMCPServiceStatusChanged, true));

        _ = UpdateStatusAsync(MCPServerManager.GetStatusSnapshot());
    }

    public override void Dispose()
    {
        base.Dispose();

        _ = DisposeClientAsync();
    }

    private void OnMCPServiceStatusChanged(MCPServiceStatusChangedEvent e)
    {
        _ = UpdateStatusAsync(e.MCPServiceStatus);
    }

    private async Task UpdateStatusAsync(MCPServiceStatus status)
    {
        _status = status;

        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(LastError));
        OnPropertyChanged(nameof(Endpoint));
        OnPropertyChanged(nameof(IsRunning));
        OnPropertyChanged(nameof(CanStart));
        OnPropertyChanged(nameof(CanStop));
        OnPropertyChanged(nameof(CanRefresh));
        OnPropertyChanged(nameof(CanExecuteTool));

        if (IsRunning)
        {
            await RefreshToolsAsync();
            return;
        }

        SelectedTool = null;
        Tools.Clear();
        OnPropertyChanged(nameof(ToolsSummary));

        await DisposeClientAsync();
    }

    private async Task RefreshToolsAsync()
    {
        if (!IsRunning || IsBusy)
        {
            return;
        }

        IsBusy = true;

        try
        {
            var client = await GetClientAsync();
            var tools = await client.ListToolsAsync();
            var selectedName = SelectedTool?.Name;

            Tools.Clear();

            foreach (var tool in tools
                         .Select(t => t.ProtocolTool)
                         .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase))
            {
                Tools.Add(new MCPToolItem(tool));
            }

            SelectedTool = Tools.FirstOrDefault(t => t.Name == selectedName);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            ToastManager.PromptException(e);

            await DisposeClientAsync();
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(ToolsSummary));
        }
    }

    private async Task<McpClient> GetClientAsync()
    {
        if (_client is not null)
        {
            return _client;
        }

        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(Endpoint),
            Name = InspectorClientName
        });

        _client = await McpClient.CreateAsync(transport, new McpClientOptions
        {
            ClientInfo = new Implementation
            {
                Name = InspectorClientName,
                Version = ProductInfo.Version
            }
        });

        return _client;
    }

    private async Task DisposeClientAsync()
    {
        var client = _client;
        _client = null;

        if (client is null)
        {
            return;
        }

        try
        {
            await client.DisposeAsync();
        }
        catch
        {
            //ignore
        }
    }

    private ICollectionView CreateToolsView()
    {
        var view = CollectionViewSource.GetDefaultView(Tools);
        view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MCPToolItem.Group)));
        view.Filter = FilterTool;
        return view;
    }

    private bool FilterTool(object item)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return true;
        }

        return item is MCPToolItem tool &&
               (tool.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                tool.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
    }

    private void ResetArguments()
    {
        ArgumentsText = SelectedTool is null
            ? string.Empty
            : MCPInspectorJson.CreateArgumentsTemplate(SelectedTool.InputSchema);
    }

    private void SetResult(string summary, string contentText, string rawText)
    {
        ResultSummary = summary;
        _resultContentText = contentText;
        _resultRawText = rawText;
        OnPropertyChanged(nameof(ResultText));
    }

    private async void OnExecuteButtonClick(object sender, RoutedEventArgs e)
    {
        var tool = SelectedTool;
        if (tool is null || !CanExecuteTool)
        {
            return;
        }

        Dictionary<string, JsonElement>? arguments;

        try
        {
            arguments = MCPInspectorJson.ParseArguments(ArgumentsText);
        }
        catch (JsonException ex)
        {
            SetResult("Invalid arguments JSON", ex.Message, string.Empty);
            return;
        }

        IsBusy = true;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var client = await GetClientAsync();
            var result = await client.CallToolAsync(new CallToolRequestParams
            {
                Name = tool.Name,
                Arguments = arguments
            });

            stopwatch.Stop();

            SetResult(
                $"{(result.IsError == true ? "Error" : "OK")} · {stopwatch.ElapsedMilliseconds} ms",
                MCPInspectorJson.FormatContent(result),
                MCPInspectorJson.Serialize(result));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex);

            SetResult($"Failed · {stopwatch.ElapsedMilliseconds} ms", ex.Message, ex.ToString());

            if (ex is not McpException)
            {
                await DisposeClientAsync();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnResetArgumentsButtonClick(object sender, RoutedEventArgs e)
    {
        ResetArguments();
    }

    private void OnCopyEndpointButtonClick(object sender, RoutedEventArgs e)
    {
        ClipboardTools.SetText(Endpoint);
    }

    private async void OnRefreshButtonClick(object sender, RoutedEventArgs e)
    {
        await RefreshToolsAsync();
    }

    private async void OnStartButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await MCPServerManager.StartServerAsync();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private async void OnStopButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await MCPServerManager.StopServerAsync();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }
}