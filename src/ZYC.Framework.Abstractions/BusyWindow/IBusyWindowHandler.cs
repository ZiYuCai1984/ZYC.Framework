namespace ZYC.Framework.Abstractions.BusyWindow;

public interface IBusyWindowHandler : IDisposable
{
    string StatusText { get; set; }

    bool ShowProgress { get; set; }

    bool IsProgressIndeterminate { get; set; }

    double ProgressValue { get; set; }

    string Title { get; set; }

    void Close();
}