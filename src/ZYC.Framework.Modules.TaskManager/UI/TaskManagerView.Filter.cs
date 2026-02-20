using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager.UI;

internal partial class TaskManagerView
{
    private CollectionViewSource ManagedTasksCollectionViewSource { get; } = new();

    public ICollectionView ManagedTasksCollectionView => ManagedTasksCollectionViewSource.View;

    public int CollectionViewCount => ManagedTasksCollectionView.Cast<object>().Count();

    private ObservableCollection<ManagedTaskWrapper> ManagedTasks { get; } = new();

    public FilterType FilterType
    {
        get => TaskManagerState.FilterType;
        set
        {
            if (TaskManagerState.FilterType == value)
            {
                return;
            }


            TaskManagerState.FilterType = value;
            OnPropertyChanged();
        }
    }

    public string? FilterText
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    private void RefreshCollectionView()
    {
        ManagedTasksCollectionView.Refresh();
        OnPropertyChanged(nameof(CollectionViewCount));
    }

    private void OnFilter(object sender, FilterEventArgs e)
    {
        if (e.Item is not ManagedTaskWrapper t)
        {
            e.Accepted = false;
            return;
        }

        if (!MatchFilterType(t, FilterType))
        {
            e.Accepted = false;
            return;
        }

        var raw = FilterText;
        if (string.IsNullOrWhiteSpace(raw))
        {
            e.Accepted = true;
            return;
        }

        var tokens = raw.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var token in tokens)
        {
            if (!MatchToken(t, token))
            {
                e.Accepted = false;
                return;
            }
        }

        e.Accepted = true;
    }

    private static bool MatchToken(ManagedTaskWrapper t, string token)
    {
        return Contains(t.DefinitionId, token)
               || Contains(t.ProviderId, token)
               || Contains(t.StatusText, token)
               || Contains(t.FaultText, token);
    }

    private static bool Contains(string? hay, string needle)
    {
        return !string.IsNullOrEmpty(hay) && hay.Contains(needle, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MatchFilterType(ManagedTaskWrapper t, FilterType ft)
    {
        return ft switch
        {
            FilterType.All => true,
            FilterType.Running => t.State == ManagedTaskState.Running,
            FilterType.Paused => t.State == ManagedTaskState.Paused,
            FilterType.Completed => t.State == ManagedTaskState.Completed,
            FilterType.Faulted => t.State == ManagedTaskState.Faulted,
            FilterType.Canceled => t.State == ManagedTaskState.Canceled,
            _ => true
        };
    }
}