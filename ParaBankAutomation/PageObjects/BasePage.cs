using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects;

public class BasePage
{
    public readonly IPage Page;
    public readonly string BaseUrl;

    public BasePage(IPage page, string baseUrl)
    {
        Page = page;
        BaseUrl = baseUrl;
    }

    public async Task NavigateToUrl(string path = "")
    {
        var url = string.IsNullOrEmpty(path) ? BaseUrl : $"{BaseUrl}/{path}";
        await Page.GotoAsync(url);
    }

    public async Task ClickElement(string selector)
    {
        await Page.ClickAsync(selector);
    }

    public async Task FillInput(string selector, string text)
    {
        await Page.FillAsync(selector, text);
    }

    public async Task<string> GetText(string selector)
    {
        return await Page.TextContentAsync(selector) ?? string.Empty;
    }

    public async Task<bool> IsVisible(string selector)
    {
        try
        {
            return await Page.IsVisibleAsync(selector);
        }
        catch
        {
            return false;
        }
    }

    public async Task WaitForSelector(string selector, int timeout = 10000)
    {
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions 
        { 
            Timeout = timeout 
        });
    }

    public async Task<IReadOnlyList<ILocator>> GetElements(string selector)
    {
        return await Page.Locator(selector).AllAsync();
    }

    public async Task SelectOption(string selector, string value)
    {
        await Page.SelectOptionAsync(selector, value);
    }

    public async Task<string> GetCurrentUrl()
    {
        return Page.Url;
    }
}