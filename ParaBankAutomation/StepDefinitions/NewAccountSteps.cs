using Microsoft.Playwright;
using FluentAssertions;
using ParaBankAutomation.PageObjects;
using ParaBankAutomation.Utilities;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class NewAccountSteps
    {
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private readonly OpenAccountPage _openAccountPage;
        private readonly AccountOverviewPage _accountOverviewPage;

        public NewAccountSteps(IPage page, TestConfiguration config)
        {
            _page = page;
            _config = config;
            _openAccountPage = new OpenAccountPage(page, _config.BaseUrl);
            _accountOverviewPage = new AccountOverviewPage(page, _config.BaseUrl);
        }

        [When(@"I navigate to the open new account page")]
        public async Task WhenINavigateToTheOpenNewAccountPage()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/openaccount.htm");
        }

        [When(@"I select ""(.*)"" as account type")]
        public async Task WhenISelectAsAccountType(string accountType)
        {
            await _openAccountPage.SelectAccountType(accountType);
        }

        [When(@"I select an existing account to transfer initial funds from")]
        public async Task WhenISelectAnExistingAccountToTransferInitialFundsFrom()
        {
            await _openAccountPage.SelectFromAccount(0);
        }

        [When(@"I click the open new account button")]
        public async Task WhenIClickTheOpenNewAccountButton()
        {
            await _openAccountPage.ClickOpenAccountButton();
        }

        [Then(@"I should see a confirmation message for the new account")]
        public async Task ThenIShouldSeeAConfirmationMessageForTheNewAccount()
        {
            var newAccountNumber = await _openAccountPage.GetNewAccountNumber();
            var isCreated = !string.IsNullOrEmpty(newAccountNumber);
            isCreated.Should().BeTrue("New account should be created successfully");
        }

        [Then(@"the new account number should be displayed")]
        public async Task ThenTheNewAccountNumberShouldBeDisplayed()
        {
            var newAccountNumber = await _openAccountPage.GetNewAccountNumber();
            newAccountNumber.Should().NotBeNullOrEmpty("New account number should be displayed");
        }

        [Then(@"the new account should appear in my accounts overview")]
        public async Task ThenTheNewAccountShouldAppearInMyAccountsOverview()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var accounts = await _accountOverviewPage.GetAllAccounts();
            accounts.Should().NotBeEmpty("Account list should contain the new account");
        }
    }
}