using Microsoft.Playwright;
using FluentAssertions;
using ParaBankAutomation.PageObjects;
using ParaBankAutomation.Utilities;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class UserSetupSteps
    {
        private readonly IPage _page;
        private readonly TestConfiguration _config;
        private readonly RegistrationPage _registrationPage;
        private readonly LoginPage _loginPage;
        private readonly OpenAccountPage _openAccountPage;
        private readonly AccountOverviewPage _accountOverviewPage;
        private readonly ScenarioContext _scenarioContext;

        public UserSetupSteps(IPage page, TestConfiguration config, ScenarioContext scenarioContext)
        {
            _page = page;
            _config = config;
            _scenarioContext = scenarioContext;
            _registrationPage = new RegistrationPage(page, _config.BaseUrl);
            _loginPage = new LoginPage(page, _config.BaseUrl);
            _openAccountPage = new OpenAccountPage(page, _config.BaseUrl);
            _accountOverviewPage = new AccountOverviewPage(page, _config.BaseUrl);
        }

        [Given(@"I am on the ParaBank registration page")]
        public async Task GivenIAmOnTheParaBankRegistrationPage()
        {
            await _registrationPage.NavigateToRegistration();
        }

        [When(@"I register a new user with username ""(.*)""")]
        public async Task WhenIRegisterANewUserWithUsername(string username)
        {
            var user = _config.GetUserByName(username);
            var userData = GetRegistrationData(username);

            await _registrationPage.RegisterUser(user.Username, user.Password, userData);
            
            // Store username for later steps
            _scenarioContext["CurrentUsername"] = user.Username;
            _scenarioContext["CurrentPassword"] = user.Password;
        }

        [Then(@"the user should be created successfully")]
        public async Task ThenTheUserShouldBeCreatedSuccessfully()
        {
            var isSuccess = await _registrationPage.IsRegistrationSuccessful();
            var userExists = await _registrationPage.UserAlreadyExists();

            if (userExists)
            {
                Console.WriteLine("User already exists - logging in instead");
                
                // Login with existing user
                var username = _scenarioContext.Get<string>("CurrentUsername");
                var password = _scenarioContext.Get<string>("CurrentPassword");
                
                await _loginPage.NavigateToLoginPage();
                await _loginPage.Login(username, password);
                
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
                isSuccess.Should().BeTrue("User registration should be successful");
            }
        }

        [Then(@"the user should have at least one account")]
        public async Task ThenTheUserShouldHaveAtLeastOneAccount()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var accounts = await _accountOverviewPage.GetAllAccounts();
            accounts.Should().NotBeEmpty("User should have at least one account after registration");
        }

        [When(@"I open a second account for the user")]
        public async Task WhenIOpenASecondAccountForTheUser()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var currentAccounts = await _accountOverviewPage.GetAllAccounts();

            if (currentAccounts.Count >= 2)
            {
                Console.WriteLine("User already has 2+ accounts - skipping account creation");
                return;
            }

            // Open a second account
            await _page.GotoAsync($"{_config.BaseUrl}/openaccount.htm");
            await _openAccountPage.SelectAccountType("SAVINGS");
            await _openAccountPage.SelectFromAccount(0);
            await _openAccountPage.ClickOpenAccountButton();
            
            // Wait for account creation
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        [Then(@"the user should have 2 accounts")]
        public async Task ThenTheUserShouldHave2Accounts()
        {
            await _page.GotoAsync($"{_config.BaseUrl}/overview.htm");
            var accounts = await _accountOverviewPage.GetAllAccounts();
            accounts.Should().HaveCountGreaterOrEqualTo(2, "User should have at least 2 accounts");
            
            // Logout after setup
            await _loginPage.Logout();
        }

        // Helper method to get registration data
        private UserRegistrationData GetRegistrationData(string username)
        {
            return username.ToLower() switch
            {
                "john" => new UserRegistrationData
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Address = "123 Main Street",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Phone = "555-1234",
                    SSN = "123-45-6789"
                },
                "highbrow90" => new UserRegistrationData
                {
                    FirstName = "High",
                    LastName = "Brow",
                    Address = "456 Oak Avenue",
                    City = "Boston",
                    State = "MA",
                    ZipCode = "02101",
                    Phone = "555-9876",
                    SSN = "987-65-4321"
                },
                _ => new UserRegistrationData
                {
                    FirstName = "Test",
                    LastName = "User",
                    Address = "789 Test St",
                    City = "Test City",
                    State = "TC",
                    ZipCode = "99999",
                    Phone = "555-0000",
                    SSN = "000-00-0000"
                }
            };
        }
    }
}