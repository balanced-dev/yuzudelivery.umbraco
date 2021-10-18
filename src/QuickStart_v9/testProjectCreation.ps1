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

./getUmbracoTemplate.ps1

Remove-Item '.\TestOutput\Core' -Recurse -ErrorAction Ignore
Remove-Item '.\TestOutput\Standalone' -Recurse -ErrorAction Ignore
Remove-Item '.\TestOutput\Web' -Recurse -ErrorAction Ignore

New-Item -Path ".\TestOutput\" -Name "Standalone" -ItemType "directory"
New-Item -Path ".\TestOutput\" -Name "Core" -ItemType "directory"
New-Item -Path ".\TestOutput\" -Name "Web" -ItemType "directory"

Reset-Templates
Copy-Item -Path ".\nuget.config" -Destination ".\TestOutput\Standalone" -Recurse
Copy-Item -Path ".\nuget.config" -Destination ".\TestOutput\Core" -Recurse
Copy-Item -Path ".\nuget.config" -Destination ".\TestOutput\Web" -Recurse

dotnet new --install .\Standalone\
dotnet new yuzu-delivery -o .\TestOutput\Standalone

dotnet new --install .\Core\
dotnet new yuzu-delivery-core -o .\TestOutput\Core

dotnet new --install .\Web\
dotnet new yuzu-delivery-web -o .\TestOutput\Web