using Microsoft.Playwright;
using System.Threading.Tasks;
using ParaBankAutomation.PageObjects;

namespace ParaBankAutomation.PageObjects
{
    public class LoginPage : BasePage
    {
        public LoginPage(IPage page, string baseUrl) : base(page, baseUrl) { }

        const string UsernameInput = "input[name='username']";
        const string PasswordInput = "input[name='password']";
        const string LoginButton = "input[value='Log In']";
        const string ErrorMessage = ".error";
        const string RegisterLink = "a[href*='register.htm']";

        public async Task NavigateToLoginPage()
        {
            await NavigateToUrl("index.htm");
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
            await ClickElement("a[href='logout.htm']");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task<string> GetErrorMessage()
        {
            await WaitForSelector(ErrorMessage);
            return await GetText(ErrorMessage);
        }

        public async Task<bool> IsLoginFormVisible()
        {
            return await IsVisible(UsernameInput) && await IsVisible(PasswordInput);
        }

        public async Task ClickRegisterLink()
        {
            await ClickElement(RegisterLink);
        }
    }
}
