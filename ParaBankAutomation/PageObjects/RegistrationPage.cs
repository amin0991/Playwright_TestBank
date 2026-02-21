using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects
{
    public class RegistrationPage : BasePage
    {
        private readonly string _baseUrl;

        // Locators
        private readonly string _firstNameInput = "input[id='customer.firstName']";
        private readonly string _lastNameInput = "input[id='customer.lastName']";
        private readonly string _addressInput = "input[id='customer.address.street']";
        private readonly string _cityInput = "input[id='customer.address.city']";
        private readonly string _stateInput = "input[id='customer.address.state']";
        private readonly string _zipCodeInput = "input[id='customer.address.zipCode']";
        private readonly string _phoneInput = "input[id='customer.phoneNumber']";
        private readonly string _ssnInput = "input[id='customer.ssn']";
        private readonly string _usernameInput = "input[id='customer.username']";
        private readonly string _passwordInput = "input[id='customer.password']";
        private readonly string _confirmPasswordInput = "input[id='repeatedPassword']";
        private readonly string _registerButton = "input[value='Register']";
        private readonly string _welcomeMessage = "p:has-text('Welcome')";
        private readonly string _errorMessage = "span.error";

        public RegistrationPage(IPage page, string baseUrl) : base(page, baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task NavigateToRegistration()
        {
            await Page.GotoAsync($"{_baseUrl}/register.htm");
        }

        public async Task RegisterUser(string username, string password, UserRegistrationData userData)
        {
            await FillInput(_firstNameInput, userData.FirstName);
            await FillInput(_lastNameInput, userData.LastName);
            await FillInput(_addressInput, userData.Address);
            await FillInput(_cityInput, userData.City);
            await FillInput(_stateInput, userData.State);
            await FillInput(_zipCodeInput, userData.ZipCode);
            await FillInput(_phoneInput, userData.Phone);
            await FillInput(_ssnInput, userData.SSN);
            await FillInput(_usernameInput, username);
            await FillInput(_passwordInput, password);
            await FillInput(_confirmPasswordInput, password);
            
            await ClickElement(_registerButton);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task<bool> IsRegistrationSuccessful()
        {
            try
            {
                // Check for welcome message OR that we're on overview page
                var hasWelcome = await IsVisible(_welcomeMessage);
                var onOverview = Page.Url.Contains("overview.htm");
                return hasWelcome || onOverview;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UserAlreadyExists()
        {
            try
            {
                if (await IsVisible(_errorMessage))
                {
                    var error = await GetText(_errorMessage);
                    return error.Contains("already exists", StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public class UserRegistrationData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
    }
}