# ParaBank BDD Test Automation Framework

A comprehensive Behavior-Driven Development (BDD) test automation framework for ParaBank banking application using C#, SpecFlow, Playwright, and NUnit.

## Project Overview

This framework demonstrates professional-grade test automation with:
- **BDD approach** using SpecFlow and Gherkin syntax
- **Page Object Model** design pattern
- **Cross-browser testing** with Playwright
- **Automated reporting** with HTML test reports
- **CI/CD ready** with GitHub Actions workflow

##  Table of Contents

- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Test Execution](#test-execution)
- [Project Structure](#project-structure)
- [Test Scenarios](#test-scenarios)
- [Reporting](#reporting)
- [Assumptions and Limitations](#assumptions-and-limitations)
- [Troubleshooting](#troubleshooting)

##  Prerequisites

Before running the tests, ensure you have the following installed:

### Required Software
- **.NET 10.0 SDK** or later
  - Download: https://dotnet.microsoft.com/download
  - Verify: `dotnet --version`

- **Playwright browsers**
  - Auto-installed during first build

### Optional (for development)
- **Visual Studio 2022** or **VS Code**
- **Git** (for version control)

### System Requirements
- **OS:** Windows 10/11, macOS, or Linux
- **RAM:** Minimum 4GB (8GB recommended)
- **Disk Space:** ~500MB for dependencies

##  Setup Instructions

### 1. Clone or Extract the Project
```bash
# If using Git
git clone <repository-url>
cd ParaBankAutomation

# Or extract the ZIP file and navigate to the folder
cd ParaBankAutomation
```

### 2. Restore NuGet Packages
```bash
dotnet restore
```

### 3. Install Playwright Browsers
```bash
playwright install chromium
# Optional: Install other browsers
# playwright install firefox
# playwright install webkit
```

### 4. Verify Build
```bash
dotnet build
```

You should see: **Build succeeded**

### 5. Configuration (Optional)

Edit `appsettings.json` to customize:
```json
{
  "AppSettings": {
    "BaseUrl": "https://parabank.parasoft.com/parabank",
    "Browser": "chromium",           // chromium, firefox, or webkit
    "Headless": false,                // true for headless mode
    "Timeout": 30000,                 // 30 seconds
    "ScreenshotOnFailure": true
  }
}
```

## Test Execution

### Run All Tests
```bash
# Basic execution
dotnet test

# With HTML report
dotnet test --logger "html;LogFileName=TestReport.html"

# View HTML report
start TestResults/TestReport.html
```

### Run Specific Test Categories
```bash
# Run only login tests
dotnet test --filter "Category=Authentication"

# Run critical tests only
dotnet test --filter "Category=Critical"

# Run positive scenarios
dotnet test --filter "Category=Positive"

# Run negative scenarios
dotnet test --filter "Category=Negative"
```

### Run Tests in Different Browsers

Edit `appsettings.json` and change `"Browser"` value:
- `"chromium"` (default)
- `"firefox"`
- `"webkit"`

Then run: `dotnet test`

### Headless Mode

For CI/CD or faster execution, set `"Headless": true` in `appsettings.json`

##  Project Structure
```
ParaBankAutomation/
├── Features/                      # Gherkin feature files
│   ├── Login.feature             # User authentication scenarios
│   ├── AccountOperations.feature # Account management tests
│   ├── NewAccount.feature        # Account creation tests
│   └── MoneyTransfer.feature     # Money transfer scenarios
├── StepDefinitions/              # Step implementation
│   ├── LoginSteps.cs
│   ├── AccountOperationsSteps.cs
│   ├── NewAccountSteps.cs
│   └── MoneyTransferSteps.cs
├── PageObjects/                  # Page Object Model
│   ├── BasePage.cs              # Base page with common methods
│   ├── LoginPage.cs
│   ├── AccountOverviewPage.cs
│   ├── TransferFundsPage.cs
│   ├── AccountActivityPage.cs
│   ├── OpenAccountPage.cs
│   └── BillPayPage.cs
├── Hooks/                        # Test lifecycle hooks
│   └── TestHooks.cs             # Setup/teardown logic
├── Utilities/                    # Helper classes
│   └── TestConfiguration.cs     # Configuration management
├── screenshots/                  # Test failure screenshots
├── logs/                         # Execution logs
├── TestResults/                  # Test reports
├── appsettings.json             # Configuration file
├── specflow.json                # SpecFlow configuration
└── README.md                    # This file
```

##  Test Scenarios

### User Authentication (4 scenarios)
- ✅ Successful login with valid credentials
- ✅ Failed login with invalid credentials
- ✅ Login with different invalid combinations
- ✅ Successful logout

### Account Operations (4 scenarios)
- ✅ View account overview
- ✅ View account activity
- ✅ Transfer funds between accounts
- ✅ Validate insufficient funds transfer

### New Account Creation (2 scenarios)
- ✅ Open a new checking account
- ✅ Open a new savings account

### Money Transfer (3 scenarios)
- ✅ Send money from john to highbrow90
- ✅ Send money between own accounts
- ✅ Attempt to send with insufficient funds

**Total: 16 comprehensive test scenarios**

## 📊 Reporting

### HTML Test Reports

After running tests, view the HTML report:
```bash
start TestResults/TestReport.html
```

**Report includes:**
- Total tests, passed, failed, skipped
- Pass percentage
- Execution duration
- Detailed failure information
- Stack traces for debugging

### Screenshots

Failed test screenshots are saved in: `screenshots/`

**Naming format:** `BELHAJJI_Test_MMddyyyyHHmmss.png`

### Logs

Detailed execution logs: `logs/test-execution-YYYYMMDD.log`

## Assumptions and Limitations

### Test Data Requirements

**Important:** Tests require pre-configured user accounts on ParaBank:

#### Required Test Users

1. **Primary User: john**
   - Username: `john`
   - Password: `demo`
   - Required: **2 bank accounts** (checking + savings)

2. **Secondary User: highbrow90**
   - Username: `highbrow90`
   - Password: `demo`
   - Required: **2 bank accounts** (checking + savings)

#### How to Create Test Users

1. Go to: https://parabank.parasoft.com/parabank/register.htm
2. Register with username/password above
3. After registration, open a 2nd account via "Open New Account"
4. Logout and repeat for second user

**Without these accounts, 11 out of 16 tests will fail** (authentication tests will still pass)

### Known Limitations

1. **Test Data Dependency**
   - Tests require specific user accounts with pre-configured data
   - Account balances must be sufficient for transfer tests
   - ParaBank demo site may reset data periodically

2. **Network Dependency**
   - Tests require internet connection
   - ParaBank demo site must be available
   - Network timeouts may cause test failures

3. **Browser Compatibility**
   - Tested on: Chromium, Firefox, WebKit
   - Some features may behave differently across browsers

4. **Parallel Execution**
   - Tests can run in parallel but share the same test users
   - Concurrent modifications may cause race conditions

5. **Dynamic Test Data**
   - Account numbers are generated dynamically by ParaBank
   - Tests adapt to available accounts

### Environment Assumptions

- Tests assume ParaBank demo site is accessible
- Default configuration uses ParaBank production demo URL
- Tests run in **non-headless mode** by default (for demonstration)
- Screenshots are enabled for failed tests
- 30-second timeout for all operations

### Expected Test Results

**With properly configured test users:**
-  **13-14 tests passing** (out of 16)
-  **2-3 tests failing** (expected - different error messages for empty credentials)

**Without test users:**
-  **5 tests passing** (authentication tests only)
-  **11 tests failing** (account-related tests)

##  Troubleshooting

### Build Errors

**Issue:** `Package not found`
```bash
# Solution
dotnet restore
dotnet clean
dotnet build
```

**Issue:** `Playwright not found`
```bash
# Solution
playwright install chromium
```

### Test Failures

**Issue:** All account tests fail
```
Expected accountCount to be greater than 0 but found 0
```
**Solution:** Create test users with bank accounts (see Test Data Requirements)

**Issue:** Timeout errors
```
Timeout 30000ms exceeded
```
**Solution:** 
- Check internet connection
- Verify ParaBank site is accessible
- Increase timeout in `appsettings.json`

**Issue:** Login tests fail
```
The username and password could not be verified
```
**Solution:** Verify test users exist on ParaBank with correct credentials

### No Screenshots Generated

**Issue:** `screenshots/` folder is empty

**Solution:**
```bash
# Create folder manually
mkdir screenshots

# Verify ScreenshotOnFailure is true in appsettings.json
```

### Slow Test Execution

**Solution:**
- Enable headless mode: `"Headless": true`
- Reduce timeout: `"Timeout": 20000`
- Run specific test categories instead of all tests

##  Additional Resources

- **SpecFlow Documentation:** https://docs.specflow.org/
- **Playwright for .NET:** https://playwright.dev/dotnet/
- **ParaBank Demo:** https://parabank.parasoft.com/parabank/
- **BDD Best Practices:** https://cucumber.io/docs/bdd/
##  Remarks:
- for this test I used my own data, also because next week I may be busy for this reason I could not send my question about data to use also intial state of test that you wan, for any further question please do not hesitate, either by call or send them into my email.
##  Author

**Amin M Belhajji**
- Framework: ParaBank BDD Test Automation
- Technology Stack: C# | SpecFlow | Playwright | NUnit
- Date: 14/02/2026
