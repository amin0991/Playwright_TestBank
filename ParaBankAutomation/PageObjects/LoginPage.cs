using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects
{
    public class LoginPage : BasePage
    {
        // Locators
        const string USERNAME_INPUT = "#username";
        const string PASSWORD_INPUT = "#password";
        const string LOGIN_BUTTON = "input[value='Log In']";
        const string ERROR_MESSAGE = ".error";
        const string REGISTER_LINK = "p:has-text('Register') a";
        const string LOGOUT_LINK = "a[href*='logout.htm']"; // Locator for the logout link

        public LoginPage(IPage page) : base(page) { }

        public async Task NavigateToLoginPage()
        {
            await Page.GotoAsync("/parabank/index.htm");
        }

        public async Task Login(string username, string password)
        {
            await Page.FillAsync(USERNAME_INPUT, username);
            await Page.FillAsync(PASSWORD_INPUT, password);
            await Page.ClickAsync(LOGIN_BUTTON);
        }

        public async Task<string> GetErrorMessage()
        {
            return await Page.TextContentAsync(ERROR_MESSAGE);
        }

        public async Task<bool> IsLoginFormVisible()
        {
            return await Page.IsVisibleAsync(USERNAME_INPUT) && await Page.IsVisibleAsync(PASSWORD_INPUT) && await Page.IsVisibleAsync(LOGIN_BUTTON);
        }

        public async Task ClickRegisterLink()
        {
            await Page.ClickAsync(REGISTER_LINK);
        }

        // Added to resolve the 'Logout' method not found error
        public async Task Logout()
        {
            await Page.ClickAsync(LOGOUT_LINK);
        }
    }
}
