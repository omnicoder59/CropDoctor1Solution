@echo off
REM Quick start script for Docker build and deployment

echo ========================================
echo CropDoctor1 - Docker Build Script
echo ========================================

REM Check if Docker is installed
docker --version >nul 2>&1
if errorlevel 1 (
	echo ERROR: Docker is not installed or not in PATH
	exit /b 1
)

REM Set variables
set IMAGE_NAME=cropdoctor
set IMAGE_TAG=latest
set CONTAINER_NAME=cropdoctor-app
set PORT=8080

echo.
echo Step 1: Building Docker image...
echo Command: docker build -t %IMAGE_NAME%:%IMAGE_TAG% .
docker build -t %IMAGE_NAME%:%IMAGE_TAG% .
if errorlevel 1 (
	echo ERROR: Docker build failed
	exit /b 1
)

echo.
echo Step 2: Stopping existing container (if any)...
docker stop %CONTAINER_NAME% >nul 2>&1
docker rm %CONTAINER_NAME% >nul 2>&1

echo.
echo Step 3: Running Docker container...
echo Command: docker run -d -p %PORT%:80 --name %CONTAINER_NAME% %IMAGE_NAME%:%IMAGE_TAG%
docker run -d -p %PORT%:80 --name %CONTAINER_NAME% %IMAGE_NAME%:%IMAGE_TAG%
if errorlevel 1 (
	echo ERROR: Docker run failed
	exit /b 1
)

echo.
echo Step 4: Waiting for application to start...
timeout /t 5 /nobreak

echo.
echo ========================================
echo Build and deployment successful!
echo ========================================
echo.
echo Application is running at: http://localhost:%PORT%
echo Container name: %CONTAINER_NAME%
echo Image: %IMAGE_NAME%:%IMAGE_TAG%
echo.
echo Useful commands:
echo   View logs:    docker logs %CONTAINER_NAME%
echo   Stop:         docker stop %CONTAINER_NAME%
echo   Restart:      docker restart %CONTAINER_NAME%
echo   Remove:       docker rm %CONTAINER_NAME%
echo.
