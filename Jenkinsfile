// // Jenkinsfile for CI/CD Pipeline //
// // This Jenkinsfile defines a CI/CD pipeline for a .NET application that builds, tests, and deploys the application to Docker Hub and IIS.
//ngonnnnnnnnnnn  
//  ------------------ tich hop traefik ----------------------

pipeline {
    agent any
    environment {
        LANG = 'en_US.UTF-8'
        LC_ALL = 'en_US.UTF-8'
        DOCKERHUB_CREDENTIALS = 'a8043e21-320b-4f12-b72e-612d7a93c553'
        IMAGE_NAME = 'zyond/web_cicd'
        DOCKER_IMAGE_NAME = 'zyond/web_cicd'
        DOCKER_TAG = 'latest'
        MINIO_CREDENTIALS = 'ec062030-09a1-4183-8f4f-81e593dacae3'
    }
    stages {
        stage('Clone') {
            steps {
                echo 'Cloning source code'
                git branch: 'master', url: 'https://github.com/zyond26/Restaurant_cicd.git'
            }
        }

        stage('Restore Packages') {
            steps {
                echo 'Restoring NuGet packages...'
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo 'Building the project...'
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Run Tests') {
            steps {
                echo 'Running unit tests...'
                bat 'dotnet test --no-build --verbosity normal'
            }
        }

        stage('Publish to Folder') {
            steps {
                echo 'Cleaning old publish folder...'
                bat 'if exist "%WORKSPACE%\\publish" rd /s /q "%WORKSPACE%\\publish"'
                echo 'Publishing to temporary folder...'
                bat 'dotnet publish -c Release -o "%WORKSPACE%\\publish"'
            }
        }

        // ----------- Build and Push Docker Image -----------

        stage('Build Docker Image') {
            steps {
                script {
                    docker.build("${DOCKER_IMAGE_NAME}:${DOCKER_TAG}")
                }
            }
        }

        stage('Login to Docker Hub') {
            steps {
                script {
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        // Login success
                    }
                }
            }
        }

        stage('Push Docker Image') {
            steps {
                script {
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        docker.image("${DOCKER_IMAGE_NAME}:${DOCKER_TAG}").push()
                    }
                }
            }
        }

        // ----------- Chạy Container từ Image vừa build -----------

        stage('Run Docker Container') {
            steps {
                script {
                    echo 'Stopping and removing existing container (if any)...'
                    bat 'docker stop myweb || exit 0'
                    bat 'docker rm myweb || exit 0'

                    echo 'Running new container...'
                    bat "docker run -d -p 5000:5000 --name myweb ${DOCKER_IMAGE_NAME}:${DOCKER_TAG}"
                }
            }
        }

        // ----------- MinIO Upload -----------

        stage('Tạo file test') {
            steps {
                bat 'echo Build thành công > build-log.txt'
            }
        }

        stage('Cấu hình AWS CLI cho MinIO') {
            steps {
                bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" configure set aws_access_key_id admin'
                bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" configure set aws_secret_access_key 12345678'
            }
        }

        stage('Upload file lên MinIO') {
            steps {
                bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" --endpoint-url http://minio.localhost s3 cp WebRestaurant12_autobackup_629062_2025-07-28T10-16-35.BAK s3://order-files/WebRestaurant12_autobackup_629062_2025-07-28T10-16-35.BAK'
            }
        }

        // ----------- Deploy to IIS -----------

        stage('Copy to IIS Folder') {
            steps {
                echo 'Stopping IIS...'
                bat 'iisreset /stop'
                echo 'Cleaning existing deploy folder...'
                bat 'if exist C:\\WOR_cicd rd /s /q C:\\WOR_cicd'
                echo 'Creating IIS folder...'
                bat 'mkdir C:\\WOR_cicd'
                echo 'Copying to IIS folder...'
                bat 'xcopy /E /Y /I /R "%WORKSPACE%\\publish\\*" "C:\\WOR_cicd\\"'
                echo 'Starting IIS again...'
                bat 'iisreset /start'
            }
        }

        stage('Ensure IIS Site Exists') {
            steps {
                powershell '''
                    Import-Module WebAdministration
                    $siteName = "WOR_cicd"
                    $sitePath = "C:\\WOR_cicd"
                    $sitePort = 8089
                    if (-not (Test-Path "IIS:\\Sites\\$siteName")) {
                        New-Website -Name $siteName -Port $sitePort -PhysicalPath $sitePath -Force
                    } else {
                        Write-Host "Website $siteName already exists"
                    }
                '''
            }
        }
    }
}
