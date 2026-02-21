pipeline {

    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:10.0'
            args '--user root'
        }
    }

    environment {
        PROJECT_DIR              = 'ParaBankAutomation'
        PLAYWRIGHT_BROWSERS_PATH = '0'
    }

    options {
        timeout(time: 30, unit: 'MINUTES')
        buildDiscarder(logRotator(numToKeepStr: '10'))
    }

    stages {

        stage('1 - Checkout') {
            steps {
                echo '--- Cloning repository from GitHub ---'
                checkout scm
                sh "ls -la ${PROJECT_DIR}/"
            }
        }

        stage('2 - Install OS Dependencies') {
            steps {
                echo '--- Installing Playwright Linux libraries ---'
                sh '''
                    apt-get update -qq
                    apt-get install -y --no-install-recommends \
                        libnss3 libnspr4 libatk1.0-0 \
                        libatk-bridge2.0-0 libcups2 libdrm2 \
                        libdbus-1-3 libxkbcommon0 libxcomposite1 \
                        libxdamage1 libxfixes3 libxrandr2 \
                        libgbm1 libasound2t64
                '''
            }
        }

        stage('3 - Restore NuGet Packages') {
            steps {
                echo '--- Restoring NuGet dependencies ---'
                dir("${PROJECT_DIR}") {
                    sh 'dotnet restore --verbosity minimal'
                }
            }
        }

        stage('4 - Build Solution') {
            steps {
                echo '--- Compiling C# solution ---'
                dir("${PROJECT_DIR}") {
                    sh 'dotnet build --no-restore --configuration Release'
                }
            }
        }

        stage('5 - Install Playwright Browsers') {
           steps
           {
                echo '--- Downloading Chromium browser ---'
                dir("${PROJECT_DIR}") 
                {
                        sh '''
                            export PATH="$PATH:/root/.dotnet/tools"
                            dotnet tool install --global Microsoft.Playwright.CLI || true
                            /root/.dotnet/tools/playwright install chromium
                            '''
                }
            }
        }

        stage('6 - Execute Tests') {
            steps {
                echo '--- Running all 16 BDD scenarios ---'
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

    }

        post {
            always {
                archiveArtifacts artifacts: '**/TestResults/**/*', allowEmptyArchive: true
                junit testResults: '**/TestResults/**/*.trx', allowEmptyResults: true
            }
            success  { echo 'ALL STAGES PASSED.' }
            unstable { echo 'SOME TESTS FAILED — check TestResults/Report.html' }
            failure  { echo 'PIPELINE FAILED — check Console Output.' }
    }
}