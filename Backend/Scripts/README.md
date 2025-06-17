param(
    [Parameter(Mandatory=$true)]
    [string]$FunctionAppName,
    [string]$Configuration = "Release",
    [string]$FunctionsProjectPath = "../ServerManagement.Functions"
)

# Resolve the project path relative to this script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Resolve-Path (Join-Path $scriptDir $FunctionsProjectPath)

Write-Host "Publishing Functions project at $projectPath using configuration $Configuration"

# Build and publish the functions project
$publishDir = Join-Path $projectPath "bin\$Configuration\net8.0\publish"
dotnet publish $projectPath -c $Configuration -o $publishDir

# Deploy to Azure
Write-Host "Deploying to Azure Function App: $FunctionAppName"
# Change to the publish directory where host.json should be located
Push-Location $publishDir
try {
    func azure functionapp publish $FunctionAppName --dotnet-isolated --publish-local-settings -i
} finally {
    Pop-Location
}