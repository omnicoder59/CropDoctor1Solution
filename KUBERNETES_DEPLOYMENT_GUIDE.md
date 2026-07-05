# Kubernetes Deployment Guide for CropDoctor

## Prerequisites

1. **Kubernetes Cluster** running (AKS, EKS, GKE, or local minikube)
2. **kubectl** installed and configured
3. **Docker image** pushed to a registry accessible from K8s
4. **SQL Server** deployed (or managed service like Azure SQL)

---

## Step 1: Push Docker Image to Registry

### Option A: Docker Hub

```bash
docker login

# Tag the image
docker tag cropdoctor:latest <your-username>/cropdoctor:latest

# Push
docker push <your-username>/cropdoctor:latest
```

### Option B: Azure Container Registry

```powershell
az login
az acr login --name <registry-name>

# Tag the image
docker tag cropdoctor:latest <registry-name>.azurecr.io/cropdoctor:latest

# Push
docker push <registry-name>.azurecr.io/cropdoctor:latest
```

### Option C: Private Registry

Update the image URL in `k8s/deployment.yaml`:

```yaml
image: your-registry.com/cropdoctor:latest
imagePullSecrets:
- name: regcred
```

Create the secret:

```bash
kubectl create secret docker-registry regcred \
  --docker-server=your-registry.com \
  --docker-username=<username> \
  --docker-password=<password> \
  --docker-email=<email> \
  -n cropdoctor
```

---

## Step 2: Prepare SQL Server

### Option A: Deploy SQL Server in Kubernetes

```bash
kubectl apply -f k8s-sqlserver.yaml
```

See `k8s-sqlserver.yaml` below.

### Option B: Use Managed Database (Recommended)

- **Azure SQL Database** (Azure)
- **Amazon RDS** (AWS)
- **Google Cloud SQL** (GCP)

Update connection string in `k8s/secret.yaml` with your managed database endpoint.

---

## Step 3: Update Kubernetes Manifests

Edit `k8s/deployment.yaml`:

```yaml
image: your-registry/cropdoctor:latest  # Your registry URL
```

Edit `k8s/secret.yaml` (or use environment-specific overrides):

```yaml
ConnectionStrings__DefaultConnection: "Server=your-sqlserver-endpoint;Database=CropDoctorDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;Encrypt=true;"
```

Edit `k8s/ingress.yaml` (if using Ingress):

```yaml
- host: your-domain.com
  http:
	paths:
	- path: /
	  pathType: Prefix
	  backend:
		service:
		  name: cropdoctor-service
		  port:
			number: 80
```

---

## Step 4: Deploy to Kubernetes

### Using kubectl directly

```bash
# Create namespace
kubectl create namespace cropdoctor

# Apply all manifests
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/hpa.yaml
# kubectl apply -f k8s/ingress.yaml  # If using Ingress
```

### Using Kustomize (Recommended)

```bash
kubectl apply -k k8s/
```

---

## Step 5: Verify Deployment

```bash
# Check namespace
kubectl get namespace cropdoctor

# Check pods
kubectl get pods -n cropdoctor

# Check deployment
kubectl get deployment -n cropdoctor

# Check service
kubectl get service -n cropdoctor

# Check pod details
kubectl describe pod <pod-name> -n cropdoctor

# View logs
kubectl logs -f deployment/cropdoctor -n cropdoctor

# Get service external IP (if LoadBalancer)
kubectl get svc cropdoctor-service -n cropdoctor
```

---

## Step 6: Access the Application

### Via LoadBalancer Service

```bash
kubectl get svc cropdoctor-service -n cropdoctor
# Get the EXTERNAL-IP, then access: http://EXTERNAL-IP
```

### Via Port Forwarding (Local Testing)

```bash
kubectl port-forward svc/cropdoctor-service 8080:80 -n cropdoctor

# Access: http://localhost:8080
```

### Via Ingress

```bash
# Requires Ingress controller (nginx, traefik, etc.)
kubectl apply -f k8s/ingress.yaml

# Access: https://your-domain.com
```

---

## Common kubectl Commands

```bash
# Scale deployment
kubectl scale deployment cropdoctor --replicas=5 -n cropdoctor

# Update image
kubectl set image deployment/cropdoctor cropdoctor=your-registry/cropdoctor:v2 -n cropdoctor

# View real-time resources
kubectl top pods -n cropdoctor
kubectl top nodes

# Exec into pod
kubectl exec -it <pod-name> -n cropdoctor -- /bin/bash

# Restart deployment
kubectl rollout restart deployment/cropdoctor -n cropdoctor

# Rollback to previous version
kubectl rollout undo deployment/cropdoctor -n cropdoctor

# Delete everything
kubectl delete namespace cropdoctor
```

---

## Debugging

### Pod not starting?

```bash
kubectl describe pod <pod-name> -n cropdoctor
kubectl logs <pod-name> -n cropdoctor
```

### Check events

```bash
kubectl get events -n cropdoctor --sort-by='.lastTimestamp'
```

### Port forwarding for debugging

```bash
kubectl port-forward pod/<pod-name> 8080:80 -n cropdoctor
```

---

## Bonus: SQL Server in Kubernetes

Create `k8s-sqlserver.yaml`:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: sqlserver
  namespace: cropdoctor
spec:
  type: ClusterIP
  ports:
  - port: 1433
	targetPort: 1433
  selector:
	app: sqlserver
---
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: sqlserver
  namespace: cropdoctor
spec:
  serviceName: sqlserver
  replicas: 1
  selector:
	matchLabels:
	  app: sqlserver
  template:
	metadata:
	  labels:
		app: sqlserver
	spec:
	  containers:
	  - name: sqlserver
		image: mcr.microsoft.com/mssql/server:2022-latest
		env:
		- name: ACCEPT_EULA
		  value: "Y"
		- name: SA_PASSWORD
		  valueFrom:
			secretKeyRef:
			  name: cropdoctor-secrets
			  key: SA_PASSWORD
		- name: MSSQL_PID
		  value: "Developer"
		ports:
		- containerPort: 1433
		resources:
		  requests:
			memory: "2Gi"
			cpu: "1000m"
		  limits:
			memory: "2Gi"
			cpu: "1000m"
		volumeMounts:
		- name: sqlserver-data
		  mountPath: /var/opt/mssql
  volumeClaimTemplates:
  - metadata:
	  name: sqlserver-data
	spec:
	  accessModes:
	  - ReadWriteOnce
	  resources:
		requests:
		  storage: 10Gi
```

Deploy:
```bash
kubectl apply -f k8s-sqlserver.yaml
```

---

## Production Checklist

- [ ] Image pushed to private registry
- [ ] Secret values base64 encoded or use external secret management (Sealed Secrets, HashiCorp Vault)
- [ ] Resource requests/limits set
- [ ] Health checks (liveness/readiness probes) configured
- [ ] Ingress controller installed and configured
- [ ] TLS/SSL certificates set up
- [ ] Database backups configured
- [ ] Monitoring/logging set up (Prometheus, ELK, etc.)
- [ ] Network policies defined
- [ ] Pod Disruption Budgets configured
- [ ] Resource quotas set per namespace

---

## Resources

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [kubectl Cheat Sheet](https://kubernetes.io/docs/reference/kubectl/cheatsheet/)
- [Deploy to AKS](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-deploy-application)
- [Deploy to EKS](https://docs.aws.amazon.com/eks/latest/userguide/getting-started.html)
