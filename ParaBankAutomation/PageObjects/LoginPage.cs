using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects
{
    public class LoginPage: BasePage
    {
        private ILocator UsernameInput => _page.Locator("[name='username']");
        private ILocator PasswordInput => _page.Locator("[name='password']");
        private ILocator LoginButton => _page.Locator("[value='Log In']");

        public LoginPage(IPage page) : base(page) { }

        public async Task Login(string username, string password)
        {
            await UsernameInput.FillAsync(username);
            await PasswordInput.FillAsync(password);
            await LoginButton.ClickAsync();
        }

        public async Task Logout()
        {
            await _page.Locator("text=Log Out").ClickAsync();
        }
    }
}