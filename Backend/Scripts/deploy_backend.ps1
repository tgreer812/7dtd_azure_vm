param(
    [Parameter(Mandatory=$true)]
    [string]$FunctionAppName,
    [string]$Configuration = "Release",
    [switch]$ProvisionInfrastructure
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..\..")

if ($ProvisionInfrastructure.IsPresent) {
    $infraScript = Join-Path $repoRoot "Deployment/Azure/Scripts/deploy_functions.ps1"
    Write-Host "Provisioning Azure resources via $infraScript"
    & $infraScript
}

$publishScript = Join-Path $scriptDir "publish_functions.ps1"
Write-Host "Publishing Function App $FunctionAppName using $publishScript"
& $publishScript -FunctionAppName $FunctionAppName -Configuration $Configuration

