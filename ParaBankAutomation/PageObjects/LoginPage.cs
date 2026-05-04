using ParaBankAutomation.Hooks;
using ParaBankAutomation.Pages;
using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public sealed class UserSetupSteps
    {
        private readonly IPage _page;
        private readonly LoginPage _loginPage;
        private readonly HomePage _homePage;
        private readonly AccountOverviewPage _accountOverviewPage;
        private readonly PageManager _pages;

        public UserSetupSteps(IPage page, PageManager pages)
        {
            _page = page;
            _pages = pages;
            _loginPage = _pages.LoginPage;
            _homePage = _pages.HomePage;
            _accountOverviewPage = _pages.AccountOverviewPage;
        }

        [Given(@"I am on the ParaBank website")]
        public async Task GivenIAmOnTheParaBankWebsite()
        {
            await _pages.NavigateToPage<LoginPage>();
        }

        [When(@"I enter a valid username '([^']*)' and password '([^']*)'")]
        public async Task WhenIEnterAValidUsernameAndPassword(string username, string password)
        {
            await _loginPage.EnterUsername(username);
            await _loginPage.EnterPassword(password);
        }

        [When(@"I click the Login button")]
        public async Task WhenIClickTheLoginButton()
        {
            await _loginPage.ClickLoginButton();
        }

        [Then(@"I should be logged in successfully")]
        public async Task ThenIShouldBeLoggedInSuccessfully()
        {
            await _homePage.WaitForPageLoad();
            Assert.IsTrue(await _homePage.IsLogoutButtonVisible(), "Login was not successful.");
        }

        [When(@"I enter an invalid username '([^']*)' and password '([^']*)'")]
        public async Task WhenIEnterAnInvalidUsernameAndPassword(string username, string password)
        {
            await _loginPage.EnterUsername(username);
            await _loginPage.EnterPassword(password);
        }

        [Then(@"I should see an error message '([^']*)'")]
        public async Task ThenIShouldSeeAnErrorMessage(string errorMessage)
        {
            string actualErrorMessage = await _loginPage.GetErrorMessage();
            Assert.AreEqual(errorMessage, actualErrorMessage, "Error message mismatch.");
        }

        [Then(@"I should be redirected to the Account Overview page")]
        public async Task ThenIShouldBeRedirectedToTheAccountOverviewPage()
        {
            await _accountOverviewPage.WaitForPageLoad();
            Assert.IsTrue(await _accountOverviewPage.IsAccountOverviewHeaderVisible(), "Not on Account Overview page.");
        }

        [When(@"I navigate to the Register page")]
        public async Task WhenINavigateToTheRegisterPage()
        {
            await _loginPage.ClickRegisterButton();
        }

        [Then(@"I should be on the Register page")]
        public async Task ThenIShouldBeOnTheRegisterPage()
        {
            await _pages.RegisterPage.WaitForPageLoad();
            Assert.IsTrue(await _pages.RegisterPage.IsRegisterFormVisible(), "Not on Register page.");
        }

        [When(@"I am on the ParaBank Home page")]
        public async Task WhenIAmOnTheParaBankHomePage()
        {
            await _pages.NavigateToPage<HomePage>();
            await _homePage.WaitForPageLoad();
        }

        [When(@"I log out of the application")]
        public async Task WhenILogOutOfTheApplication()
        {
            await _homePage.Logout();
            await _pages.NavigateToPage<HomePage>(); // Assuming HomePage has a navigation method
        }

        [Then(@"I should be on the login page")]
        public async Task ThenIShouldBeOnTheLoginPage()
        {
            await _loginPage.WaitForPageLoad();
            Assert.IsTrue(await _loginPage.IsLoginButtonVisible(), "Not on Login page after logout.");
        }
    }
}
