param(
    [Parameter(Mandatory=$true)]
    [string]$FunctionAppName,
    
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$VMResourceGroupName,
    
    [string]$SubscriptionId = (az account show --query id -o tsv)
)

Write-Host "Configuring Managed Identity roles for Function App: $FunctionAppName" -ForegroundColor Green

# Enable system-assigned managed identity if not already enabled
Write-Host "Enabling system-assigned managed identity..."
$identity = az functionapp identity assign `
    --name $FunctionAppName `
    --resource-group $ResourceGroupName `
    --query principalId -o tsv

if (-not $identity) {
    Write-Error "Failed to get or create managed identity"
    exit 1
}

Write-Host "Managed Identity Principal ID: $identity" -ForegroundColor Cyan

# Define the scope for VM operations (at resource group level)
$scope = "/subscriptions/$SubscriptionId/resourceGroups/$VMResourceGroupName"

Write-Host "Assigning roles at scope: $scope" -ForegroundColor Yellow

# Assign Virtual Machine Contributor role for VM management operations
Write-Host "Assigning Virtual Machine Contributor role..."
az role assignment create `
    --assignee $identity `
    --role "Virtual Machine Contributor" `
    --scope $scope

# Assign Reader role for general resource visibility
Write-Host "Assigning Reader role..."
az role assignment create `
    --assignee $identity `
    --role "Reader" `
    --scope $scope

# If using managed disks, also need this role
Write-Host "Assigning Managed Disk Operator role..."
az role assignment create `
    --assignee $identity `
    --role "Disk Snapshot Contributor" `
    --scope $scope

# If you need to read VM extensions
Write-Host "Assigning Virtual Machine User Login role..."
az role assignment create `
    --assignee $identity `
    --role "Virtual Machine User Login" `
    --scope $scope

Write-Host "`nRole assignments completed!" -ForegroundColor Green
Write-Host "Note: It may take a few minutes for the role assignments to propagate." -ForegroundColor Yellow

# Display current role assignments
Write-Host "`nCurrent role assignments for the Managed Identity:" -ForegroundColor Cyan
az role assignment list --assignee $identity --all -o table
