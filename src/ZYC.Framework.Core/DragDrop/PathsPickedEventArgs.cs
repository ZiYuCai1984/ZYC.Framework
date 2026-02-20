namespace ZYC.Framework.Core.DragDrop;

public class PathsPickedEventArgs : EventArgs
{
    public PathsPickedEventArgs(string[] paths)
    {
        Paths = paths;
    }

    public string[] Paths { get; }
}