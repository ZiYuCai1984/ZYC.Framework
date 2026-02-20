namespace ZYC.Framework.WebView2;

public partial class WebViewHostBase
{
    public async Task ClickElementAsync(string getElementScript)
    {
        await InvokeElementMethodAsync(getElementScript, "click");
    }

    public async Task ClickElementByClassNameAsync(string className, int index = 0)
    {
        await ClickElementAsync($"{Document_GetElementsByClassName}('{className}')[{index}]");
    }

    public async Task ClickElementByIdAsync(string id)
    {
        await ClickElementAsync($"{Document_GetElementById}('{id}')");
    }

    public async Task ClickElementByNameAsync(string name, int index = 0)
    {
        await ClickElementAsync($"{Document_GetElementsByName}('{name}')[{index}]");
    }

    public async Task ClickElementByTagNameAsync(string tagName, int index = 0)
    {
        await ClickElementAsync($"{Document_GetElementsByTagName}('{tagName}')[{index}]");
    }
}