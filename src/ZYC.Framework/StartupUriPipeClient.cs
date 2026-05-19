using System.IO;
using System.IO.Pipes;
using System.Text;

namespace ZYC.Framework;

internal static class StartupUriPipeClient
{
    private static readonly TimeSpan SendTimeout = TimeSpan.FromSeconds(2);

    private static readonly TimeSpan ConnectSlice = TimeSpan.FromMilliseconds(250);

    public static void TrySend(string pipeName, Uri startupUri)
    {
        var deadline = DateTime.UtcNow + SendTimeout;

        while (true)
        {
            try
            {
                using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.None);
                pipe.Connect(GetConnectTimeout(deadline));

                using var writer = new StreamWriter(pipe, new UTF8Encoding(false));
                writer.AutoFlush = true;

                writer.Write(startupUri.ToString());
                return;
            }
            catch (TimeoutException) when (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(50);
            }
            catch (IOException) when (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(50);
            }
            catch
            {
                return;
            }
        }
    }

    private static int GetConnectTimeout(DateTime deadline)
    {
        var remaining = deadline - DateTime.UtcNow;
        var timeout = Math.Min(ConnectSlice.TotalMilliseconds, remaining.TotalMilliseconds);
        return Math.Max(1, (int)timeout);
    }
}
