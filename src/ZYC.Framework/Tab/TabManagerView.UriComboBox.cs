using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Tab;

internal partial class TabManagerView
{
    private bool _suppressSelectionChanged;

    private async Task CommitAndNavigateAsync(ComboBox cb)
    {
        var raw = cb.Text;

        string? uri = null;
        try
        {
            uri = UriTools.NormalizeUri(raw);
        }
        catch
        {
            //ignore
        }

        if (uri is null)
        {
            return;
        }

        cb.Text = uri;

        BindingOperations.GetBindingExpression(cb, ComboBox.TextProperty)?.UpdateSource();

        await StartNavigateAsync(uri);
    }


    private async void OnUriComboBoxKeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            e.Handled = true;

            await CommitAndNavigateAsync((ComboBox)sender);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private async void OnGoButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await CommitAndNavigateAsync(UriComboBox);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private async void OnUriComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_suppressSelectionChanged)
            {
                return;
            }

            var cb = (ComboBox)sender;
            if (cb.SelectedItem is null)
            {
                return;
            }

            try
            {
                _suppressSelectionChanged = true;

                cb.Text = cb.SelectedItem.ToString();

                await CommitAndNavigateAsync(cb);
            }
            finally
            {
                _suppressSelectionChanged = false;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private void OnUriComboBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        //ignore
    }

    private void OnUriComboBoxGotFocus(object sender, RoutedEventArgs e)
    {
        LifetimeScope.Resolve<IParallelWorkspaceManager>().SetFocusedWorkspace(WorkspaceNode);
    }
}