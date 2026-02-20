namespace ZYC.Framework.Abstractions.Notification.Banner;

public interface IBannerManager
{
    void PromptRestart();

    void Prompt<T>() where T : IBanner;
}