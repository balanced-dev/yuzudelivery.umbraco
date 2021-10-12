Remove-Item 'UmbracoTemplates' -Recurse -ErrorAction Ignore
Remove-Item 'Standalone' -Recurse -ErrorAction Ignore
Remove-Item 'Web' -Recurse -ErrorAction Ignore

$UmbracoVersion = "9.0.0"
$UmbracoFormsVersion = "9.0.0"
$YuzuDeliveryCoreVersion = "1.0.48"
$YuzuDeliveryImportVersion = "1.0.96"
$YuzuDeliveryUmbracoVersion = "1.0.22"

$author = "Hi-Fi Ltd"
$groupIdentity = "YuzuDelivery.Templates"

$descriptionStandalone = "Standalone web project for Umbraco Yuzu delivery"
$identityStandalone = "YuzuDelivery.Umbraco.Templates.CSharp"
$nameStandalone = "Yuzu Delivery"
$shortNameStandalone = "yuzu-delivery"
$defaultNameStandalone = "YuzuDelivery1"

$descriptionWeb = "Web only project for Umbraco Yuzu delivery"
$identityWeb = "YuzuDelivery.Umbraco.Templates.Web.CSharp"
$nameWeb = "Yuzu Delivery Web"
$shortNameWeb = "yuzu-delivery-web"
$defaultNameWeb = "YuzuDelivery.Web1"


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

    $ViewImports = Get-Content ".\$($folder)\Views\_ViewImports.cshtml", ".\Yuzu\StandaloneContent\_ViewImports.cshtml"
    Set-Content ".\$($folder)\Views\_ViewImports.cshtml" $ViewImports
}

function Update-ModeslBuilder {
    param (
        $Folder,
        [bool] $isCore
    )

    $AppSettings = Get-Content ".\$($folder)\appsettings.Development.json"
    $toFind = '"Hosting'

    if($isCore) {
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
        [bool] $isCore
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
                Directory = "/yuzu/YuzuConfig.json"
                ManualMappingDirectory = "/yuzu/Mappings/"
                Data = "/wwwroot/_client/images"
                Images = "/yuzu/_templates/data"
            }
        }
    }

    if($isCore) {
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
        $Folder
    )

    $pathToConfig = ".\$($folder)\UmbracoProject.csproj"

    $csproj = [xml](get-content $pathToConfig)

    $newNode = [xml]"
        <ItemGroup>
            <PackageReference Include='Umbraco.Cms.Forms' Version='$($UmbracoFormsVersion)' />
            <PackageReference Include='YuzuDelivery.Core' Version='$($YuzuDeliveryCoreVersion)' />
            <PackageReference Include='YuzuDelivery.Import' Version='$($YuzuDeliveryImportVersion)' />
            <PackageReference Include='YuzuDelivery.Umbraco.Import' Version='$($YuzuDeliveryImportVersion)' />
            <PackageReference Include='YuzuDelivery.Umbraco.Core' Version='$($YuzuDeliveryUmbracoVersion)' />
            <PackageReference Include='YuzuDelivery.Umbraco.Forms' Version='$($YuzuDeliveryUmbracoVersion)' />
            <PackageReference Include='YuzuDelivery.Umbraco.Members' Version='$($YuzuDeliveryUmbracoVersion)' />
        </ItemGroup>"

    $newNode = $csproj.ImportNode($newNode.ItemGroup, $true)
    $csproj.Project.AppendChild($newNode) | out-null

    $csproj.Save("$($PSScriptRoot)$($pathToConfig)")
}

function Update-Template-Meta {
    param (
        $Folder,
        [bool] $isCore
    )

    $pathToTemplateJson = ".\$($folder)\.template.config\template.json"

    $templateJson = get-content $pathToTemplateJson ` | ConvertFrom-Json

    $templateJson.author = $author
    $templateJson.groupIdentity = $groupIdentity

    if($isCore) {
        $templateJson.description = $descriptionWeb
        $templateJson.identity = $identityWeb
        $templateJson.name = $nameWeb
        $templateJson.shortName = $shortNameWeb
        $templateJson.defaultName = $defaultNameWeb
    }
    else {
        $templateJson.description = $descriptionStandalone
        $templateJson.identity = $identityStandalone
        $templateJson.name = $nameStandalone
        $templateJson.shortName = $shortNameStandalone
        $templateJson.defaultName = $defaultNameStandalone
    }

    $output = ConvertTo-Json $templateJson | Format-Json

    Set-Content $pathToTemplateJson $output

}

Copy-Template -folder 'Web'
Copy-Template -folder 'Standalone'

Update-ViewImports -folder 'Web'
Update-ViewImports -folder 'Standalone'

Update-ModeslBuilder -folder 'Web' -isCore $True 
Update-ModeslBuilder -folder 'Standalone' -isCore $False 

Add-Yuzu-AppSettings -folder 'Web' -isCore $True
Add-Yuzu-AppSettings -folder 'Standalone' -isCore $False

Copy-Forms-Partials -folder 'Web'
Copy-Forms-Partials -folder 'Standalone'

Add-Project-Dependencies 'Web'
Add-Project-Dependencies 'Standalone'

Update-Template-Meta 'Web' -isCore $True
Update-Template-Meta 'Standalone' -isCore $False

# Create Core Template
# Test template
