using Microsoft.Playwright;
using System.Threading.Tasks;

namespace ParaBankAutomation.PageObjects
{
    public class LoginPage : BasePage
    {
        // Locators
        private const string UsernameInput = "#username";
        private const string PasswordInput = "#password";
        private const string LoginButton = "[value='Log In']";
        private const string ErrorMessage = ".error";
        private const string RegisterLink = "text=Register";
        private const string LogoutLink = "text=Log Out"; // Added LogoutLink locator

        public LoginPage(IPage page, string baseUrl) : base(page, baseUrl) { }

        public async Task NavigateToLoginPage()
        {
            await NavigateAsync("/index.htm");
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
            await ClickElement(LogoutLink);
        }

        public async Task<string> GetErrorMessage()
        {
            await WaitForSelector(ErrorMessage);
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