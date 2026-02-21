using Microsoft.Playwright;
using FluentAssertions;
using ParaBankAutomation.PageObjects;
using ParaBankAutomation.Utilities;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class LoginSteps
    {
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private readonly LoginPage _loginPage;

        public LoginSteps(IPage page, TestConfiguration config)
        {
            _page = page;
            _config = config;
            _loginPage = new LoginPage(page, _config.BaseUrl);
        }

        [Given(@"I am on the ParaBank home page")]
        public async Task GivenIAmOnTheParaBankHomePage()
        {
            await _loginPage.NavigateToLoginPage();
        }

        [Given(@"I am logged in as a valid user")]
        public async Task GivenIAmLoggedInAsAValidUser()
        {
            await _loginPage.NavigateToLoginPage();
            await _loginPage.Login("john", "demo");
            await ThenIShouldSeeTheAccountOverviewPage();
        }

        [When(@"I login with valid credentials")]
        public async Task WhenILoginWithValidCredentials()
        {
            await _loginPage.Login("john", "demo");
        }

        [When(@"I login with username ""(.*)"" and password ""(.*)""")]
        public async Task WhenILoginWithUsernameAndPassword(string username, string password)
        {
            await _loginPage.Login(username, password);
        }

        [When(@"I click on the logout link")]
        public async Task WhenIClickOnTheLogoutLink()
        {
            await _page.ClickAsync("a[href='logout.htm']");
        }

        [Then(@"I should see the account overview page")]
        public async Task ThenIShouldSeeTheAccountOverviewPage()
        {
            /*
            await _page.WaitForSelectorAsync("h1:has-text('Accounts Overview')", new PageWaitForSelectorOptions { Timeout = 5000 });
            var isDisplayed = await _page.IsVisibleAsync("h1:has-text('Accounts Overview')");
            isDisplayed.Should().BeTrue("Account overview page should be displayed after successful login");
        */
         // Wait for URL to change to overview page
            await _page.WaitForURLAsync("**/overview.htm", new PageWaitForURLOptions { Timeout = 10000 });
            
            // Verify we're on the overview page
            var currentUrl = _page.Url;
            currentUrl.Should().Contain("overview.htm", "Should be on account overview page after login");
        }

        [Then(@"I should see a welcome message with the username")]
        public async Task ThenIShouldSeeAWelcomeMessageWithTheUsername()
        {
            var welcomeSelector = "p.smallText";
            var welcomeMessage = await _page.TextContentAsync(welcomeSelector);
            welcomeMessage.Should().Contain("Welcome", "Welcome message should be displayed");
        }

        [Then(@"I should see an error message ""(.*)""")]
        public async Task ThenIShouldSeeAnErrorMessage(string expectedError)
        {
            var errorMessage = await _loginPage.GetErrorMessage();
            errorMessage.Should().Contain(expectedError, "Error message should be displayed for invalid credentials");
        }

        [Then(@"I should remain on the login page")]
        public async Task ThenIShouldRemainOnTheLoginPage()
        {
            /*
            var currentUrl = _page.Url;
            currentUrl.Should().Contain("index.htm", "User should remain on the login page after failed login");
            */
             var currentUrl = _page.Url;
              currentUrl.Should().Match(url => url.Contains("index.htm") || url.Contains("login.htm"),"User should remain on the login page after failed login");
        
        }

        [Then(@"I should be redirected to the login page")]
        public async Task ThenIShouldBeRedirectedToTheLoginPage()
        {
            var currentUrl = _page.Url;
            currentUrl.Should().Contain("index.htm", "User should be redirected to login page after logout");
        }

        [Then(@"I should see the login form")]
        public async Task ThenIShouldSeeTheLoginForm()
        {
            var isLoginFormVisible = await _page.IsVisibleAsync("input[name='username']");
            isLoginFormVisible.Should().BeTrue("Login form should be visible");
        }
    }
}