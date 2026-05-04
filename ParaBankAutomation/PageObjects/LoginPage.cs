using ParaBankAutomation.Hooks;
using ParaBankAutomation.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace ParaBankAutomation.StepDefinitions
{
    [Binding]
    public class UserSetupSteps
    {
        private readonly LoginPage _loginPage;
        private readonly RegisterPage _registerPage;
        private readonly AccountServicesPage _accountServicesPage;
        private readonly ScenarioContext _scenarioContext;

        public UserSetupSteps(ScenarioContext scenarioContext, LoginPage loginPage, RegisterPage registerPage, AccountServicesPage accountServicesPage)
        {
            _scenarioContext = scenarioContext;
            _loginPage = loginPage;
            _registerPage = registerPage;
            _accountServicesPage = accountServicesPage;
        }

        [Given("the user is on the login page")]
        public async Task GivenTheUserIsOnTheLoginPage()
        {
            await _loginPage.GoToLoginPage();
        }

        [When("the user enters username {string} and password {string}")]
        public async Task WhenTheUserEntersUsernameAndPassword(string username, string password)
        {
            await _loginPage.EnterUsername(username);
            await _loginPage.EnterPassword(password);
        }

        [When("clicks the login button")]
        public async Task WhenClicksTheLoginButton()
        {
            await _loginPage.ClickLoginButton();
        }

        [Then("the user should be logged in successfully")]
        public async Task ThenTheUserShouldBeLoggedInSuccessfully()
        {
            await _accountServicesPage.AssertWelcomeMessageIsDisplayed();
        }

        [Given("the user is on the register page")]
        public async Task GivenTheUserIsOnTheRegisterPage()
        {
            await _registerPage.GoToRegisterPage();
        }

        [When("the user enters first name {string}, last name {string}, address {string}, city {string}, state {string}, zip code {string}, phone {string}, ssn {string}, username {string}, password {string}, and confirm password {string}")]
        public async Task WhenTheUserEntersRegistrationDetails(string firstName, string lastName, string address, string city, string state, string zipCode, string phone, string ssn, string username, string password, string confirmPassword)
        {
            await _registerPage.EnterFirstName(firstName);
            await _registerPage.EnterLastName(lastName);
            await _registerPage.EnterAddress(address);
            await _registerPage.EnterCity(city);
            await _registerPage.EnterState(state);
            await _registerPage.EnterZipCode(zipCode);
            await _registerPage.EnterPhone(phone);
            await _registerPage.EnterSsn(ssn);
            await _registerPage.EnterUsername(username);
            await _registerPage.EnterPassword(password);
            await _registerPage.EnterConfirmPassword(confirmPassword);
        }

        [When("clicks the register button")]
        public async Task WhenClicksTheRegisterButton()
        {
            await _registerPage.ClickRegisterButton();
        }

        [Then("the user should be registered successfully")]
        public async Task ThenTheUserShouldBeRegisteredSuccessfully()
        {
            await _registerPage.AssertRegistrationSuccessMessage();
        }

        [AfterScenario("Logout")]
        public async Task AfterScenarioLogout()
        {
            await _accountServicesPage.Logout();
        }
    }
}
