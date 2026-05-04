using ParaBankAutomation.Hooks;
using ParaBankAutomation.Pages;
using System;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class UserSetupSteps
    {
        private readonly LoginPage _loginPage;
        private readonly AccountOverviewPage _accountOverviewPage;
        private readonly CommonMethods _commonMethods;

        public UserSetupSteps(LoginPage loginPage, AccountOverviewPage accountOverviewPage, CommonMethods commonMethods)
        {
            _loginPage = loginPage;
            _accountOverviewPage = accountOverviewPage;
            _commonMethods = commonMethods;
        }

        [Given(@"I am on the login page")]
        public async Task GivenIAmOnTheLoginPage()
        {
            await _loginPage.GoToAsync();
        }

        [When(@"I login with valid credentials ""(.*)"" and ""(.*)""")]
        public async Task WhenILoginWithValidCredentialsAnd(string username, string password)
        {
            await _loginPage.Login(username, password);
        }

        [Then(@"I should be logged in successfully")]
        public async Task ThenIShouldBeLoggedInSuccessfully()
        {
            await _accountOverviewPage.IsPageLoaded();
        }

        [When(@"I login with invalid credentials ""(.*)"" and ""(.*)""")]
        public async Task WhenILoginWithInvalidCredentialsAnd(string username, string password)
        {
            await _loginPage.Login(username, password);
        }

        [Then(@"I should see an error message ""(.*)""")]
        public async Task ThenIShouldSeeAnErrorMessage(string errorMessage)
        {
            await _loginPage.IsErrorMessageDisplayed(errorMessage);
        }

        [Given(@"I am logged in as ""(.*)"" with password ""(.*)""")]
        public async Task GivenIAmLoggedInAsWithPassword(string username, string password)
        {
            await _loginPage.GoToAsync();
            await _loginPage.Login(username, password);
            await _accountOverviewPage.IsPageLoaded();
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("LoggedIn"))
            {
                await _accountOverviewPage.Logout();
            }
        }
    }
}
