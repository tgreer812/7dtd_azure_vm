# Deploy Web API ARM Template

param(
    [string]$GameServerHost = ""
)

$azureDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$armDir = Resolve-Path (Join-Path $azureDir "\Arm\WebApi")
$configFile = Resolve-Path (Join-Path $armDir "private_parameters.json")
$templateFile = Resolve-Path (Join-Path $armDir "template.json")
$parametersPath = Resolve-Path (Join-Path $armDir "parameters.json")
$parametersFile = "@" + $parametersPath.Path
$resourceGroup = "tg-7d2d-dedicated-test"

Write-Output "ARM Directory: $armDir"
Write-Output "Template File: $templateFile"
Write-Output "Parameters File: $parametersFile"

if (-not (Test-Path $configFile)) {
    Write-Error "A private configuration file named 'private_parameters.json' is expected at: $configFile"
    exit 1
}

# Ensure resource group exists
$resGroups = az group list --query "[].name" -o tsv
$exists = $resGroups -contains $resourceGroup
if (-not $exists) {
    Write-Output "Resource group '$resourceGroup' does not exist. Creating..."
    az group create --name $resourceGroup --location "eastus"
} else {
    Write-Output "Resource group '$resourceGroup' already exists."
}

$extraParam = if ($GameServerHost -ne "") { "--parameters gameServerHost=$GameServerHost" } else { "" }

az deployment group create          `
    --resource-group $resourceGroup `
    --template-file $templateFile   `
    --parameters $parametersFile    `
    --parameters $configFile        `
    --parameters resourceGroupName=$resourceGroup `
    $extraParam

Write-Output "Web API resources have been provisioned."

# Assign roles to the Web App managed identity
$parameters = Get-Content $parametersPath | ConvertFrom-Json
$webAppName = $parameters.parameters.name.value
$vmResourceGroup = $parameters.parameters.vmResourceGroupName.value
$subscriptionId = $parameters.parameters.subscriptionId.value

$principalId = az webapp show --name $webAppName --resource-group $resourceGroup --query identity.principalId -o tsv

if (-not [string]::IsNullOrEmpty($principalId)) {
    $scope = "/subscriptions/$subscriptionId/resourceGroups/$vmResourceGroup"
    az role assignment create --assignee $principalId --role "Virtual Machine Contributor" --scope $scope | Out-Null
    az role assignment create --assignee $principalId --role "Reader" --scope $scope | Out-Null
    Write-Output "Role assignments configured for managed identity." 
} else {
    Write-Warning "Could not determine managed identity principal ID."
}
