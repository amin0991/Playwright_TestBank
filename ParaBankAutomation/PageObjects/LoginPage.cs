using Microsoft.Playwright;
using System.Threading.Tasks;

namespace ParaBankAutomation.PageObjects
{
    public class LoginPage : BasePage
    {
        // Locators for elements on the login page
        private const string UsernameInput = "input[name='username']";
        private const string PasswordInput = "input[name='password']";
        private const string LoginButton = "input[value='Log In']";
        private const string ErrorMessage = ".error";
        private const string RegisterLink = "#loginPanel p:nth-child(2) a";

        public LoginPage(IPage page, string baseUrl) : base(page, baseUrl)
        {
        }

        public async Task NavigateToLoginPage()
        {
            await Page.GotoAsync($"{_baseUrl}/index.htm");
            await WaitForSelector(LoginButton);
        }

        public async Task EnterUsername(string username)
        {
            await FillInput(UsernameInput, username);
        }

        public async Task EnterPassword(string password)
        {
            await FillInput(PasswordInput, password);
        }

        public async Task ClickLoginButton()
        {
            await ClickElement(LoginButton);
        }

        public async Task Login(string username, string password)
        {
            await EnterUsername(username);
            await EnterPassword(password);
            await ClickLoginButton();
        }

        public async Task Logout()
        {
            await Page.ClickAsync("text=Log Out");
        }

        public async Task<string> GetErrorMessage()
        {
            return await GetText(ErrorMessage);
        }

        public async Task<bool> IsLoginFormVisible()
        {
            return await IsVisible(LoginButton);
        }

        public async Task ClickRegisterLink()
        {
            await ClickElement(RegisterLink);
        }
    }
}