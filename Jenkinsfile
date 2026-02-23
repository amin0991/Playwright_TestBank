pipeline {

    agent none

    environment {
        IS_LOCAL                 = 'true'
        PROJECT_DIR              = 'ParaBankAutomation'
        PLAYWRIGHT_BROWSERS_PATH = '0'
    }

    options {
        timeout(time: 30, unit: 'MINUTES')
        buildDiscarder(logRotator(numToKeepStr: '10'))
    }

    stages {

        stage('Run Local') {
            when {
                beforeAgent true
                expression { env.IS_LOCAL == 'true' }
            }
            agent { label 'built-in' }
            stages {

                stage('1 - Checkout') {
                    steps {
                        echo '--- LOCAL: Cloning repository ---'
                        checkout scm
                        sh "ls -la ${PROJECT_DIR}/"
                        sh "rm -rf ${PROJECT_DIR}/bin ${PROJECT_DIR}/obj || true"   // ← clean slate every build
                        sh "chmod -R 777 /var/jenkins_home/.nuget || true"
                    }
                }

                stage('3 - Restore NuGet Packages') {
                    steps {
                        dir("${PROJECT_DIR}") {
                            sh 'dotnet restore --verbosity minimal'
                        }
                    }
                }

                stage('4 - Build Solution') {
                    steps {
                        dir("${PROJECT_DIR}") {
                            sh 'dotnet build --no-restore --configuration Release'
                        }
                    }
                }

                stage('5 - Install Playwright Browsers') {
                    steps {
                        dir("${PROJECT_DIR}") {
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

        stage('Run Docker') {
            when {
                beforeAgent true
                expression { env.IS_LOCAL == 'false' }
            }
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:10.0'
                    args '--user root'
                }
            }
            stages {

                stage('1 - Checkout') {
                    steps {
                        echo '--- DOCKER: Cloning repository ---'
                        checkout scm
                        sh "ls -la ${PROJECT_DIR}/"
                    }
                }

                stage('2 - Install OS Dependencies') {
                    steps {
                        sh '''
                            apt-get update -qq
                            apt-get install -y --no-install-recommends \
                                libnss3 libnspr4 libatk1.0-0 \
                                libatk-bridge2.0-0 libcups2 libdrm2 \
                                libdbus-1-3 libxkbcommon0 libxcomposite1 \
                                libxdamage1 libxfixes3 libxrandr2 \
                                libgbm1 libasound2t64 \
                                libpango-1.0-0 libcairo2
                        '''
                    }
                }

                stage('3 - Restore NuGet Packages') {
                    steps {
                        dir("${PROJECT_DIR}") {
                            sh 'dotnet restore --verbosity minimal'
                        }
                    }
                }

                stage('4 - Build Solution') {
                    steps {
                        dir("${PROJECT_DIR}") {
                            sh 'dotnet build --no-restore --configuration Release'
                        }
                    }
                }

                stage('5 - Install Playwright Browsers') {
                    steps {
                        dir("${PROJECT_DIR}") {
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
    }
}