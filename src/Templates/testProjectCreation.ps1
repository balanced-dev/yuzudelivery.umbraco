function Reset-Templates{
    [cmdletbinding()]
    param(
        [string]$templateEngineUserDir = (join-path -Path $env:USERPROFILE -ChildPath .templateengine)
    )
    process{
        'resetting dotnet new templates. folder: "{0}"' -f $templateEngineUserDir | Write-host
        get-childitem -path $templateEngineUserDir -directory | Select-Object -ExpandProperty FullName | remove-item -recurse
        &dotnet new --debug:reinit
    }
}

$UmbracoVersion = "9.0.1"
$UmbracoFormsVersion = "9.0.1"
$YuzuDeliveryCoreVersion = "1.0.49.4"
$YuzuDeliveryImportVersion = "1.0.98.41"
$YuzuDeliveryUmbracoVersion = "1.2.40.55"

./buildTemplates.ps1 -UmbracoVersion $UmbracoVersion -UmbracoFormsVersion $UmbracoFormsVersion -YuzuDeliveryCoreVersion $YuzuDeliveryCoreVersion -YuzuDeliveryImportVersion $YuzuDeliveryImportVersion -YuzuDeliveryUmbracoVersion $YuzuDeliveryUmbracoVersion 

Remove-Item '.\TestOutput\Standalone' -Recurse -Force -ErrorAction Ignore
Remove-Item '.\TestOutput\Core' -Recurse -Force -ErrorAction Ignore
Remove-Item '.\TestOutput\Web' -Recurse -Force -ErrorAction Ignore

New-Item -Path ".\TestOutput\" -Name "Standalone" -ItemType "directory"
New-Item -Path ".\TestOutput\" -Name "Core" -ItemType "directory"
New-Item -Path ".\TestOutput\" -Name "Web" -ItemType "directory"

Reset-Templates
Copy-Item -Path ".\nuget.config" -Destination ".\TestOutput\Standalone" -Recurse
Copy-Item -Path ".\nuget.config" -Destination ".\TestOutput\Core" -Recurse
Copy-Item -Path ".\nuget.config" -Destination ".\TestOutput\Web" -Recurse

dotnet new --install .\Templates\Standalone\ 
dotnet new yuzu-delivery -o .\TestOutput\Standalone

dotnet new --install .\Templates\Core\
dotnet new yuzu-delivery-core -o .\TestOutput\Core

dotnet new --install .\Templates\Web\
dotnet new yuzu-delivery-web -o .\TestOutput\Web