Install-Module Newtonsoft.Json
Import-Module Newtonsoft.Json

Remove-Item 'UmbracoTemplates' -Recurse -ErrorAction Ignore
Remove-Item 'Core' -Recurse -ErrorAction Ignore
Remove-Item 'Standalone' -Recurse -ErrorAction Ignore
Remove-Item 'Web' -Recurse -ErrorAction Ignore

$UmbracoVersion = "9.0.1"
$UmbracoFormsVersion = "9.0.1"
$YuzuDeliveryCoreVersion = "1.0.49.4"
$YuzuDeliveryImportVersion = "1.0.98.39"
$YuzuDeliveryUmbracoVersion = "1.2.40.51"

$author = "Hi-Fi Ltd"

$groupIdentityStandalone    = "YuzuDelivery.Templates"
$descriptionStandalone      = "Standalone web project for Umbraco Yuzu delivery"
$identityStandalone         = "YuzuDelivery.Umbraco.Templates.CSharp"
$nameStandalone             = "Yuzu Delivery"
$shortNameStandalone        = "yuzu-delivery"
$defaultNameStandalone      = "YuzuDelivery1"

$groupIdentityCore           = "YuzuDelivery.Templates.Core"
$descriptionCore             = "Core only project for Umbraco Yuzu delivery"
$identityCore               = "YuzuDelivery.Umbraco.Templates.CSharp.Core"
$nameCore                    = "Yuzu Delivery Core"
$shortNameCore               = "yuzu-delivery-core"
$defaultNameCore             = "YuzuDelivery.Core"

$groupIdentityWeb           = "YuzuDelivery.Templates.Web"
$descriptionWeb             = "Web only project for Umbraco Yuzu delivery"
$identityWeb                = "YuzuDelivery.Umbraco.Templates.CSharp.Web"
$nameWeb                    = "Yuzu Delivery Web"
$shortNameWeb               = "yuzu-delivery-web"
$defaultNameWeb             = "YuzuDelivery.Web1"


#Download Umbraco.Templates nuget package
New-Item -Path ".\" -Name "UmbracoTemplates" -ItemType "directory"
Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/Umbraco.Templates/$($UmbracoVersion)" -OutFile (".\UmbracoTemplates\umbraco.templates.nupkg.zip")

#unarchive nuget package
New-Item -Path ".\UmbracoTemplates" -Name "unarchived" -ItemType "directory"
Expand-Archive -LiteralPath ".\UmbracoTemplates\umbraco.templates.nupkg.zip" -DestinationPath (".\UmbracoTemplates\unarchived")

function Copy-Template {
    param (
        $Folder
    )

    #Copy Umbraco Project from nuget archive to specfic project
    New-Item -Path ".\" -Name $folder -ItemType "directory"
    Copy-Item -Path ".\UmbracoTemplates\unarchived\UmbracoProject\*" -Destination ".\$($folder)" -Recurse
}

function Update-ViewImports {
    param (
        $Folder
    )

    $ViewImports = Get-Content ".\$($folder)\Views\_ViewImports.cshtml", ".\Yuzu\_ViewImports.cshtml"
    Set-Content ".\$($folder)\Views\_ViewImports.cshtml" $ViewImports
}

function Copy-Icon {
    param (
        $Folder
    )

    Copy-Item -Path ".\Yuzu\Icon\icon.png" -Destination ".\$($folder)\.template.config\icon.png"
}

function Copy-Yuzu-Composer {
    param (
        $Folder
    )

    $pathToYuzuDir = ".\$($folder)\Yuzu"

    Copy-Item -Path ".\Yuzu\Startup" -Destination $pathToYuzuDir -Recurse
}

function Copy-Yuzu-Core {

    Copy-Item -Path ".\Yuzu\Core\" -Destination ".\Core" -Recurse
}

function Update-ModeslBuilder {
    param (
        $Folder,
        [bool] $isWeb
    )

    $AppSettings = Get-Content ".\$($folder)\appsettings.Development.json"
    $toFind = '"Hosting'

    if($isWeb) {
        $replaceWith = '"ModelsBuilder": {
            "ModelsMode": "SourceCodeManual",
            "ModelsNamespace": "YuzuDelivery.Umbraco",
            "AcceptUnsafeModelsDirectory": "true",
            "ModelsDirectory": "~/../Umbraco.Cms.Web.UI/UmbracoModels"
        },
        "Hosting' 
    }
    else {
        $replaceWith = '"ModelsBuilder": {
            "ModelsMode": "SourceCodeManual",
            "ModelsNamespace": "YuzuDelivery.Umbraco"
        },
        "Hosting' 
    }
 

    $AppSettings = $AppSettings -replace $toFind, $replaceWith
    Set-Content ".\$($folder)\appsettings.Development.json" $AppSettings
}

# Formats JSON in a nicer format than the built-in ConvertTo-Json does.
function Format-Json([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
  $indent = 0;
  ($json -Split '\n' |
    % {
      if ($_ -match '[\}\]]') {
        # This line contains  ] or }, decrement the indentation level
        $indent--
      }
      $line = (' ' * $indent * 2) + $_.TrimStart().Replace(':  ', ': ')
      if ($_ -match '[\{\[]') {
        # This line contains [ or {, increment the indentation level
        $indent++
      }
      $line
  }) -Join "`n"
}

function Add-Yuzu-AppSettings {
    param (
        $Folder,
        [bool] $isWeb
    )

    $AppSettings = Get-Content -Path ".\$($folder)\appsettings.Development.json" -Raw

    $AppSettings = $AppSettings.Trim()
    $AppSettings = $AppSettings.Substring(0, $AppSettings.Length-3)

    $yuzu = [PSCustomObject]@{
        Yuzu = [PSCustomObject]@{
            Core = [PSCustomObject]@{ 
                Pages = "/yuzu/_templates/src/pages"
                Partials = "/yuzu/_templates/src/blocks"
                SchemaMeta = "/yuzu/_templates/paths"
            }
            VmGeneration = [PSCustomObject]@{ 
                IsActive = $True 
                AcceptUnsafeDirectory = $False 
                Directory = "~/yuzu/viewmodels"
            }
            Import = [PSCustomObject]@{ 
                IsActive = $True 
                Config = "/yuzu/YuzuConfig.json"
                ManualMappingDirectory = "/yuzu/Mappings/"
                Data = "/wwwroot/_client/images"
                Images = "/yuzu/_templates/data"
            }
        }
    }

    if($isWeb) {
        $yuzu.Yuzu.VmGeneration.AcceptUnsafeDirectory = $True 
        $yuzu.Yuzu.VmGeneration.Directory = "~/../Umbraco.Cms.Web.UI/ViewModels"
    }

    $yuzuString = ConvertTo-Json $yuzu | Format-Json
    $yuzuString = $yuzuString.Substring(1, $yuzuString.Length-1) 

    $output = "$($AppSettings),", $yuzuString

    Set-Content ".\$($folder)\appsettings.Development.json" $output
}

function Copy-Forms-Partials {
    param (
        $Folder,
        [bool] $isCore
    )

    Copy-Item -Path "..\Forms\Web\UI\Views\Partials\*" -Destination ".\$($folder)\Views\Partials" -Recurse
}

function Add-Project-Dependencies {
    param (
        $Folder,
        [bool] $isCore
    )

    $pathToConfig = ".\$($folder)\UmbracoProject.csproj"

    $csproj = [xml](get-content $pathToConfig)

    if($isCore) {
        $newNode = [xml]"
            <ItemGroup>
                <PackageReference Include='Umbraco.Forms.Core' Version='$($UmbracoFormsVersion)' />
                <PackageReference Include='YuzuDelivery.Core' Version='$($YuzuDeliveryCoreVersion)' />
                <PackageReference Include='YuzuDelivery.Import' Version='$($YuzuDeliveryImportVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Import.Core' Version='$($YuzuDeliveryImportVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Core' Version='$($YuzuDeliveryUmbracoVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Forms' Version='$($YuzuDeliveryUmbracoVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Members' Version='$($YuzuDeliveryUmbracoVersion)' />
            </ItemGroup>"
    }
    else {
        $newNode = [xml]"
            <ItemGroup>
                <PackageReference Include='Umbraco.Forms' Version='$($UmbracoFormsVersion)' />
                <PackageReference Include='YuzuDelivery.Core' Version='$($YuzuDeliveryCoreVersion)' />
                <PackageReference Include='YuzuDelivery.Import' Version='$($YuzuDeliveryImportVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Import' Version='$($YuzuDeliveryImportVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Core' Version='$($YuzuDeliveryUmbracoVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Forms' Version='$($YuzuDeliveryUmbracoVersion)' />
                <PackageReference Include='YuzuDelivery.Umbraco.Members' Version='$($YuzuDeliveryUmbracoVersion)' />
            </ItemGroup>"
    }

    $newNode = $csproj.ImportNode($newNode.ItemGroup, $true)
    $csproj.Project.AppendChild($newNode) | out-null

    $csproj.Save("$($PSScriptRoot)$($pathToConfig)")
}

function Update-Template-Meta {
    param (
        $Folder,
        [bool] $isCore,
        [bool] $isWeb
    )

    $pathToTemplateJson = ".\$($folder)\.template.config\template.json"

    $json = get-content $pathToTemplateJson

    $config = [Newtonsoft.Json.Linq.JObject]::Parse($json) # parse string

    $config["author"].Value = $author

    if($isCore) {
        $config["symbols"]["version"]["defaultValue"].Value = $UmbracoVersion

        $config["groupIdentity"].Value = $groupIdentityCore
        $config["description"].Value = $descriptionCore
        $config["identity"].Value = $identityCore
        $config["name"].Value = $nameCore
        $config["shortName"].Value = $shortNameCore
        $config["shortName"].Value = $shortNameCore
        $config["defaultName"].Value = $defaultNameCore
    }
    elseif($isWeb) {
        $config["groupIdentity"].Value = $groupIdentityWeb
        $config["description"].Value = $descriptionWeb
        $config["identity"].Value = $identityWeb
        $config["name"].Value = $nameWeb
        $config["shortName"].Value = $shortNameWeb
        $config["shortName"].Value = $shortNameWeb
        $config["defaultName"].Value = $defaultNameWeb
    }
    else {
        $config["groupIdentity"].Value = $groupIdentityStandalone
        $config["description"].Value = $descriptionStandalone
        $config["identity"].Value = $identityStandalone
        $config["name"].Value = $nameStandalone
        $config["shortName"].Value = $shortNameStandalone
        $config["shortName"].Value = $shortNameStandalone
        $config["defaultName"].Value = $defaultNameStandalone
    }

    $output = [Newtonsoft.Json.JsonConvert]::SerializeObject($config)

    Set-Content $pathToTemplateJson $output

}

Copy-Template -folder 'Web'
Copy-Template -folder 'Standalone'
Copy-Yuzu-Core

Update-ViewImports -folder 'Web'
Update-ViewImports -folder 'Standalone'

Copy-Yuzu-Composer 'Core'
Copy-Yuzu-Composer 'Standalone'

Update-ModeslBuilder -folder 'Web' -isWeb $True 
Update-ModeslBuilder -folder 'Standalone' -isWeb $False 

Add-Yuzu-AppSettings -folder 'Web' -isWeb $True
Add-Yuzu-AppSettings -folder 'Standalone' -isWeb $False

Copy-Forms-Partials -folder 'Web'
Copy-Forms-Partials -folder 'Standalone'

Copy-Icon -folder 'Core'
Copy-Icon -folder 'Web'
Copy-Icon -folder 'Standalone'

Add-Project-Dependencies 'Core' -isCore $True
Add-Project-Dependencies 'Web' -isCore $False
Add-Project-Dependencies 'Standalone' -isCore $False

Update-Template-Meta 'Core' -isWeb $True -isCore $True
Update-Template-Meta 'Web' -isWeb $True -isCore $False
Update-Template-Meta 'Standalone' -isWeb $False -isCore $False

# Create Core Template
# Test template
