pipeline {
	agent any

	environment {
		DOCKER_REGISTRY = credentials('docker-registry-credentials') // Jenkins credentials for Docker registry
		IMAGE_NAME = "cropdoctor"
		IMAGE_TAG = "${BUILD_NUMBER}"
		DOCKER_IMAGE = "${DOCKER_REGISTRY_USR}/${IMAGE_NAME}:${IMAGE_TAG}"
		DOCKER_IMAGE_LATEST = "${DOCKER_REGISTRY_USR}/${IMAGE_NAME}:latest"
	}

	stages {
		stage('Checkout') {
			steps {
				script {
					echo 'Checking out source code...'
					checkout([$class: 'GitSCM',
						branches: [[name: '*/master']],
						userRemoteConfigs: [[
							url: 'https://github.com/omnicoder59/CropDoctor1Solution.git',
							credentialsId: 'github-credentials' // Configure this in Jenkins
						]]
					])
				}
			}
		}

		stage('Build') {
			steps {
				script {
					echo 'Building .NET 8 application...'
					bat 'dotnet build CropDoctor1/CropDoctor1.csproj -c Release'
				}
			}
		}

		stage('Test') {
			steps {
				script {
					echo 'Running tests...'
					// Uncomment if you have test projects
					// bat 'dotnet test CropDoctor1.Tests/CropDoctor1.Tests.csproj -c Release --no-build'
				}
			}
		}

		stage('Build Docker Image') {
			steps {
				script {
					echo "Building Docker image: ${DOCKER_IMAGE}"
					bat """
						docker build -t ${DOCKER_IMAGE} .
						docker tag ${DOCKER_IMAGE} ${DOCKER_IMAGE_LATEST}
					"""
				}
			}
		}

		stage('Push to Registry') {
			steps {
				script {
					echo "Pushing image to Docker registry..."
					bat """
						docker login -u ${DOCKER_REGISTRY_USR} -p ${DOCKER_REGISTRY_PSW}
						docker push ${DOCKER_IMAGE}
						docker push ${DOCKER_IMAGE_LATEST}
					"""
				}
			}
		}

		stage('Deploy') {
			steps {
				script {
					echo 'Deploying container...'
					// Example: Deploy to local Docker or remote server
					// Customize based on your deployment target (Docker Swarm, Kubernetes, etc.)

					// For local deployment:
					bat """
						docker run -d -p 8080:80 --name cropdoctor-${BUILD_NUMBER} ${DOCKER_IMAGE}
					"""

					// For remote deployment via SSH:
					// sshagent(['ssh-credentials-id']) {
					//     sh '''
					//         ssh user@remote-host "docker pull ${DOCKER_IMAGE} && docker run -d -p 80:80 ${DOCKER_IMAGE}"
					//     '''
					// }
				}
			}
		}

		stage('Health Check') {
			steps {
				script {
					echo 'Waiting for service to start...'
					sleep(time: 10, unit: 'SECONDS')

					echo 'Checking health...'
					retry(5) {
						bat 'curl -f http://localhost:8080/health || exit /b 1'
					}
				}
			}
		}
	}

	post {
		always {
			echo 'Pipeline completed'
			// Cleanup old containers
			bat '''
				FOR /f "tokens=*" %%i IN ('docker ps -a -q -f "status=exited"') DO docker rm %%i
				FOR /f "tokens=*" %%i IN ('docker images -q --filter "dangling=true"') DO docker rmi %%i
			'''
		}
		success {
			echo 'Deployment successful!'
		}
		failure {
			echo 'Pipeline failed!'
		}
	}
}
