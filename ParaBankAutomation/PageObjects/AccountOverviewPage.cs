using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects;

public class AccountOverviewPage : BasePage
{
    // Locators 
    private const string WelcomeMessage = ".smallText";
    private const string LogoutLink = "a[href*='logout.htm']";
    private const string AccountsTable = "#accountTable";
    private const string AccountRows = "#accountTable tbody tr";
    private const string AccountNumber = "td:nth-child(1) a";
    private const string Balance = "td:nth-child(2)";
    private const string AvailableAmount = "td:nth-child(3)";
    private const string TotalBalance = "#accountTable tfoot tr td:nth-child(2)";

    public AccountOverviewPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task<bool> IsAccountOverviewDisplayed()
    {
        try
        {
            await WaitForSelector(AccountsTable, 5000);
            return await IsVisible(AccountsTable);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetWelcomeMessage()
    {
        return await GetText(WelcomeMessage);
    }

    public async Task<bool> IsWelcomeMessageDisplayed(string expectedUsername)
    {
        var welcomeText = await GetWelcomeMessage();
        return welcomeText.Contains(expectedUsername, StringComparison.OrdinalIgnoreCase);
    }

    public async Task ClickLogout()
    {
        await ClickElement(LogoutLink);
    }

    public async Task<int> GetAccountCount()
    {
        var accounts = await GetElements(AccountRows);
        return accounts.Count;
    }

    public async Task<List<AccountInfo>> GetAllAccounts()
    {
        var accounts = new List<AccountInfo>();
        var rows = await GetElements(AccountRows);

        foreach (var row in rows)
        {
            var accountNumber = await row.Locator(AccountNumber).TextContentAsync() ?? string.Empty;
            var balance = await row.Locator(Balance).TextContentAsync() ?? string.Empty;
            var available = await row.Locator(AvailableAmount).TextContentAsync() ?? string.Empty;

            accounts.Add(new AccountInfo
            {
                AccountNumber = accountNumber,
                Balance = balance,
                AvailableAmount = available
            });
        }

        return accounts;
    }

    public async Task<string> GetFirstAccountNumber()
    {
        var accounts = await GetAllAccounts();
        return accounts.FirstOrDefault()?.AccountNumber ?? string.Empty;
    }

    public async Task ClickAccountNumber(string accountNumber)
    {
        await ClickElement($"a[href*='activity.htm?id={accountNumber}']");
    }
}

public class AccountInfo
{
    public string AccountNumber { get; set; } = string.Empty;
    public string Balance { get; set; } = string.Empty;
    public string AvailableAmount { get; set; } = string.Empty;
}