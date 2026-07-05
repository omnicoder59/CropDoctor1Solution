# Docker & Jenkins Deployment Guide for CropDoctor1

## Prerequisites

1. **Docker** installed and running
2. **Jenkins** server running (2.400+)
3. **Git** installed on Jenkins agent
4. **.NET 8 SDK** or Docker CLI access
5. **Docker Registry** account (Docker Hub, Azure Container Registry, etc.)

## Step 1: Local Testing with Docker

### Build the Docker image locally
```bash
docker build -t cropdoctor:latest .
```

### Run the container locally
```bash
docker run -d -p 8080:80 --name cropdoctor-local cropdoctor:latest
```

### Test the application
```bash
curl http://localhost:8080
```

### Using Docker Compose (with SQL Server)
```bash
docker-compose up -d
```

## Step 2: Jenkins Configuration

### 2.1 Install Required Jenkins Plugins
- Docker Pipeline
- Pipeline: Stage View
- Git
- GitHub Integration

**Steps:**
1. Go to **Manage Jenkins** → **Manage Plugins**
2. Search for and install the plugins listed above
3. Restart Jenkins

### 2.2 Configure Docker Credentials in Jenkins

1. Go to **Manage Jenkins** → **Credentials**
2. Click **Global** → **Add Credentials**
3. Select **Username with password**
4. Fill in:
   - **Username**: Your Docker registry username
   - **Password**: Your Docker registry password (or token)
   - **ID**: `docker-registry-credentials`
   - Click **Save**

### 2.3 Configure Git Credentials in Jenkins

1. Go to **Manage Jenkins** → **Credentials**
2. Click **Global** → **Add Credentials**
3. Select **SSH Key** or **Username with password**
4. Fill in your GitHub credentials
5. **ID**: `github-credentials`
6. Click **Save**

### 2.4 Create a New Jenkins Pipeline Job

1. Click **New Item**
2. Enter job name: `CropDoctor1-Docker-Build`
3. Select **Pipeline**
4. Click **OK**

### 2.5 Configure the Pipeline

In the **Pipeline** section:

**Definition**: Select "Pipeline script from SCM"

**SCM**: Select "Git"
- **Repository URL**: `https://github.com/omnicoder59/CropDoctor1Solution.git`
- **Credentials**: Select `github-credentials`
- **Branch Specifier**: `*/master`
- **Script Path**: `Jenkinsfile`

Click **Save**

## Step 3: Docker Registry Setup

### For Docker Hub
```bash
docker login
# Enter your Docker Hub credentials
```

### For Azure Container Registry
```bash
az acr login --name <registry-name>
```

### For Private Registry
Update the `DOCKER_REGISTRY` credentials in Jenkins

## Step 4: Update Jenkinsfile for Your Environment

Edit the `Jenkinsfile` and customize:

1. **Registry URL**: Change `DOCKER_REGISTRY_USR` to your registry
2. **Image Name**: Change `IMAGE_NAME` if needed
3. **Deployment Target**: Update the Deploy stage for your infrastructure:
   - **Docker Swarm**: Use `docker service update`
   - **Kubernetes**: Use `kubectl apply` with deployment manifest
   - **Cloud VM**: Use SSH to deploy on remote server

### Example for Remote SSH Deployment
```groovy
stage('Deploy') {
	steps {
		sshagent(['ssh-credentials-id']) {
			sh '''
				ssh user@your-server "docker pull ${DOCKER_IMAGE} && \
				docker run -d -p 80:80 --restart unless-stopped ${DOCKER_IMAGE}"
			'''
		}
	}
}
```

### Example for Kubernetes Deployment
```groovy
stage('Deploy') {
	steps {
		sh '''
			kubectl set image deployment/cropdoctor \
			cropdoctor=${DOCKER_IMAGE} --record
		'''
	}
}
```

## Step 5: Trigger the Pipeline

### Option A: Manual Trigger
1. Go to your Jenkins job
2. Click **Build Now**

### Option B: GitHub Webhook (Automatic on Push)
1. Go to your GitHub repository settings
2. Click **Webhooks** → **Add webhook**
3. **Payload URL**: `http://your-jenkins-url/github-webhook/`
4. **Content type**: `application/json`
5. **Events**: Select "Push events"
6. Click **Add webhook**

Jenkins will now automatically build and deploy on every push to master.

## Step 6: Environment Configuration

For database connections and other environment-specific settings, either:

### Option A: Environment Variables in docker-compose.yml
```yaml
environment:
  ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=CropDoctor;..."
```

### Option B: Configuration Files (appsettings.Production.json)
Mount configuration files:
```bash
docker run -v /path/to/appsettings.Production.json:/app/appsettings.Production.json ...
```

### Option C: Secrets Management (Recommended)
Use Jenkins credentials or Docker secrets:
```groovy
environment {
	DB_CONNECTION = credentials('database-connection-string')
}
```

## Troubleshooting

### Docker build fails
- Check Docker is running: `docker ps`
- Clear Docker cache: `docker builder prune`

### Jenkins can't find Jenkinsfile
- Ensure `Jenkinsfile` is in root of repository
- Check branch name matches (default: `master` or `main`)

### Container exits immediately
- Check logs: `docker logs <container-id>`
- Verify appsettings.json configuration
- Check database connection string

### Permission denied errors
- Run Jenkins agent as Docker user: Add Jenkins user to docker group
- Linux: `sudo usermod -aG docker jenkins`

## Security Best Practices

1. **Never commit secrets** - Use Jenkins credentials
2. **Use minimal base images** - Current Dockerfile uses ASP.NET runtime only
3. **Run containers as non-root** - Add to Dockerfile:
   ```dockerfile
   RUN useradd -m appuser
   USER appuser
   ```
4. **Scan images for vulnerabilities**:
   ```bash
   docker run --rm -v /var/run/docker.sock:/var/run/docker.sock aquasec/trivy image <image>
   ```
5. **Use image tags** - Never use `latest` in production
6. **Enable container signing** - Use Docker Content Trust

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Jenkins Pipeline Documentation](https://www.jenkins.io/doc/book/pipeline/)
- [ASP.NET Core Deployment](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [Docker Security Best Practices](https://docs.docker.com/develop/dev-best-practices/)
