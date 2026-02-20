using ZYC.CoreToolkit;

namespace ZYC.Framework.WebView2;

public partial class WebViewHostBase
{
    // ReSharper disable InconsistentNaming
    private static string Document_GetElementsByClassName => "document.getElementsByClassName";
    private static string Document_GetElementById => "document.getElementById";
    private static string Document_GetElementsByName => "document.getElementsByName";
    private static string Document_GetElementsByTagName => "document.getElementsByTagName";

    public async Task<string> GetElementPropertyAsync(string getElementScript, string property)
    {
        return await ExecuteScriptAsync($"{getElementScript}.{property}");
    }


    public async Task<string> GetElementPropertyByClassNameAsync(string className, string property, int index = 0)
    {
        return await GetElementPropertyAsync($"{Document_GetElementsByClassName}('{className}')[{index}]", property);
    }

    public async Task<string> GetElementPropertyByIdAsync(string id, string property)
    {
        return await GetElementPropertyAsync($"{Document_GetElementById}('{id}')", property);
    }

    public async Task<string> GetElementPropertyByNameAsync(string name, string property, int index = 0)
    {
        return await GetElementPropertyAsync($"{Document_GetElementsByName}('{name}')[{index}]", property);
    }

    public async Task<string> GetElementPropertyByTagNameAsync(string tagName, string property, int index = 0)
    {
        return await GetElementPropertyAsync($"{Document_GetElementsByTagName}('{tagName}')[{index}]", property);
    }


    public async Task SetElementPropertyAsync(string getElementScript, string property, string value)
    {
        await ExecuteScriptAsync($"{getElementScript}.{property}={value};");
    }

    public async Task SetElementPropertyByClassNameAsync(string className, string property, string value, int index = 0)
    {
        await SetElementPropertyAsync($"{Document_GetElementsByClassName}('{className}')[{index}]", property, value);
    }

    public async Task SetElementPropertyByIdAsync(string id, string property, string value)
    {
        await SetElementPropertyAsync($"{Document_GetElementById}('{id}')", property, value);
    }

    public async Task SetElementPropertyByNameAsync(string name, string property, string value, int index = 0)
    {
        await SetElementPropertyAsync($"{Document_GetElementsByName}('{name}')[{index}]", property, value);
    }

    public async Task SetElementPropertyByTagNameAsync(string tagName, string property, string value, int index = 0)
    {
        await SetElementPropertyAsync($"{Document_GetElementsByTagName}('{tagName}')[{index}]", property, value);
    }


    public async Task InvokeElementMethodAsync(string getElementScript, string methodName, string args = "")
    {
        var script = $"{getElementScript}.{methodName}({args});";
        await ExecuteScriptAsync(script);
    }

    public async Task InvokeElementMethodByClassNameAsync(string className, string methodName, string args = "",
        int index = 0)
    {
        await InvokeElementMethodAsync($"{Document_GetElementsByClassName}('{className}')[{index}]", methodName, args);
    }

    public async Task InvokeElementMethodByIdAsync(string id, string methodName, string args = "")
    {
        await InvokeElementMethodAsync($"{Document_GetElementById}('{id}')", methodName, args);
    }

    public async Task InvokeElementMethodByNameAsync(string name, string methodName, string args = "", int index = 0)
    {
        await InvokeElementMethodAsync($"{Document_GetElementsByName}('{name}')[{index}]", methodName, args);
    }

    public async Task InvokeElementMethodByTagNameAsync(string tagName, string methodName, string args = "",
        int index = 0)
    {
        await InvokeElementMethodAsync($"{Document_GetElementsByTagName}('{tagName}')[{index}]", methodName, args);
    }

    public async Task<T> InvokeElementMethodAsync<T>(string getElementScript, string methodName, string args = "")
    {
        var result = await ExecuteScriptAsync($"{getElementScript}.{methodName}({args});");
        return JsonTools.Deserialize<T>(result)!;
    }

    public async Task<T> InvokeElementMethodByClassNameAsync<T>(string className, string methodName, string args = "",
        int index = 0)
    {
        return await InvokeElementMethodAsync<T>($"{Document_GetElementsByClassName}('{className}')[{index}]",
            methodName,
            args);
    }

    public async Task<T> InvokeElementMethodByIdAsync<T>(string id, string methodName, string args = "")
    {
        return await InvokeElementMethodAsync<T>($"{Document_GetElementById}('{id}')", methodName, args);
    }

    public async Task<T> InvokeElementMethodByNameAsync<T>(string name, string methodName, string args = "",
        int index = 0)
    {
        return await InvokeElementMethodAsync<T>($"{Document_GetElementsByName}('{name}')[{index}]", methodName, args);
    }

    public async Task<T> InvokeElementMethodByTagNameAsync<T>(string tagName, string methodName, string args = "",
        int index = 0)
    {
        return await InvokeElementMethodAsync<T>($"{Document_GetElementsByTagName}('{tagName}')[{index}]", methodName,
            args);
    }
}