param(
    [Parameter(Mandatory=$true)]
    [string]$FunctionAppName,
    
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [string]$Configuration = "Release",
    [string]$FunctionsProjectPath = "../ServerManagement.Functions",
    [string]$SettingsFile = "../ServerManagement.Functions/prod.settings.json"
)

# Resolve the project path relative to this script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Resolve-Path (Join-Path $scriptDir $FunctionsProjectPath)
$settingsPath = Resolve-Path (Join-Path $scriptDir $SettingsFile)

Write-Host "Publishing Functions project at $projectPath using configuration $Configuration" -ForegroundColor Green

# Check if settings file exists
if (-not (Test-Path $settingsPath)) {
    Write-Error "Settings file not found at: $settingsPath"
    exit 1
}

# Build and publish the functions project
$publishDir = Join-Path $projectPath "bin\$Configuration\net8.0\publish"
dotnet publish $projectPath -c $Configuration -o $publishDir

# Deploy to Azure
Write-Host "Deploying to Azure Function App: $FunctionAppName" -ForegroundColor Yellow
Push-Location $publishDir
try {
    func azure functionapp publish $FunctionAppName --dotnet-isolated
} finally {
    Pop-Location
}

# Load settings from prod.settings.json
Write-Host "Loading settings from $settingsPath" -ForegroundColor Yellow
$settings = Get-Content $settingsPath | ConvertFrom-Json

# Convert settings to Azure format (exclude system settings)
$appSettings = @()
foreach ($key in $settings.Values.PSObject.Properties.Name) {
    if ($key -notlike "AzureWebJobs*" -and $key -ne "FUNCTIONS_WORKER_RUNTIME") {
        $value = $settings.Values.$key
        # Escape special characters for Azure CLI
        $escapedValue = $value -replace '"', '\"'
        $appSettings += "$key=$escapedValue"
    }
}

# Update Function App settings
if ($appSettings.Count -gt 0) {
    Write-Host "Updating Function App settings..." -ForegroundColor Yellow
    
    # Build the arguments array
    $azArgs = @(
        "functionapp", "config", "appsettings", "set",
        "--name", $FunctionAppName,
        "--resource-group", $ResourceGroupName,
        "--settings"
    )
    
    # Add each setting as a separate argument
    foreach ($setting in $appSettings) {
        $azArgs += $setting
    }
    
    # Execute the command
    & az $azArgs
    
    Write-Host "Application settings updated successfully!" -ForegroundColor Green
}