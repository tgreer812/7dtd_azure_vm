# Deploy VM ARM Template

$azureDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$armDir = Resolve-Path (Join-Path $azureDir "\Arm\VM")
$configFile = Resolve-Path (Join-Path $armDir "private_parameters.json")
$templateFile = Resolve-Path (Join-Path $armDir "template.json")
$parametersFile = "@" + (Resolve-Path (Join-Path $armDir "parameters.json")).Path
$resourceGroup = "tg-7d2d-dedicated"

Write-Output "ARM Directory: $armDir"
Write-Output "Template File: $templateFile"
Write-Output "Parameters File: $parametersFile"

if (-not (Test-Path $configFile)) {
    Write-Error "A private configuration file named 'private_parameters.json' is expected at: $configFile"
    exit 1
}

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
    --parameters $configFile        `
    --parameters resourceGroupName=$resourceGroup

Write-Output "Resources have been provisioned. Server may still be installing in the background"
az network public-ip show --resource-group $resourceGroup --name 7dtd-pip --query "{fqdn:dnsSettings.fqdn,address: ipAddress}"
