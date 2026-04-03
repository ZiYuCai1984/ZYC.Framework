using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Core.WindowTitle;

namespace ZYC.Framework.WindowTitle.BuildIn;

[RegisterSingleInstance]
internal class GitHubTitleItem : WindowTitleItem
{
    public GitHubTitleItem(OpenGitHubCommand openGitHubCommand) : base("Github",
        null!)
    {
        OpenGitHubCommand = openGitHubCommand;
    }

    private OpenGitHubCommand OpenGitHubCommand { get; }


    public override ICommand Command => OpenGitHubCommand;
}