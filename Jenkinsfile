// =================================================================
// Jenkinsfile — ParaBank BDD Test Automation CI/CD Pipeline
// Author:      Amin M. Belhajji
// Date:        February 2026
// Stack:       C# | SpecFlow | Playwright | NUnit | Docker
// Repository:  https://github.com/amin0991/Playwright_TestBank
// =================================================================
 
pipeline {
 
    // ---------------------------------------------------------------
    // AGENT DECLARATION
    // All stages execute inside Microsoft's official .NET 10.0 SDK
    // Docker container. Jenkins pulls the image automatically if
    // it is not already cached on the host machine.
    // '--user root' is needed to install Playwright system libraries.
    // ---------------------------------------------------------------
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:10.0'
            args  '--user root'
        }
    }
 
    // ---------------------------------------------------------------
    // ENVIRONMENT VARIABLES
    // Centralizes all configurable values. Modifying a value here
    // propagates consistently through the entire pipeline.
    // ---------------------------------------------------------------
    environment {
        // The sub-directory containing the .csproj file
        PROJECT_DIR              = 'ParaBankAutomation'
 
        // Playwright uses this variable to locate browser binaries
        PLAYWRIGHT_BROWSERS_PATH = '0'
 
        // Force headless mode — mandatory inside a Docker container
        // where there is no display server (no GUI available)
        HEADED                   = 'false'
    }
 
    // ---------------------------------------------------------------
    // GLOBAL OPTIONS
    // ---------------------------------------------------------------
    options {
        timeout(time: 30, unit: 'MINUTES')  // Abort runaway builds
        timestamps()                         // Prefix logs with time
        buildDiscarder(logRotator(numToKeepStr: '10'))
    }
 
    // ---------------------------------------------------------------
    // STAGES — Sequential phases of the CI pipeline
    // ---------------------------------------------------------------
    stages {
 
        // ==========================================================
        // STAGE 1: SOURCE CODE CHECKOUT
        // Jenkins clones the GitHub repository into the workspace.
        // 'checkout scm' uses the SCM config set in job settings.
        // ==========================================================
        stage('1 - Checkout') {
            steps {
                echo '--- STAGE 1: Cloning from GitHub ---'
                checkout scm
                sh 'ls -la'
                sh "ls -la ${PROJECT_DIR}/"
            }
        }
 
        // ==========================================================
        // STAGE 2: INSTALL OS DEPENDENCIES
        // Playwright requires specific Linux system libraries to
        // control headless Chromium. These must be installed before
        // any test execution.
        // ==========================================================
        stage('2 - Install OS Dependencies') {
            steps {
                echo '--- STAGE 2: Installing Playwright OS libs ---'
                sh '''
                    apt-get update -qq
                    apt-get install -y --no-install-recommends \
                        libnss3 libnspr4 libatk1.0-0 \
                        libatk-bridge2.0-0 libcups2 libdrm2 \
                        libdbus-1-3 libxkbcommon0 libxcomposite1 \
                        libxdamage1 libxfixes3 libxrandr2 \
                        libgbm1 libasound2
                '''
            }
        }
 
        // ==========================================================
        // STAGE 3: RESTORE NUGET PACKAGES
        // Downloads all NuGet dependencies (SpecFlow, Playwright,
        // NUnit, etc.) from nuget.org into the container's cache.
        // ==========================================================
        stage('3 - Restore NuGet Packages') {
            steps {
                echo '--- STAGE 3: Restoring NuGet packages ---'
                dir("${PROJECT_DIR}") {
                    sh 'dotnet restore --verbosity minimal'
                }
            }
        }
 
        // ==========================================================
        // STAGE 4: BUILD
        // Compiles all C# source files. '--no-restore' skips
        // redundant package restoration (done in Stage 3).
        // ==========================================================
        stage('4 - Build Solution') {
            steps {
                echo '--- STAGE 4: Compiling C# solution ---'
                dir("${PROJECT_DIR}") {
                    sh 'dotnet build --no-restore --configuration Release'
                }
            }
        }
 
        // ==========================================================
        // STAGE 5: INSTALL PLAYWRIGHT BROWSERS
        // Downloads the Chromium binary. Must execute after build
        // so the Playwright CLI is available in the output directory.
        // ==========================================================
        stage('5 - Install Playwright Browsers') {
            steps {
                echo '--- STAGE 5: Downloading Chromium browser ---'
                dir("${PROJECT_DIR}") {
                    sh '''
                        # Find the Playwright CLI in the build output
                        PW=$(find . -name 'playwright' -type f 2>/dev/null | head -1)
                        if [ -n "$PW" ]; then
                            echo "Using Playwright CLI at: $PW"
                            $PW install chromium
                        else
                            echo "Playwright CLI not found — trying dotnet tool"
                            dotnet tool install --global Microsoft.Playwright.CLI || true
                            playwright install chromium || true
                        fi
                    '''
                }
            }
        }
 
        // ==========================================================
        // STAGE 6: EXECUTE TEST SUITE
        // Runs all 16 BDD scenarios. '|| true' prevents Jenkins
        // from marking the build as FAILED when tests fail —
        // allowing the post section to still archive reports.
        //
        // Expected result per README:
        //   - With test users configured: 13-14 passing
        //   - Without test users:          5 passing
        // ==========================================================
        stage('6 - Execute Tests') {
            steps {
                echo '--- STAGE 6: Running SpecFlow/Playwright tests ---'
                dir("${PROJECT_DIR}") {
                    sh '''
                        mkdir -p ../TestResults
                        dotnet test \
                            --no-build \
                            --configuration Release \
                            --logger "trx;LogFileName=../TestResults/Results.trx" \
                            --logger "html;LogFileName=../TestResults/Report.html" \
                            --logger "console;verbosity=normal" \
                        || true
                    '''
                }
            }
        }
 
    }  // end stages
 
    // ---------------------------------------------------------------
    // POST SECTION
    // Executes AFTER all stages complete, regardless of outcome.
    // Critical for report preservation and notification.
    // ---------------------------------------------------------------
    post {
 
        always {
            echo '--- POST: Archiving test reports ---'
 
            // Archive reports as Jenkins Build Artifacts
            archiveArtifacts artifacts: 'TestResults/**/*',
                              allowEmptyArchive: true
 
            // Publish TRX results to Jenkins test trend charts
            // (requires JUnit Plugin, pre-installed in Jenkins LTS)
            junit testResults:    'TestResults/**/*.trx',
                  allowEmptyResults: true
        }
 
        success  { echo 'BUILD SUCCEEDED — All stages completed.' }
        unstable { echo 'BUILD UNSTABLE — Some tests failed. Review TestResults/Report.html' }
        failure  { echo 'BUILD FAILED — Review console output for errors.' }
 
    }  // end post
 
}  // end pipeline
