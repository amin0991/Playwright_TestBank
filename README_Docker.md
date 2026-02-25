# 🏦 ParaBank BDD Test Automation Framework

> **C# | SpecFlow | Playwright | NUnit | Jenkins CI/CD | Docker**

A professional-grade Behavior-Driven Development (BDD) test automation framework for the ParaBank banking application, featuring full CI/CD pipeline integration via Jenkins and Docker.

---

## 📋 Table of Contents

- [Project Overview](#-project-overview)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Local Setup](#-local-setup)
- [Test Execution](#-test-execution)
- [Project Structure](#-project-structure)
- [Test Scenarios](#-test-scenarios)
- [CI/CD Pipeline](#-cicd-pipeline)
- [Docker Setup](#-docker-setup)
- [Test Data Requirements](#-test-data-requirements)
- [Configuration](#-configuration)
- [Reporting](#-reporting)
- [Troubleshooting](#-troubleshooting)

---

## 🎯 Project Overview

This framework automates functional testing of the [ParaBank](https://parabank.parasoft.com/parabank) banking application using a BDD approach. Tests are written in Gherkin (plain English), making them readable by both technical and non-technical stakeholders.

**Key capabilities:**
- BDD scenarios with SpecFlow + Gherkin
- Page Object Model for maintainable selectors
- Two execution modes: **Local** and **Docker**
- Jenkins pipeline with automated GitHub triggers
- HTML test reports with screenshots on failure

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        DEVELOPER MACHINE                        │
│                                                                 │
│  ┌──────────────┐    push     ┌──────────────────────────────┐  │
│  │  VS Code /   │ ──────────► │   GitHub Repository          │  │
│  │  Visual      │             │   amin0991/Playwright_TestBank│  │
│  │  Studio      │             └──────────────┬───────────────┘  │
│  └──────────────┘                            │ webhook/poll      │
│                                              ▼                  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │                    DOCKER DESKTOP                         │  │
│  │                                                           │  │
│  │  ┌─────────────────────────┐  ┌────────────────────────┐ │  │
│  │  │  jenkins-parabank-docker│  │   parabank-app         │ │  │
│  │  │  Port: 8081             │  │   Port: 8090           │ │  │
│  │  │                         │  │   (parasoft/parabank)  │ │  │
│  │  │  Pipeline stages:       │  └────────────────────────┘ │  │
│  │  │  1. Checkout            │                              │  │
│  │  │  2. Install OS Deps     │  ┌────────────────────────┐ │  │
│  │  │  3. Restore NuGet       │  │   n8n                  │ │  │
│  │  │  4. Build Solution      │  │   Port: 5678           │ │  │
│  │  │  5. Install Playwright  │  │   (workflow automation)│ │  │
│  │  │  6. Execute Tests       │  └────────────────────────┘ │  │
│  │  │                         │                              │  │
│  │  │  IS_LOCAL = 'false'     │                              │  │
│  │  │  → runs in mcr/dotnet   │                              │  │
│  │  │    Docker agent         │                              │  │
│  │  └─────────────────────────┘                              │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### Design Patterns

```
Features (Gherkin)
      │
      ▼
Step Definitions  ──►  Page Objects  ──►  Playwright Browser
      │                    │
      │                    └──►  BasePage (common methods)
      │
      ▼
   Hooks (Setup/Teardown)
      │
      ▼
TestConfiguration (appsettings.json)
```

| Layer | Responsibility |
|-------|---------------|
| **Features** | Human-readable test scenarios in Gherkin |
| **Step Definitions** | C# bindings that map Gherkin steps to code |
| **Page Objects** | Encapsulate page selectors and interactions |
| **BasePage** | Shared Playwright methods (click, fill, wait) |
| **Hooks** | Browser lifecycle: open/close per scenario |
| **TestConfiguration** | Centralized config from appsettings.json |

---

## 🛠️ Tech Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| C# / .NET | 10.0 | Programming language & runtime |
| SpecFlow | Latest | BDD framework (Gherkin → C#) |
| Playwright | 1.2.3 | Browser automation |
| NUnit | 3.x | Test runner |
| FluentAssertions | Latest | Readable assertions |
| Jenkins | LTS | CI/CD pipeline |
| Docker | Latest | Container orchestration |
| n8n | Latest | Workflow automation |

---

## ✅ Prerequisites

### For Local Execution

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) — verify: `dotnet --version`
- [Git](https://git-scm.com/) — verify: `git --version`
- Visual Studio 2022 or VS Code (recommended)

### For Docker / Jenkins Mode

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — verify: `docker --version`
- Docker containers running (see [Docker Setup](#-docker-setup))

---

## 💻 Local Setup

### 1. Clone the repository

```powershell
git clone https://github.com/amin0991/Playwright_TestBank.git
cd Playwright_TestBank
```

### 2. Restore NuGet packages

```powershell
dotnet restore
```

### 3. Build the solution

```powershell
dotnet build
```

### 4. Install Playwright browsers

```powershell
# Navigate to the output folder
cd ParaBankAutomation/bin/Release/net10.0

# Install Chromium (required)
dotnet tool install --global Microsoft.Playwright.CLI
playwright install chromium

# Optional: install other browsers
# playwright install firefox
# playwright install webkit
```

### 5. Set up test users on ParaBank

> ⚠️ **Critical step** — tests will fail without this!

**Option A — Use local ParaBank (recommended):**

```powershell
docker run -d --name parabank-app -p 8090:8080 parasoft/parabank
```

Then update `appsettings.json`:
```json
"BaseUrl": "http://localhost:8090/parabank"
```

**Option B — Use the public demo site:**

Go to https://parabank.parasoft.com/parabank/register.htm and create accounts manually (see [Test Data Requirements](#-test-data-requirements)).

### 6. Run the tests

```powershell
cd C:\path\to\Playwright_TestBank
dotnet test ParaBankAutomation
```

---

## ▶️ Test Execution

### Run all tests

```powershell
dotnet test
```

### Run with HTML report

```powershell
dotnet test --logger "html;LogFileName=TestReport.html"
```

### Run by tag/category

```powershell
# Smoke tests only
dotnet test --filter "Category=smoke"

# Regression suite
dotnet test --filter "Category=regression"

# Login tests only
dotnet test --filter "FullyQualifiedName~Login"
```

### Run in headless mode

Edit `appsettings.json`:
```json
"Headless": true
```

---

## 📁 Project Structure

```
Playwright_TestBank/
├── ParaBankAutomation/
│   ├── Features/                        # Gherkin test scenarios
│   │   ├── Login.feature                # Authentication tests
│   │   ├── AccountOperations.feature    # Account management tests
│   │   ├── NewAccount.feature           # Account creation tests
│   │   ├── MoneyTransfer.feature        # Transfer & bill pay tests
│   │   └── UserSetup.feature            # Test data setup
│   │
│   ├── StepDefinitions/                 # Gherkin step implementations
│   │   ├── LoginSteps.cs
│   │   ├── AccountOperationsSteps.cs
│   │   ├── NewAccountSteps.cs
│   │   ├── MoneyTransferSteps.cs
│   │   └── UserSetupSteps.cs
│   │
│   ├── PageObjects/                     # Page Object Model
│   │   ├── BasePage.cs                  # Common Playwright methods
│   │   ├── LoginPage.cs
│   │   ├── AccountOverviewPage.cs
│   │   ├── AccountActivityPage.cs
│   │   ├── TransferFundsPage.cs
│   │   ├── OpenAccountPage.cs
│   │   └── BillPayPage.cs
│   │
│   ├── Hooks/
│   │   └── TestHooks.cs                 # Browser setup/teardown per scenario
│   │
│   ├── Utilities/
│   │   └── TestConfiguration.cs         # Reads appsettings.json
│   │
│   ├── screenshots/                     # Auto-saved on test failure
│   ├── TestResults/                     # HTML reports output
│   ├── appsettings.json                 # ⚙️ Main configuration file
│   └── specflow.json                    # SpecFlow settings
│
├── Jenkinsfile                          # CI/CD pipeline definition
├── Dockerfile                           # For local Docker builds
├── Dockerfile.jenkins                   # Jenkins-specific Docker image
└── ParaBankAutomation.slnx             # Solution file
```

---

## 🧪 Test Scenarios

### 🔐 User Authentication — `Login.feature`

| Scenario | Tags | Status |
|----------|------|--------|
| Successful login with valid credentials | @smoke | ✅ |
| Successful logout | @smoke | ✅ |
| Failed login with invalid credentials | @regression | ⚠️ Depends on ParaBank state |
| Login with empty username | @regression | ✅ |
| Login with empty password | @regression | ✅ |
| Login with wrong password for valid user | @regression | ⚠️ Depends on ParaBank state |

### 💰 Account Operations — `AccountOperations.feature`

| Scenario | Tags | Status |
|----------|------|--------|
| View accounts overview | @regression | ⚠️ Needs accounts |
| View account activity | @regression | ⚠️ Needs accounts |
| Transfer funds between accounts | @regression | ⚠️ Needs 2 accounts |
| Validate insufficient funds transfer | @regression | ⚠️ Needs 2 accounts |

### 🏦 New Account Creation — `NewAccount.feature`

| Scenario | Tags | Status |
|----------|------|--------|
| Open a new savings account | @smoke @regression | ⚠️ Needs existing account |
| Open a new checking account | @regression | ⚠️ Needs existing account |

### 💸 Money Transfer — `MoneyTransfer.feature`

| Scenario | Tags | Status |
|----------|------|--------|
| Send money from john to highbrow90 | @regression | ⚠️ Needs accounts |
| Send money between own accounts | @regression | ⚠️ Needs 2 accounts |
| Attempt to send with insufficient funds | @regression | ⚠️ Needs accounts |

**Total: 18 test scenarios** | ✅ Always pass | ⚠️ Require test data setup

---

## 🚀 CI/CD Pipeline

### Pipeline Overview (Jenkinsfile)

```groovy
IS_LOCAL = 'true'   // Local .NET execution
IS_LOCAL = 'false'  // Docker container execution (recommended for CI)
```

### Pipeline Stages

```
Stage 1: Checkout
    └── git clone from GitHub (main branch)
    └── rm -rf bin obj  ← prevents permission issues

Stage 2: Install OS Dependencies (Docker mode only)
    └── apt-get install: libnss3, libgbm1, libpango, libasound2 ...

Stage 3: Restore NuGet Packages
    └── dotnet restore

Stage 4: Build Solution
    └── dotnet build --configuration Release

Stage 5: Install Playwright Browsers
    └── dotnet tool install Microsoft.Playwright.CLI
    └── playwright install chromium

Stage 6: Execute Tests
    └── dotnet test
    └── Generates TestResults/
```

### Jenkins Access

| Instance | URL | Purpose |
|----------|-----|---------|
| Original Jenkins | http://localhost:8080 | Stable reference |
| Docker Jenkins | http://localhost:8081 | Active CI/CD with Docker socket |

### Triggering a Build

**Manual:** Jenkins → DXC-Playwright-CI-Docker → **Build Now**

**Automatic via n8n:** http://localhost:5678 — workflow triggers Jenkins on GitHub push

---

## 🐳 Docker Setup

### Start all services

```powershell
# 1. ParaBank application
docker run -d --name parabank-app -p 8090:8080 parasoft/parabank

# 2. Jenkins with Docker socket access
docker run -d `
  --name jenkins-parabank-docker `
  -p 8081:8080 `
  -p 50001:50000 `
  -v jenkins_docker_home:/var/jenkins_home `
  -v //var/run/docker.sock:/var/run/docker.sock `
  --group-add 0 `
  amin90/jenkins-parabank

# 3. n8n workflow automation
docker run -d --name n8n -p 5678:5678 n8nio/n8n
```

### Verify all containers are running

```powershell
docker ps
```

Expected output:
```
CONTAINER ID   IMAGE                      PORTS
xxxxxxxxxxxx   parasoft/parabank          0.0.0.0:8090->8080/tcp
xxxxxxxxxxxx   amin90/jenkins-parabank    0.0.0.0:8081->8080/tcp
xxxxxxxxxxxx   n8nio/n8n                  0.0.0.0:5678->3000/tcp
```

### Fix Docker socket permissions (if pipeline fails)

```powershell
docker exec -it --user root jenkins-parabank-docker bash -c "chmod 666 /var/run/docker.sock"
```

### Container URLs

| Service | URL | Credentials |
|---------|-----|-------------|
| ParaBank App | http://localhost:8090/parabank | See test users below |
| Jenkins | http://localhost:8081 | admin / (see Jenkins setup) |
| n8n | http://localhost:5678 | — |

---

## 👤 Test Data Requirements

Tests require pre-configured users in ParaBank before execution.

### Required Users

#### User 1 — john (Primary Test User)
```
Username : john
Password : demo
Required : minimum 2 bank accounts (checking + savings)
```

#### User 2 — highbrow90 (Secondary Test User)
```
Username : highbrow90
Password : demo
Required : minimum 1 bank account
```

### How to Create Test Users

1. Go to: http://localhost:8090/parabank/register.htm
2. Fill in registration form with the credentials above
3. After login, go to **Open New Account** → create a Checking account
4. Go to **Open New Account** again → create a Savings account
5. Logout and repeat for `highbrow90`

> ⚠️ If using **parabank.parasoft.com** (public demo), data may reset periodically — always recreate accounts before running the full suite.

### Impact of Missing Test Data

| Missing | Tests Affected | Error |
|---------|---------------|-------|
| No accounts for `john` | 7 tests | "User has 0 accounts" |
| Only 1 account for `john` | 3 tests | "User needs at least 2 accounts" |
| `highbrow90` missing | 1 test | Collection is empty |

---

## ⚙️ Configuration

Edit `ParaBankAutomation/appsettings.json`:

```json
{
  "AppSettings": {
    "BaseUrl": "http://localhost:8090/parabank",  // ← Local Docker
    // "BaseUrl": "https://parabank.parasoft.com/parabank",  // ← Public demo
    // "BaseUrl": "http://host.docker.internal:8090/parabank",  // ← From inside Jenkins container
    "Browser": "chromium",          // chromium | firefox | webkit
    "Headless": true,               // false = visible browser (useful for debugging)
    "Timeout": 30000,               // milliseconds
    "ScreenshotOnFailure": true,
    "VideoOnFailure": false
  },
  "TestUsers": {
    "ValidUser": {
      "Username": "john",
      "Password": "demo"
    },
    "SecondUser": {
      "Username": "highbrow90",
      "Password": "demo"
    }
  }
}
```

### BaseUrl Guide

| Scenario | BaseUrl value |
|----------|--------------|
| Running tests locally on your machine | `http://localhost:8090/parabank` |
| Running tests inside Jenkins Docker container | `http://host.docker.internal:8090/parabank` |
| Using the public Parasoft demo server | `https://parabank.parasoft.com/parabank` |

---

## 📊 Reporting

### HTML Test Report

```powershell
dotnet test --logger "html;LogFileName=TestReport.html"
# Open report
start ParaBankAutomation/TestResults/TestReport.html
```

### Screenshots on Failure

Auto-saved to `screenshots/` folder on every failed test.
Format: `BELHAJJI_Test_MMddyyyyHHmmss.png`

### Jenkins Build Results

Go to: http://localhost:8081 → DXC-Playwright-CI-Docker → latest build → **Test Results**

---

## 🔧 Troubleshooting

### ❌ "User has 0 accounts"

Test data is missing. Follow the [Test Data Requirements](#-test-data-requirements) section to create `john`'s accounts.

### ❌ "Timeout 30000ms exceeded waiting for Locator"

Either ParaBank is unreachable or the `BaseUrl` is wrong.

```powershell
# Check if ParaBank container is running
docker ps | findstr parabank-app

# Restart if needed
docker start parabank-app
```

### ❌ "permission denied while trying to connect to the Docker daemon socket"

```powershell
docker exec -it --user root jenkins-parabank-docker bash -c "chmod 666 /var/run/docker.sock"
```

### ❌ "dotnet: command not found" inside Jenkins

The Docker agent (`mcr.microsoft.com/dotnet/sdk:10.0`) should include .NET. If not:

```groovy
// In Jenkinsfile, verify agent image
agent { docker { image 'mcr.microsoft.com/dotnet/sdk:10.0' } }
```

### ❌ Build fails with permission denied on bin/obj

The Jenkinsfile already handles this — Stage 1 runs `rm -rf bin obj`. If still failing:

```powershell
# Manually clean inside container
docker exec -it jenkins-parabank-docker bash -c \
  "rm -rf /var/jenkins_home/workspace/DXC-Playwright-CI-Docker/ParaBankAutomation/bin \
          /var/jenkins_home/workspace/DXC-Playwright-CI-Docker/ParaBankAutomation/obj"
```

### ❌ `.error` element is hidden (Login error tests)

ParaBank occasionally shows an internal error with a hidden `<p class="error">` instead of the standard validation message. This is a ParaBank server issue — wait a few minutes and retry.

### ❌ NuGet restore fails (network timeout)

```powershell
dotnet restore --verbosity detailed
```

Check corporate proxy settings if on a restricted network.

---

## 🌿 Branch Strategy

| Branch | Purpose |
|--------|---------|
| `main` | Stable, production-ready tests |
| `n8n/auto-fix` | n8n automated fix experiments |

---

## 👥 Authors

**Mohamed Amin Belhajji**
QA Test Automation Engineer — DXC Technology
Framework: ParaBank BDD Test Automation
Stack: C# | SpecFlow | Playwright | NUnit | Jenkins | Docker
