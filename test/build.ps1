Param(
    [string]$Script = "build.cake",
    [string]$Target = "Default",

    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release",
    
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity = "Verbose",

    [switch]$Experimental,
    [switch]$WhatIf,
    [switch]$Mono,
    [switch]$SkipToolPackageRestore
)



# Find tools
Write-Host "Using local tools"
$PSScriptRoot = split-path -parent $MyInvocation.MyCommand.Definition
$TOOLS_DIR = Join-Path $PSScriptRoot "tools"

if (!(Test-Path $TOOLS_DIR)) 
{
    Write-Host "Creating tools directory"
    New-Item $TOOLS_DIR -itemtype directory
}



# Define Paths
$CAKE_EXE = Join-Path $TOOLS_DIR "Cake/Cake.exe"

$NUGET_URL = "https://nuget.org/nuget.exe"
$NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"

$PACKAGES_CONFIG = Join-Path $TOOLS_DIR "packages.config"



# Save paths to environment for use in child processes
$ENV:PATH = $TOOLS_DIR
$ENV:NUGET_EXE = $NUGET_EXE



# Should we use experimental build of Roslyn?
$UseExperimental = "";
if($Experimental.IsPresent) 
{
    $UseExperimental = "-experimental"
}

# Is this a dry run?
$UseDryRun = "";
if($WhatIf.IsPresent) 
{
    $UseDryRun = "-dryrun"
}

# Should we use mono?
$UseMono = "";
if($Mono.IsPresent) 
{
    $UseMono = "-mono"
}



# Try download NuGet.exe if it does not exist.
if (!(Test-Path $NUGET_EXE)) 
{
    Write-Host "Downloading Nuget"
    (New-Object System.Net.WebClient).DownloadFile($NUGET_URL, $NUGET_EXE)
}

# Make sure NuGet exists where we expect it.
if (!(Test-Path $NUGET_EXE)) 
{
    Throw "Could not find NuGet.exe"
}



# Restore tools from NuGet
if (-Not $SkipToolPackageRestore.IsPresent)
{
    Push-Location
    Set-Location $TOOLS_DIR

    if (Test-Path $PACKAGES_CONFIG)
    {
        # Restore tools from config
        Invoke-Expression "$NUGET_EXE install -ExcludeVersion"
    }
    else
    {
        # Install just Cake if missing config
        Invoke-Expression "$NUGET_EXE install Cake -ExcludeVersion"
    }

    Pop-Location
    if ($LASTEXITCODE -ne 0) 
    {
        exit $LASTEXITCODE
    }
}

# Make sure that Cake has been installed.
if (!(Test-Path $CAKE_EXE)) 
{
    Throw "Could not find Cake.exe"
}



# Start Cake
Invoke-Expression "$CAKE_EXE `"$Script`" -tools=`"$TOOLS_DIR`" -target=`"$Target`" -configuration=`"$Configuration`" -custom=`"$Custom`" -verbosity=`"$Verbosity`" $UseMono $UseDryRun $UseExperimental"
exit $LASTEXITCODE