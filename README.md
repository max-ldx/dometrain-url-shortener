# dometrain-url-shortener
Let's Build It : URL Shortener Course

## Infrastructure as Code

### Log in into Azure

```bash
az login
```

### Create Resource Group

```bash
az group create --name dometrain-urlshortener-dev --location francecentral
```

### Create User for GH Actions

```bash
az ad sp create-for-rbac --name "GitHub-Actions-SP" \
                         --role contributor \
                         --scopes /subscriptions/684381a3-7c37-4390-9a25-ffe6003169ad \
                         --sdk-auth
```

### Apply to Custom Contributor Role
```bash
az ad sp create-for-rbac --name "GitHub-Actions-SP" \
                         --role 'infra_deploy' \
                         --scopes /subscriptions/684381a3-7c37-4390-9a25-ffe6003169ad \
                         --sdk-auth
```

### Configure a federated identity credential on an app
