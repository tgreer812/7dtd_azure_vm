# Deploy Azure Functions ARM Template

$azureDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$armDir = Resolve-Path (Join-Path $azureDir "\Arm\WebApi")
$templateFile = Resolve-Path (Join-Path $armDir "template.json")
$parametersFile = "@" + (Resolve-Path (Join-Path $armDir "parameters.json")).Path
$resourceGroup = "tg-7d2d-dedicated-test"

Write-Output "ARM Directory: $armDir"
Write-Output "Template File: $templateFile"
Write-Output "Parameters File: $parametersFile"

# Save a list of all existing resource groups
$resGroups = az group list --query "[].name" -o tsv

# Check if the resource group exists
$exists = $resGroups -contains $resourceGroup

if (-not $exists) {
    Write-Output "Resource group '$resourceGroup' does not exist. Creating..."
    az group create --name $resourceGroup --location "eastus"
} else {
    Write-Output "Resource group '$resourceGroup' already exists."
}

az deployment group create          `
    --resource-group $resourceGroup `
    --template-file $templateFile   `
    --parameters $parametersFile    `
    --parameters resourceGroupName=$resourceGroup

Write-Output "Web API resources have been provisioned."
