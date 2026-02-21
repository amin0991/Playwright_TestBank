using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects;

public class TransferFundsPage : BasePage
{
    // Locators
    private const string AmountInput = "#amount";
    private const string FromAccountDropdown = "#fromAccountId";
    private const string ToAccountDropdown = "#toAccountId";
    private const string TransferButton = "input[value='Transfer']";
    private const string ConfirmationMessage = "#showResult h1";
    private const string TransferCompleteMessage = "#showResult p";
    private const string ErrorMessage = ".error";

    public TransferFundsPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task NavigateToTransferFunds()
    {
        await ClickElement("a[href*='transfer.htm']");
    }

    public async Task EnterAmount(string amount)
    {
        await FillInput(AmountInput, amount);
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

    public async Task SelectToAccount(int accountIndex)
    {
        var accounts = await Page.Locator($"{ToAccountDropdown} option").AllAsync();
        if (accounts.Count > accountIndex)
        {
            var accountValue = await accounts[accountIndex].GetAttributeAsync("value");
            if (accountValue != null)
            {
                await SelectOption(ToAccountDropdown, accountValue);
            }
        }
    }

    public async Task ClickTransferButton()
    {
        await ClickElement(TransferButton);
    }

    public async Task TransferFunds(string amount, int fromAccountIndex, int toAccountIndex)
    {
        await EnterAmount(amount);
        await SelectFromAccount(fromAccountIndex);
        await SelectToAccount(toAccountIndex);
        await ClickTransferButton();
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

    public async Task<bool> IsTransferSuccessful()
    {
        var message = await GetConfirmationMessage();
        return message.Contains("Transfer Complete", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> GetErrorMessage()
    {
        try
        {
            await WaitForSelector(ErrorMessage, 3000);
            return await GetText(ErrorMessage);
        }
        catch
        {
            return string.Empty;
        }
    }
}