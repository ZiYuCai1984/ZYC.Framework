using System.Windows.Input;

namespace ZYC.Framework.Modules.NuGet.Abstractions.Commands;

public interface IClearNuGetHttpCacheCommand : ICommand
{
    void Execute()
    {
        Execute(null);
    }
}