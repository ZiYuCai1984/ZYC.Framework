using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Win32;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Core.DragDrop;


[Register]
public partial class DragDropPickerView
{
    public DragDropPickerView()
    {
        InitializeComponent();
    }

    public event EventHandler<PathsPickedEventArgs>? PathsPicked;

    #region Dependency Properties

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(DragDropPickerView),
            new PropertyMetadata("FolderMoveOutline"));


    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(DragDropPickerView),
            new PropertyMetadata("Drag files (or folders) here"));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty HintProperty =
        DependencyProperty.Register(nameof(Hint), typeof(string),
            typeof(DragDropPickerView),
            new PropertyMetadata("Click to browse"));

    public string Hint
    {
        get => (string)GetValue(HintProperty);
        set => SetValue(HintProperty, value);
    }

    public static readonly DependencyProperty PickerKindProperty =
        DependencyProperty.Register(nameof(PickerKind), typeof(PickerKind), typeof(DragDropPickerView),
            new PropertyMetadata(PickerKind.FileOrFolder));

    public PickerKind PickerKind
    {
        get => (PickerKind)GetValue(PickerKindProperty);
        set => SetValue(PickerKindProperty, value);
    }

    public static readonly DependencyProperty AllowMultipleProperty =
        DependencyProperty.Register(nameof(AllowMultiple), typeof(bool), typeof(DragDropPickerView),
            new PropertyMetadata(false));

    public bool AllowMultiple
    {
        get => (bool)GetValue(AllowMultipleProperty);
        set => SetValue(AllowMultipleProperty, value);
    }

    public static readonly DependencyProperty FileFilterProperty =
        DependencyProperty.Register(nameof(FileFilter), typeof(string), typeof(DragDropPickerView),
            new PropertyMetadata("All files (*.*)|*.*"));

    public string FileFilter
    {
        get => (string)GetValue(FileFilterProperty);
        set => SetValue(FileFilterProperty, value);
    }

    public static readonly DependencyProperty SelectedPathsProperty =
        DependencyProperty.Register(nameof(SelectedPaths), typeof(string[]), typeof(DragDropPickerView),
            new PropertyMetadata(Array.Empty<string>(), OnSelectedPathsChanged));

    public string[] SelectedPaths
    {
        get => (string[])GetValue(SelectedPathsProperty);
        set => SetValue(SelectedPathsProperty, value);
    }

    public static readonly DependencyProperty IsDragOverProperty =
        DependencyProperty.Register(nameof(IsDragOver), typeof(bool), typeof(DragDropPickerView),
            new PropertyMetadata(false));

    public bool IsDragOver
    {
        get => (bool)GetValue(IsDragOverProperty);
        set => SetValue(IsDragOverProperty, value);
    }

    public static readonly DependencyProperty SelectedSummaryProperty =
        DependencyProperty.Register(nameof(SelectedSummary), typeof(string), typeof(DragDropPickerView),
            new PropertyMetadata("No selection"));

    public string SelectedSummary
    {
        get => (string)GetValue(SelectedSummaryProperty);
        set => SetValue(SelectedSummaryProperty, value);
    }

    private static void OnSelectedPathsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var c = (DragDropPickerView)d;
        var paths = e.NewValue as string[] ?? [];
        if (paths.Length == 0)
        {
            c.SelectedSummary = "No selection";
        }
        else if (paths.Length == 1)
        {
            c.SelectedSummary = paths[0];
        }
        else
        {
            var s = new StringBuilder();
            s.AppendLine($"{paths.Length} item(s) selected :");

            foreach (var path in paths)
            {
                s.AppendLine(path);
            }

            c.SelectedSummary = s.ToString();
        }

        c.PathsPicked?.Invoke(c, new PathsPickedEventArgs(paths));
    }

    #endregion

    #region Drag&Drop

    private void OnDragEnter(object sender, DragEventArgs e)
    {
        IsDragOver = CanAcceptDrag(e);
        e.Effects = IsDragOver ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        IsDragOver = CanAcceptDrag(e);
        e.Effects = IsDragOver ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        IsDragOver = false;
        e.Handled = true;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        try
        {
            IsDragOver = false;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var raw = (string[])e.Data.GetData(DataFormats.FileDrop)!;
            var accepted = FilterByKind(raw, PickerKind);

            if (accepted.Count == 0)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            if (!AllowMultiple)
            {
                SelectedPaths = [accepted[0]];
            }
            else
            {
                SelectedPaths = accepted.ToArray();
            }

            e.Effects = DragDropEffects.Copy;
        }
        finally
        {
            e.Handled = true;
        }
    }

    private bool CanAcceptDrag(DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return false;
        }

        var raw = (string[])e.Data.GetData(DataFormats.FileDrop)!;

        var accepted = FilterByKind(raw, PickerKind);
        return accepted.Count > 0;
    }

    private static List<string> FilterByKind(IEnumerable<string> paths, PickerKind kind)
    {
        var list = new List<string>();
        foreach (var p in paths.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            var isFile = File.Exists(p);
            var isDir = Directory.Exists(p);

            if (kind == PickerKind.File && isFile)
            {
                list.Add(p);
            }
            else if (kind == PickerKind.Folder && isDir)
            {
                list.Add(p);
            }
            else if (kind == PickerKind.FileOrFolder && (isFile || isDir))
            {
                list.Add(p);
            }
        }

        return list;
    }

    #endregion

    #region Click => Dialog

    private void OnOverlayMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (PickerKind == PickerKind.FileOrFolder)
        {
            var menu = new ContextMenu();

            var miFile = new MenuItem { Header = "Select file" };
            miFile.Click += (_, _) => PickFile();

            var miFolder = new MenuItem { Header = "Select folder" };
            miFolder.Click += (_, _) => PickFolder();

            menu.Items.Add(miFile);
            menu.Items.Add(miFolder);

            menu.PlacementTarget = DropBorder;
            menu.IsOpen = true;
            return;
        }

        if (PickerKind == PickerKind.File)
        {
            PickFile();
        }
        else
        {
            PickFolder();
        }
    }

    private void PickFile()
    {
        var dlg = new OpenFileDialog
        {
            Multiselect = AllowMultiple,
            Filter = string.IsNullOrWhiteSpace(FileFilter) ? "All files (*.*)|*.*" : FileFilter,
            CheckFileExists = true,
            CheckPathExists = true
        };

        var owner = GetOwnerWindow();
        var ok = owner is null ? dlg.ShowDialog() == true : dlg.ShowDialog(owner) == true;

        if (!ok)
        {
            return;
        }

        var files = dlg.FileNames.Where(File.Exists).ToArray();
        if (files.Length == 0)
        {
            return;
        }

        SelectedPaths = AllowMultiple ? files : [files[0]];
    }

    private void PickFolder()
    {
        var owner = GetOwnerWindow();
        var ownerHwnd = owner is null ? 0 : new WindowInteropHelper(owner).EnsureHandle();

        var folder = FolderPickerTools.PickFolder(ownerHwnd, "Select folder");
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        if (!Directory.Exists(folder))
        {
            return;
        }

        SelectedPaths = [folder];
    }

    private Window? GetOwnerWindow()
    {
        return Window.GetWindow(this);
    }

    #endregion
}