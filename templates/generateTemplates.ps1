param(
    [string] $UmbracoVersion = "10.2.0",
    [string] $UmbracoFormsVersion = "10.1.2",
    [string] $YuzuDeliveryUmbracoVersion = $null
)

if ($null -eq (Get-Command "nbgv" -ErrorAction SilentlyContinue))
{
    dotnet tool install -g nbgv
}

if(-Not $YuzuDeliveryUmbracoVersion)
{
    $YuzuDeliveryUmbracoVersion = (nbgv get-version -v NuGetPackageVersion)
}

if($YuzuDeliveryUmbracoVersion -match "(.*)\-(.*?)\.")
{
    # Pre-release hack - prevent csproj changing on generate for every single commit (as heigh + sha change every commit)
    $short = $matches[0]
    $YuzuDeliveryUmbracoVersion = "$short*"
}


Write-Host "###################################################################"
Write-Host "# Umbraco Version:       $UmbracoVersion"
Write-Host "# Forms Version:         $UmbracoFormsVersion"
Write-Host "# Yuzu Delivery Version: $YuzuDeliveryUmbracoVersion"
Write-Host "###################################################################"

# Set current working directory to location of this file
Push-Location -Path $PSScriptRoot

# Clean up from previous runs
Remove-Item '.tmp' -Recurse -ErrorAction Ignore
Remove-Item 'standalone' -Recurse -ErrorAction Ignore
Remove-Item 'testproject' -Recurse -ErrorAction Ignore

Install-Module Newtonsoft.Json -Scope CurrentUser -Force
Import-Module Newtonsoft.Json

# Import all the utility functions used below.
Import-Module ./utils.psm1 -Force

# Download Umbraco.Templates nuget package
mkdir .tmp | Out-Null
Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/Umbraco.Templates/$($UmbracoVersion)" -OutFile (".tmp\umbraco.templates.$($UmbracoVersion).nupkg.zip")
Expand-Archive -LiteralPath ".tmp\umbraco.templates.$($UmbracoVersion).nupkg.zip" -DestinationPath (".tmp\umbraco_templates")

###########################################################
## Generate standalone
###########################################################

Copy-Upstream-Template -folder 'standalone'
Update-ViewImports -folder 'standalone'
Copy-Yuzu-Folder 'standalone'
Update-ModeslBuilder -folder 'standalone' -isWeb $False
Add-Yuzu-AppSettings -folder 'standalone'
Add-Yuzu-Development-AppSettings -folder 'standalone' -isWeb $False
Copy-Forms-Partials -folder 'standalone'
Copy-Blocklist-Partials -folder 'standalone'
Copy-Icon -folder 'standalone'

$dependencies = [ordered]@{
    "Umbraco.Forms" = $UmbracoFormsVersion;
    "YuzuDelivery.Umbraco" = $YuzuDeliveryUmbracoVersion;
}

$properties = @{
    "author" = "Hi-Fi Ltd";
    "description" = "Standalone web project for Umbraco Yuzu delivery";
    "identity" = "YuzuDelivery.Templates.Standalone";
    "groupIdentity" = "YuzuDelivery.Templates.Standalone";
    "name" = "Yuzu Delivery: Standalone";
    "shortName" = "yuzu-standalone";
    "defaultName" = "YuzuDelivery1";
    "classifications" = @("Yuzu", "Umbraco", "CMS")
}

Add-Project-Dependencies-Simple -folder 'standalone' -dependencies $dependencies
Update-Template-Meta-Simple 'standalone' -properties $properties
xcopy /s /q '.\src\standalone' '.\standalone' | Out-Null
Write-Host "Created standalone"

###########################################################
## Generate testproject
###########################################################

Copy-Upstream-Template -folder 'testproject'
Update-ViewImports -folder 'testproject'
Copy-Yuzu-Folder 'testproject'
Update-ModeslBuilder -folder 'testproject' -isWeb $False
Add-Yuzu-AppSettings -folder 'testproject'
Add-Yuzu-Development-AppSettings -folder 'testproject' -isWeb $False
Copy-Forms-Partials -folder 'testproject'
Copy-Blocklist-Partials -folder 'testproject'
Copy-Icon -folder 'testproject'
$dependencies = [ordered]@{
    "Umbraco.Forms" = $UmbracoFormsVersion;
    "YuzuDelivery.Umbraco" = $YuzuDeliveryUmbracoVersion;
    "Our.Umbraco.TheDashboard" = "10.0.1";
}
Add-Project-Dependencies-Simple -folder 'testproject' -dependencies $dependencies
$properties = @{
    "author" = "Hi-Fi Ltd";
    "description" = "Test web project for Umbraco Yuzu delivery";
    "identity" = "YuzuDelivery.Templates.TestProject";
    "groupIdentity" = "YuzuDelivery.Templates.TestProject";
    "name" = "Yuzu Delivery: Test Project";
    "shortName" = "yuzu-test";
    "defaultName" = "YuzuDelivery1";
    "classifications" = @("Yuzu", "Umbraco", "CMS")
}
Add-Forced-Restart-Watcher -folder 'testproject'
Update-Template-Meta-Simple -folder 'testproject' -properties $properties
xcopy /s /q '.\src\testproject' '.\testproject' | Out-Null
Write-Host "Created testproject"

Pop-Location