# CI/CD Pipeline Implementation Guide

## Overview

This guide covers setting up CI/CD pipelines for your CropDoctor application using:
1. **GitHub Actions** (Recommended - built-in, free)
2. **Jenkins** (Self-hosted, flexible)
3. **GitLab CI** (If using GitLab)

---

## Option 1: GitHub Actions (Recommended)

### Prerequisites
- GitHub repository (already have: `https://github.com/omnicoder59/CropDoctor1Solution`)
- Docker Hub account
- Kubernetes cluster with kubeconfig

### Step 1: Add GitHub Secrets

Go to: **Settings → Secrets and variables → Actions**

Add these secrets:

```
DOCKER_USERNAME        = your-docker-username
DOCKER_PASSWORD        = your-docker-password (or PAT token)
KUBE_CONFIG_STAGING    = base64 encoded staging kubeconfig
KUBE_CONFIG_PRODUCTION = base64 encoded production kubeconfig
SLACK_WEBHOOK          = your-slack-webhook-url (optional)
```

**To encode kubeconfig:**

```powershell
# Windows
[Convert]::ToBase64String([System.IO.File]::ReadAllBytes("$env:USERPROFILE\.kube\config")) | Set-Clipboard

# Linux/Mac
cat ~/.kube/config | base64 | xclip -selection clipboard
```

### Step 2: Workflow File Already Created

The file `.github/workflows/ci-cd.yml` is ready to use. It:
- ✅ Builds .NET 8 code
- ✅ Runs tests
- ✅ Scans for vulnerabilities
- ✅ Builds Docker image
- ✅ Scans Docker image
- ✅ Pushes to registry
- ✅ Deploys to Staging (on `develop` branch)
- ✅ Deploys to Production (on `master` branch)

### Step 3: Trigger Pipeline

Push code to trigger:

```powershell
git add .
git commit -m "Update CI/CD pipeline"
git push origin master
```

Check progress: **GitHub → Actions tab**

---

## Option 2: Jenkins Pipeline

### Prerequisites
- Jenkins server running
- Jenkins plugins installed:
  - Docker Pipeline
  - Kubernetes CLI
  - Pipeline: Stage View
  - GitHub Integration

### Step 1: Configure Jenkins Credentials

1. **Manage Jenkins** → **Credentials**
2. Add credentials:

   - **Docker Registry**
	 - Kind: Username with password
	 - ID: `docker-registry-credentials`
	 - Username: Your Docker Hub username
	 - Password: Your Docker Hub token

   - **GitHub**
	 - Kind: Username with password
	 - ID: `github-credentials`
	 - Username: Your GitHub username
	 - Password: Your GitHub PAT token

   - **Kubernetes Config**
	 - Kind: Secret file
	 - ID: `k8s-cluster-config`
	 - File: Your kubeconfig file

### Step 2: Create Jenkins Pipeline Job

1. **New Item**
2. Name: `CropDoctor-CI-CD`
3. Select: **Pipeline**
4. Click **OK**

### Step 3: Configure Pipeline

**Pipeline section:**

```
Definition: Pipeline script from SCM
SCM: Git
  Repository URL: https://github.com/omnicoder59/CropDoctor1Solution.git
  Credentials: github-credentials
  Branch: */master
  Script Path: Jenkinsfile.prod
```

Click **Save**

### Step 4: Trigger Build

Option A: Click **Build Now**

Option B: Set up GitHub webhook:
1. Go to GitHub repo → **Settings → Webhooks**
2. Click **Add webhook**
3. Payload URL: `http://your-jenkins-url/github-webhook/`
4. Content type: `application/json`
5. Events: Push events
6. Click **Add webhook**

---

## Option 3: GitLab CI

If using GitLab instead of GitHub:

### Step 1: Add `.gitlab-ci.yml`

Already created at `.gitlab-ci.yml` - no action needed.

### Step 2: Configure GitLab CI Variables

1. **Project → Settings → CI/CD → Variables**
2. Add:
   ```
   CI_REGISTRY_USER    = your-docker-username
   CI_REGISTRY_PASSWORD = your-docker-token
   KUBE_CONFIG_STAGING = base64 encoded kubeconfig
   KUBE_CONFIG_PRODUCTION = base64 encoded kubeconfig
   ```

### Step 3: Trigger Pipeline

Push code:
```git
git push origin master
```

Check: **GitLab → CI/CD → Pipelines**

---

## Pipeline Flow

### On Push to `develop` branch:
```
Code Push
   ↓
Build & Test
   ↓
Security Scan
   ↓
Build Docker Image
   ↓
Scan Image
   ↓
Deploy to Staging (Manual approval)
```

### On Push to `master` branch:
```
Code Push
   ↓
Build & Test
   ↓
Security Scan
   ↓
Build Docker Image
   ↓
Scan Image
   ↓
Deploy to Production (Manual approval)
   ↓
Notify Slack
```

---

## Branch Strategy

Recommended Git workflow:

```
main/master (Production)
	↑
	(Pull Request - requires approval)
	↑
develop (Staging)
	↑
	(Feature branches)
feature/new-feature
```

**Commands:**

```powershell
# Create feature branch
git checkout -b feature/my-feature

# Make changes and commit
git add .
git commit -m "Add my feature"

# Push to GitHub
git push origin feature/my-feature

# Create Pull Request on GitHub
# After approval, merge to develop
git checkout develop
git pull origin develop

# When ready for production, merge to master
git checkout master
git pull origin master
git merge develop
git push origin master
```

---

## Monitoring Pipeline

### GitHub Actions
- Go to: **Your repo → Actions**
- Click on workflow run to see details
- View logs for each step

### Jenkins
- Go to: `http://your-jenkins-url/job/CropDoctor-CI-CD/`
- Click on build number
- View **Console Output**

### GitLab
- Go to: **Project → CI/CD → Pipelines**
- Click on pipeline
- Click on job to see logs

---

## Rollback Strategy

If deployment fails or has issues:

```bash
# View deployment history
kubectl rollout history deployment/cropdoctor -n cropdoctor

# Rollback to previous version
kubectl rollout undo deployment/cropdoctor -n cropdoctor

# Rollback to specific revision
kubectl rollout undo deployment/cropdoctor --to-revision=2 -n cropdoctor
```

---

## Notifications

### Email Notifications (Jenkins)

Add to Jenkinsfile:
```groovy
post {
	failure {
		emailext(
			subject: "Build Failed: ${env.JOB_NAME} #${env.BUILD_NUMBER}",
			body: "Build failed. Check ${env.BUILD_URL}",
			to: "devops@example.com"
		)
	}
}
```

### Slack Notifications (GitHub Actions)

Already included in workflow. Add webhook URL to GitHub secrets:
```
SLACK_WEBHOOK = https://hooks.slack.com/services/YOUR/WEBHOOK/URL
```

### Teams Notifications

Add to GitHub Actions:
```yaml
- name: Notify Teams
  uses: jdcargile/ms-teams-notification@v1.3
  with:
	github-token: ${{ github.token }}
	ms-teams-webhook-uri: ${{ secrets.TEAMS_WEBHOOK }}
	notification-color: 3278599
```

---

## Performance Optimization

### Cache Docker Layers

```yaml
# Already in GitHub workflow
cache-from: type=registry,ref=${{ env.REGISTRY }}/${{ secrets.DOCKER_USERNAME }}/${{ env.IMAGE_NAME }}:buildcache
cache-to: type=registry,ref=${{ env.REGISTRY }}/${{ secrets.DOCKER_USERNAME }}/${{ env.IMAGE_NAME }}:buildcache,mode=max
```

### Parallel Jobs

GitHub Actions and GitLab CI run jobs in parallel by default.

### Build Cache

Add to Jenkinsfile:
```groovy
options {
	timestamps()
	timeout(time: 30, unit: 'MINUTES')
	buildDiscarder(logRotator(numToKeepStr: '10'))
}
```

---

## Security Best Practices

✅ **Use GitHub Secrets** - Never commit credentials  
✅ **Image Scanning** - Trivy scans for vulnerabilities  
✅ **RBAC** - Kubernetes role-based access control  
✅ **Network Policies** - Restrict pod-to-pod traffic  
✅ **Pod Security Policies** - Enforce security standards  
✅ **Audit Logging** - Track all deployments  

---

## Troubleshooting

### GitHub Actions: Image push fails
```
❌ Error: unauthorized: incorrect username or password
```
Solution: Check DOCKER_PASSWORD is valid (Docker token, not password)

### Kubernetes deployment fails
```
❌ ImagePullBackOff
```
Solution: Check image name and registry credentials are correct

### Pipeline hangs on kubectl
```
⏳ Waiting for rollout status...
```
Solution: Check pod is actually running: `kubectl get pods -n cropdoctor`

---

## Recommended Next Steps

1. ✅ Push code to GitHub
2. ✅ Check GitHub Actions runs successfully
3. ✅ Test deployment to staging
4. ✅ Get team approval
5. ✅ Test deployment to production
6. ✅ Set up monitoring (Prometheus, Grafana)
7. ✅ Set up logging (ELK, Loki)
8. ✅ Configure alerts

---

## Quick Reference

| Tool | Config File | Trigger | Best For |
|------|------------|---------|----------|
| GitHub Actions | `.github/workflows/ci-cd.yml` | Git push | Easy, integrated, free |
| Jenkins | `Jenkinsfile.prod` | Webhook | Custom, powerful, self-hosted |
| GitLab CI | `.gitlab-ci.yml` | Git push | Full GitLab integration |

Pick one and get started! 🚀
