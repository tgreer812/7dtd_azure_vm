# Backend Deployment Proposal

This document proposes a simple, script‑based approach for publishing the Azure Functions backend to the infrastructure deployed by the ARM templates.

## Overview
- Infrastructure is provisioned using the PowerShell scripts under `Deployment/Azure/Scripts`. `deploy_functions.ps1` creates the Function App using ARM templates.
- After the Function App exists, the backend code must be published so the Functions are available.
- Instead of a full CI/CD pipeline, we keep deployment manual via scripts checked into the repo.

## Proposed Directory
Create a new directory within `Backend`:

```
Backend/Scripts/
```

This folder will contain helper scripts for building and publishing the backend.

## Publish Script
Add a PowerShell script named `publish_functions.ps1` inside `Backend/Scripts`. Its responsibilities:

1. **Build** the `ServerManagement.Functions` project (default `Release` configuration).
2. **Publish** the project using Azure Functions Core Tools (`func azure functionapp publish`).
3. Accept the target Function App name as a parameter and optionally the build configuration.

Example usage:

```powershell
./publish_functions.ps1 -FunctionAppName my-func-app -Configuration Release
```

The script resolves the Functions project path relative to itself, runs `dotnet publish`, then executes:

```powershell
func azure functionapp publish my-func-app --csharp --dotnet-isolated --publish-local-settings -i
```

Environment variables or a small config file can be used to store the Function App name so it does not have to be typed each time.

## Combined Deployment Script
An additional script `deploy_backend.ps1` orchestrates the entire process. It can provision the Function App infrastructure and then publish the compiled code.

Example usage:

```powershell
./deploy_backend.ps1 -FunctionAppName my-func-app -ProvisionInfrastructure
```

## Deployment Flow
1. Run `Deployment/Azure/Scripts/deploy_functions.ps1` to create or update the Function App infrastructure. This script uses the ARM templates under `Deployment/Azure/Arm/Functions` (not included in the repository).
2. After the resources exist, invoke `Backend/Scripts/publish_functions.ps1` with the Function App name to upload the latest compiled code.
3. The Functions app will restart automatically and begin serving requests.

This lightweight approach keeps deployment simple while still allowing repeatable, scripted releases without a full CI/CD pipeline.
