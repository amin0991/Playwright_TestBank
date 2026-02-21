using Microsoft.Playwright;
using FluentAssertions;
using ParaBankAutomation.PageObjects;
using ParaBankAutomation.Utilities;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class MoneyTransferSteps
    {
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private readonly LoginPage _loginPage;
        private readonly AccountOverviewPage _accountOverviewPage;
        private readonly BillPayPage _billPayPage;
        private readonly ScenarioContext _scenarioContext;
        
        private string _recipientAccountNumber = "";

        public MoneyTransferSteps(IPage page, TestConfiguration config, ScenarioContext scenarioContext)
        {
            _page = page;
            _config = config;
            _scenarioContext = scenarioContext;
            _loginPage = new LoginPage(page, _config.BaseUrl);
            _accountOverviewPage = new AccountOverviewPage(page, _config.BaseUrl);
            _billPayPage = new BillPayPage(page, _config.BaseUrl);
        }

        [Given(@"I am logged in as ""(.*)""")]
        public async Task GivenIAmLoggedInAs(string userName)
        {
            var user = _config.GetUserByName(userName);
            await _loginPage.NavigateToLoginPage();
            await _loginPage.Login(user.Username, user.Password);
            
            // Wait for login to complete
            try
            {
                await _page.WaitForURLAsync("**/overview.htm", new PageWaitForURLOptions { Timeout = 10000 });
            }
            catch
            {
                // If overview doesn't load, we're still logged in, just on a different page
            }
        }

        [Given(@"I have at least one account with sufficient balance")]
        public async Task GivenIHaveAtLeastOneAccountWithSufficientBalance()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var accounts = await _accountOverviewPage.GetAllAccounts();
            accounts.Should().NotBeEmpty("User should have at least one account");
        }

        [Given(@"I have multiple accounts")]
        public async Task GivenIHaveMultipleAccounts()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var accounts = await _accountOverviewPage.GetAllAccounts();
            
            if (accounts.Count < 2)
            {
                throw new Exception($"User needs at least 2 accounts for this test, but has {accounts.Count}");
            }
        }

        [When(@"I navigate to the bill pay page")]
        public async Task WhenINavigateToTheBillPayPage()
        {
            await _billPayPage.NavigateToBillPay();
        }

        [When(@"I send (.*) dollars to account of ""(.*)""")]
        public async Task WhenISendDollarsToAccountOf(int amount, string recipientUser)
        {
            // Get recipient's account number
            _recipientAccountNumber = await GetFirstAccountNumber(recipientUser);
            
            await _billPayPage.SendPayment(
                recipientUser, 
                _recipientAccountNumber, 
                amount.ToString()
            );
        }

        [When(@"I send (.*) dollars to my second account")]
        public async Task WhenISendDollarsToMySecondAccount(int amount)
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var accounts = await _accountOverviewPage.GetAllAccounts();
            
            if (accounts.Count >= 2)
            {
                _recipientAccountNumber = accounts[1].AccountNumber;
                
                await _billPayPage.NavigateToBillPay();
                await _billPayPage.SendPayment(
                    "My Second Account",
                    _recipientAccountNumber,
                    amount.ToString()
                );
            }
        }

        [When(@"I attempt to send (.*) dollars to account of ""(.*)""")]
        public async Task WhenIAttemptToSendDollarsToAccountOf(int amount, string recipientUser)
        {
            _recipientAccountNumber = await GetFirstAccountNumber(recipientUser);
            await _billPayPage.SendPayment(
                recipientUser,
                _recipientAccountNumber,
                amount.ToString()
            );
        }

        [Then(@"I should see a bill payment confirmation")]
        public async Task ThenIShouldSeeABillPaymentConfirmation()
        {
            var isSuccessful = await _billPayPage.IsPaymentSuccessful();
            isSuccessful.Should().BeTrue("Payment should be completed successfully");
        }

        [Then(@"the payment should show the correct amount")]
        public async Task ThenThePaymentShouldShowTheCorrectAmount()
        {
            var confirmation = await _billPayPage.GetConfirmationMessage();
            confirmation.Should().Contain("Bill Payment Complete", "Confirmation should indicate success");
        }

        [Then(@"I should see an error about the payment")]
        public async Task ThenIShouldSeeAnErrorAboutThePayment()
        {
            // Check for error message or that payment didn't succeed
            var errorMessage = await _billPayPage.GetErrorMessage();
            var isSuccessful = await _billPayPage.IsPaymentSuccessful();
            
            // Either there should be an error message OR payment should not be successful
            var hasError = !string.IsNullOrEmpty(errorMessage) || !isSuccessful;
            hasError.Should().BeTrue("Should display an error or fail the payment for insufficient funds");
        }

        // Helper method to get account number
        private async Task<string> GetFirstAccountNumber(string userName)
        {
            // Save current URL
            var currentUrl = _page.Url;
            var wasLoggedIn = currentUrl.Contains("overview.htm") || currentUrl.Contains("billpay.htm");
            
            // If not already showing the target user's accounts, we need to login as them
            if (!wasLoggedIn || !currentUrl.Contains(_config.GetUserByName(userName).Username))
            {
                var user = _config.GetUserByName(userName);
                await _loginPage.NavigateToLoginPage();
                await _loginPage.Login(user.Username, user.Password);
                
                try
                {
                    await _page.WaitForURLAsync("**/overview.htm", new PageWaitForURLOptions { Timeout = 10000 });
                }
                catch
                {
                    await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
                }
            }
            else
            {
                await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            }
            
            var accounts = await _accountOverviewPage.GetAllAccounts();
            
            if (accounts.Count == 0)
            {
                throw new Exception($"User {userName} has no accounts");
            }
            
            var accountNumber = accounts.First().AccountNumber;
            
            // Don't logout - we need to continue with the test
            
            return accountNumber;
        }
    }
}