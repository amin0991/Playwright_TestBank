using Microsoft.Playwright;

namespace ParaBankAutomation.PageObjects
{
    public class BillPayPage : BasePage
    {
        private readonly string _baseUrl;

        // Locators
        private readonly string _payeeNameInput = "input[name='payee.name']";
        private readonly string _payeeAddressInput = "input[name='payee.address.street']";
        private readonly string _payeeCityInput = "input[name='payee.address.city']";
        private readonly string _payeeStateInput = "input[name='payee.address.state']";
        private readonly string _payeeZipInput = "input[name='payee.address.zipCode']";
        private readonly string _payeePhoneInput = "input[name='payee.phoneNumber']";
        private readonly string _payeeAccountInput = "input[name='payee.accountNumber']";
        private readonly string _verifyAccountInput = "input[name='verifyAccount']";
        private readonly string _amountInput = "input[name='amount']";
        private readonly string _fromAccountSelect = "select[name='fromAccountId']";
        private readonly string _sendPaymentButton = "input[value='Send Payment']";
        private readonly string _confirmationTitle = "h1.title";
        private readonly string _errorMessage = "p.error";

        public BillPayPage(IPage page, string baseUrl) : base(page, baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task NavigateToBillPay()
        {
            await Page.GotoAsync($"{_baseUrl}/billpay.htm");
        }

        public async Task SendPayment(string payeeName, string accountNumber, string amount, int fromAccountIndex = 0)
        {
            // Fill payee information
            await FillInput(_payeeNameInput, payeeName);
            await FillInput(_payeeAddressInput, "123 Test St");
            await FillInput(_payeeCityInput, "Test City");
            await FillInput(_payeeStateInput, "TS");
            await FillInput(_payeeZipInput, "12345");
            await FillInput(_payeePhoneInput, "555-1234");
            
            // Fill account details
            await FillInput(_payeeAccountInput, accountNumber);
            await FillInput(_verifyAccountInput, accountNumber);
            await FillInput(_amountInput, amount);

            // Select from account
            var options = await Page.Locator(_fromAccountSelect + " option").AllAsync();
            if (options.Count > fromAccountIndex)
            {
                var accountValue = await options[fromAccountIndex].GetAttributeAsync("value");
                if (!string.IsNullOrEmpty(accountValue))
                {
                    await SelectOption(_fromAccountSelect, accountValue);
                }
            }

            // Submit payment
            await ClickElement(_sendPaymentButton);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task<bool> IsPaymentSuccessful()
        {
            try
            {
                if (await IsVisible(_confirmationTitle))
                {
                    var title = await GetText(_confirmationTitle);
                    return title.Contains("Bill Payment Complete", StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetConfirmationMessage()
        {
            if (await IsVisible(_confirmationTitle))
            {
                return await GetText(_confirmationTitle);
            }
            return string.Empty;
        }

        public async Task<string> GetErrorMessage()
        {
            if (await IsVisible(_errorMessage))
            {
                return await GetText(_errorMessage);
            }
            return string.Empty;
        }
    }
}