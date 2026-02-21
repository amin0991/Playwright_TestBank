using Microsoft.Extensions.Configuration;

namespace ParaBankAutomation.Utilities
{
    public class TestConfiguration
    {
        private readonly IConfiguration _configuration;

        public string BaseUrl { get; }
        public string Browser { get; }
        public bool Headless { get; }
        public int Timeout { get; }
        public bool ScreenshotOnFailure { get; }
        public bool VideoOnFailure { get; }
        public UserCredentials ValidUser { get; }
        public UserCredentials SecondUser { get; }

        public TestConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            BaseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://parabank.parasoft.com/parabank";
            Browser = _configuration["AppSettings:Browser"] ?? "chromium";
            Headless = bool.Parse(_configuration["AppSettings:Headless"] ?? "false");
            Timeout = int.Parse(_configuration["AppSettings:Timeout"] ?? "30000");
            ScreenshotOnFailure = bool.Parse(_configuration["AppSettings:ScreenshotOnFailure"] ?? "true");
            VideoOnFailure = bool.Parse(_configuration["AppSettings:VideoOnFailure"] ?? "false");

            ValidUser = new UserCredentials
            {
                Username = _configuration["TestUsers:ValidUser:Username"] ?? "john",
                Password = _configuration["TestUsers:ValidUser:Password"] ?? "demo",
                FirstName = _configuration["TestUsers:ValidUser:FirstName"] ?? "John",
                LastName = _configuration["TestUsers:ValidUser:LastName"] ?? "Smith"
            };

            SecondUser = new UserCredentials
            {
                Username = _configuration["TestUsers:SecondUser:Username"] ?? "highbrow90",
                Password = _configuration["TestUsers:SecondUser:Password"] ?? "demo",
                FirstName = _configuration["TestUsers:SecondUser:FirstName"] ?? "High",
                LastName = _configuration["TestUsers:SecondUser:LastName"] ?? "Brow"
            };
        }

        public UserCredentials GetUserByName(string userName)
        {
            return userName.ToLower() switch
            {
                "john" => ValidUser,
                "highbrow90" => SecondUser,
                _ => ValidUser
            };
        }
    }

    public class UserCredentials
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}