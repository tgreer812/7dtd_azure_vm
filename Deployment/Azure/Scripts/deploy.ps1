$azureDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$armDir = Resolve-Path (Join-Path $azureDir "\Arm")
$configDir = Resolve-Path (Join-Path $azureDir "\Config")
$templateFile = Resolve-Path (Join-Path $armDir "template.json")
$globalConfigFile = Resolve-Path (Join-Path $configDir "global_config.json")
$parametersFile = "@" + (Resolve-Path (Join-Path $armDir "parameters.json")).Path
$resourceGroup = "tg-7d2d-dedicated"

Write-Output "ARM Directory: $armDir"
Write-Output "Template File: $templateFile"
Write-Output "Parameters File: $parametersFile"

if (-not (Test-Path $globalConfigFile)) {
    Write-Error "A global configuration file named 'global_config.json' is expected at: $globalConfigFile"
    exit 1
}

az deployment group create `
    --resource-group $resourceGroup `
    --template-file $templateFile `
    --parameters $parametersFile
