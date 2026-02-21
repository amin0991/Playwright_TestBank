using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects;

public class AccountActivityPage : BasePage
{
    // Locators
    private const string AccountNumber = "#accountId";
    private const string AccountType = "#accountType";
    private const string AccountBalance = "#balance";
    private const string TransactionsTable = "#transactionTable";
    private const string TransactionRows = "#transactionTable tbody tr";
    private const string NoTransactionsMessage = "#transactionTable tbody td";

    public AccountActivityPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }
    public async Task NavigateToAccountActivity()
    {
        await ClickElement("a[href*='activity.htm']");
    }
    public async Task<string> GetAccountNumber()
    {
        return await GetText(AccountNumber);
    }
    public async Task<string> GetAccountType()
    {
        return await GetText(AccountType);
    }
    public async Task<string> GetAccountBalance()
    {
        return await GetText(AccountBalance);
    }

    public async Task<bool> AreAccountDetailsDisplayed()
    {
        return await IsVisible(AccountNumber) && 
               await IsVisible(AccountType) && 
               await IsVisible(AccountBalance);
    }

    public async Task<List<TransactionInfo>> GetTransactions()
    {
        var transactions = new List<TransactionInfo>();
        
        try
        {
            await WaitForSelector(TransactionRows, 3000);
            var rows = await GetElements(TransactionRows);

            foreach (var row in rows)
            {
                var cells = await row.Locator("td").AllAsync();
                if (cells.Count >= 3)
                {
                    transactions.Add(new TransactionInfo
                    {
                        Date = await cells[0].TextContentAsync() ?? string.Empty,
                        Description = await cells[1].TextContentAsync() ?? string.Empty,
                        Amount = await cells[2].TextContentAsync() ?? string.Empty
                    });
                }
            }
        }
        catch
        {
        }

        return transactions;
    }
    public async Task<int> GetTransactionCount()
    {
        var transactions = await GetTransactions();
        return transactions.Count;
    }

    public async Task<bool> HasTransactions()
    {
        return await GetTransactionCount() > 0;
    }
}

public class TransactionInfo
{
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
}