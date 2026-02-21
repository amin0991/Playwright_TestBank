using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects;

public class OpenAccountPage : BasePage
{
    // Locators
    private const string AccountTypeDropdown = "#type";
    private const string FromAccountDropdown = "#fromAccountId";
    private const string OpenAccountButton = "input[value='Open New Account']";
    private const string ConfirmationMessage = "#openAccountResult h1";
    private const string NewAccountNumber = "#newAccountId";
    private const string ConfirmationText = "#openAccountResult p";

    public OpenAccountPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task NavigateToOpenAccount()
    {
        await ClickElement("a[href*='openaccount.htm']");
    }

    public async Task SelectAccountType(string accountType)
    {
        await SelectOption(AccountTypeDropdown, accountType);
    }

    public async Task SelectFromAccount(int accountIndex)
    {
        var accounts = await Page.Locator($"{FromAccountDropdown} option").AllAsync();
        if (accounts.Count > accountIndex)
        {
            var accountValue = await accounts[accountIndex].GetAttributeAsync("value");
            if (accountValue != null)
            {
                await SelectOption(FromAccountDropdown, accountValue);
            }
        }
    }

    public async Task ClickOpenAccountButton()
    {
        await ClickElement(OpenAccountButton);
    }

    public async Task OpenNewAccount(string accountType, int fromAccountIndex)
    {
        await SelectAccountType(accountType);
        await SelectFromAccount(fromAccountIndex);
        await ClickOpenAccountButton();
    }

    public async Task<string> GetConfirmationMessage()
    {
        try
        {
            await WaitForSelector(ConfirmationMessage, 5000);
            return await GetText(ConfirmationMessage);
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> GetNewAccountNumber()
    {
        try
        {
            await WaitForSelector(NewAccountNumber, 5000);
            return await GetText(NewAccountNumber);
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<bool> IsAccountCreatedSuccessfully()
    {
        var message = await GetConfirmationMessage();
        return message.Contains("Account Opened", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("Congratulations", StringComparison.OrdinalIgnoreCase);
    }
}