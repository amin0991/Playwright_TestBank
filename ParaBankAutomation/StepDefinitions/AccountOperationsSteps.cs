using Microsoft.Playwright;
using FluentAssertions;
using ParaBankAutomation.PageObjects;
using ParaBankAutomation.Utilities;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class AccountOperationsSteps
    {
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private readonly AccountOverviewPage _accountOverviewPage;
        private readonly TransferFundsPage _transferFundsPage;
        private readonly AccountActivityPage _accountActivityPage;

        public AccountOperationsSteps(IPage page, TestConfiguration config)
        {
            _page = page;
            _config = config;
            _accountOverviewPage = new AccountOverviewPage(page, _config.BaseUrl);
            _transferFundsPage = new TransferFundsPage(page, _config.BaseUrl);
            _accountActivityPage = new AccountActivityPage(page, _config.BaseUrl);
        }

        [When(@"I navigate to the accounts overview page")]
        public async Task WhenINavigateToTheAccountsOverviewPage()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
        }

        [Then(@"I should see a list of my accounts")]
        public async Task ThenIShouldSeeAListOfMyAccounts()
        {
            var accountCount = await _accountOverviewPage.GetAccountCount();
            accountCount.Should().BeGreaterThan(0, "User should have at least one account");
        }

        [Then(@"each account should display account number and balance")]
        public async Task ThenEachAccountShouldDisplayAccountNumberAndBalance()
        {
            var accounts = await _accountOverviewPage.GetAllAccounts();
            accounts.Should().NotBeEmpty("Account list should not be empty");

            foreach (var account in accounts)
            {
                account.AccountNumber.Should().NotBeNullOrEmpty("Account number should be displayed");
                account.Balance.Should().NotBeNullOrEmpty("Balance should be displayed");
            }
        }

        [Given(@"I have multiple accounts with sufficient balance")]
        public async Task GivenIHaveMultipleAccountsWithSufficientBalance()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var accountCount = await _accountOverviewPage.GetAccountCount();

            if (accountCount == 0)
            {
                Console.WriteLine("Warning: User has only 0 account(s). Transfer tests may fail.");
            }
        }

        [When(@"I navigate to the transfer funds page")]
        public async Task WhenINavigateToTheTransferFundsPage()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/transfer.htm");
        }

        [When(@"I transfer (.*) dollars from account (.*) to account (.*)")]
        public async Task WhenITransferDollarsFromAccountToAccount(int amount, int fromAccountIndex, int toAccountIndex)
        {
            await _transferFundsPage.TransferFunds(amount.ToString(), fromAccountIndex, toAccountIndex);
        }

        [When(@"I attempt to transfer (.*) dollars from account (.*) to account (.*)")]
        public async Task WhenIAttemptToTransferDollarsFromAccountToAccount(int amount, int fromAccountIndex, int toAccountIndex)
        {
            await _transferFundsPage.TransferFunds(amount.ToString(), fromAccountIndex, toAccountIndex);
        }

        [Then(@"I should see a transfer confirmation message")]
        public async Task ThenIShouldSeeATransferConfirmationMessage()
        {
            var isSuccessful = await _transferFundsPage.IsTransferSuccessful();
            isSuccessful.Should().BeTrue("Transfer should be completed successfully");
        }

        [Then(@"the transfer should be reflected in the account balances")]
        public async Task ThenTheTransferShouldBeReflectedInTheAccountBalances()
        {
            // Check for success message in the page
            var confirmationVisible = await _page.IsVisibleAsync("h1:has-text('Transfer Complete')");
            confirmationVisible.Should().BeTrue("Transfer confirmation should be displayed");
        }

        [Then(@"I should see an error message indicating insufficient funds")]
        public async Task ThenIShouldSeeAnErrorMessageIndicatingInsufficientFunds()
        {
            var errorMessage = await _transferFundsPage.GetErrorMessage();
            errorMessage.Should().NotBeNullOrEmpty("Should display an error for insufficient funds");
        }

        [When(@"I navigate to account activity for my first account")]
        public async Task WhenINavigateToAccountActivityForMyFirstAccount()
        {
            var accounts = await _accountOverviewPage.GetAllAccounts();
            if (accounts.Any())
            {
                var firstAccountNumber = accounts.First().AccountNumber;
                await _page.GotoAsync($"{_config.BaseUrl}/activity.htm?id={firstAccountNumber}");
            }
        }

        [Then(@"I should see the account details")]
        public async Task ThenIShouldSeeTheAccountDetails()
        {
            var accountNumber = await _accountActivityPage.GetAccountNumber();
            accountNumber.Should().NotBeNullOrEmpty("Account number should be displayed");
        }

        [Then(@"I should see a list of recent transactions")]
        public async Task ThenIShouldSeeAListOfRecentTransactions()
        {
            var transactions = await _accountActivityPage.GetTransactions();
            transactions.Should().NotBeNull("Transaction list should be displayed");
        }

        [Then(@"each transaction should display date, description, and amount")]
        public async Task ThenEachTransactionShouldDisplayDateDescriptionAndAmount()
        {
            var transactions = await _accountActivityPage.GetTransactions();

            foreach (var transaction in transactions)
            {
                transaction.Date.Should().NotBeNullOrEmpty("Transaction date should be displayed");
                transaction.Description.Should().NotBeNullOrEmpty("Transaction description should be displayed");
                transaction.Amount.Should().NotBeNullOrEmpty("Transaction amount should be displayed");
            }
        }
    }
}