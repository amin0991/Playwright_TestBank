using ParaBankAutomation.Base;
using ParaBankAutomation.Pages;
using System;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class UserSetupSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly LoginPage _loginPage;
        private readonly OverviewPage _overviewPage;
        private readonly RegisterPage _registerPage;
        private readonly AccountsOverviewPage _accountsOverviewPage;
        private readonly OpenNewAccountPage _openNewAccountPage;
        private readonly RequestLoanPage _requestLoanPage;
        private readonly BillPayPage _billPayPage;
        private readonly FindTransactionsPage _findTransactionsPage;
        private readonly TransferFundsPage _transferFundsPage;
        private readonly UpdateContactInfoPage _updateContactInfoPage;

        public UserSetupSteps(ScenarioContext scenarioContext, LoginPage loginPage, OverviewPage overviewPage,
            RegisterPage registerPage, AccountsOverviewPage accountsOverviewPage, OpenNewAccountPage openNewAccountPage,
            RequestLoanPage requestLoanPage, BillPayPage billPayPage, FindTransactionsPage findTransactionsPage,
            TransferFundsPage transferFundsPage, UpdateContactInfoPage updateContactInfoPage)
        {
            _scenarioContext = scenarioContext;
            _loginPage = loginPage;
            _overviewPage = overviewPage;
            _registerPage = registerPage;
            _accountsOverviewPage = accountsOverviewPage;
            _openNewAccountPage = openNewAccountPage;
            _requestLoanPage = requestLoanPage;
            _billPayPage = billPayPage;
            _findTransactionsPage = findTransactionsPage;
            _transferFundsPage = transferFundsPage;
            _updateContactInfoPage = updateContactInfoPage;
        }


        [Given(@"I am on the login page")]
        public void GivenIAmOnTheLoginPage()
        {
            _loginPage.GoTo();
        }

        [Given(@"I am logged in as a new user with username ""(.*)"" and password ""(.*)""")]
        public void GivenIAmLoggedInAsANewUserWithUsernameAndPassword(string username, string password)
        {
            _registerPage.GoTo();
            _registerPage.FillFormAndRegister(username, password);
            _scenarioContext["username"] = username;
            _scenarioContext["password"] = password;
        }

        [Given(@"I am logged in with username ""(.*)"" and password ""(.*)""")]
        public void GivenIAmLoggedInWithUsernameAndPassword(string username, string password)
        {
            _loginPage.GoTo();
            _loginPage.Login(username, password);
            _scenarioContext["username"] = username;
            _scenarioContext["password"] = password;
        }

        [When(@"I register a new user with first name ""(.*)"", last name ""(.*)"", address ""(.*)"", city ""(.*)"", state ""(.*)"", zip code ""(.*)"", phone ""(.*)"", ssn ""(.*)"", username ""(.*)"" and password ""(.*)"")]
        public void WhenIRegisterANewUserWithFirstNameLastNameAddressCityStateZipCodePhoneSsnUsernameAndPassword(string firstName, string lastName, string address, string city, string state, string zipCode, string phone, string ssn, string username, string password)
        {
            _registerPage.FillFormAndRegister(firstName, lastName, address, city, state, zipCode, phone, ssn, username, password);
            _scenarioContext["username"] = username;
            _scenarioContext["password"] = password;
        }

        [When(@"I log in with username ""(.*)"" and password ""(.*)"")]
        public void WhenILogInWithUsernameAndPassword(string username, string password)
        {
            _loginPage.Login(username, password);
        }

        [Then(@"I should be logged out")]
        public void ThenIShouldBeLoggedOut()
        {
            _overviewPage.ClickLogoutLink();
            _loginPage.AssertPageIsVisible();
        }

        [Then(@"I should see a logout link")]
        public void ThenIShouldSeeALogoutLink()
        {
            _overviewPage.AssertLogoutLinkIsVisible();
        }

        [When(@"I click the logout link")]
        public void WhenIClickTheLogoutLink()
        {
            _overviewPage.ClickLogoutLink();
        }
    }
}