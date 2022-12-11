function Copy-Upstream-Template {
    param (
        $Folder
    )

    # Copy Umbraco Project from nuget archive to specfic project
    New-Item -Path ".\" -Name $folder -ItemType "directory" | Out-Null
    Copy-Item -Path ".tmp\umbraco_templates\UmbracoProject\*" -Destination ".\$($folder)" -Recurse
}

function Update-ViewImports {
    param (
        $Folder
    )

    $ViewImports = Get-Content ".\$($folder)\Views\_ViewImports.cshtml", ".\src\common\Views\_ViewImports.cshtml"
    Set-Content ".\$($folder)\Views\_ViewImports.cshtml" $ViewImports
}

function Copy-Icon {
    param (
        $Folder
    )

    Copy-Item -Path ".\src\common\.template.config\icon.png" -Destination ".\$($folder)\.template.config\icon.png"
}

function Copy-Yuzu-Folder {
    param (
        $Folder
    )

    $pathToYuzuDir = ".\$($folder)\Yuzu"

    Copy-Item -Path ".\src\common\Yuzu" -Destination $pathToYuzuDir -Recurse
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
            "ModelsNamespace": "YuzuDelivery.UmbracoModels",
            "AcceptUnsafeModelsDirectory": true,
            "ModelsDirectory": "~/../CORE_NAMESPACE/UmbracoModels"
        },
        "Hosting' 
    }
    else {
        $replaceWith = '"ModelsBuilder": {
            "ModelsMode": "SourceCodeManual",
            "ModelsNamespace": "YuzuDelivery.UmbracoModels"
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
        $Folder
    )

    $AppSettings = Get-Content -Path ".\$($folder)\appsettings.json" -Raw

    $AppSettings = $AppSettings.Trim()
    $AppSettings = $AppSettings.Substring(0, $AppSettings.Length-2)

    $yuzu = [PSCustomObject]@{
        Yuzu = [PSCustomObject]@{
            Core = [PSCustomObject]@{ 
                Pages = "./Yuzu/_templates/src/pages"
                Partials = "./Yuzu/_templates/src/blocks"
                SchemaMeta = "./Yuzu/_templates/paths"
                ConfigPath = "./Yuzu/YuzuConfig.json"
            }
        }
    }

    $yuzuString = ConvertTo-Json $yuzu | Format-Json
    $yuzuString = $yuzuString.Substring(1, $yuzuString.Length-1) 

    $output = "$($AppSettings),", $yuzuString

    Set-Content ".\$($folder)\appsettings.json" $output
}


function Add-Yuzu-Development-AppSettings {
    param (
        $Folder,
        [bool] $isWeb
    )

    $AppSettings = Get-Content -Path ".\$($folder)\appsettings.Development.json" -Raw

    $AppSettings = $AppSettings.Trim()
    $AppSettings = $AppSettings.Substring(0, $AppSettings.Length-3)

    $yuzu = [PSCustomObject]@{
        Yuzu = [PSCustomObject]@{
            VmGeneration = [PSCustomObject]@{ 
                IsActive = $True 
                AcceptUnsafeDirectory = $False 
                Directory = "./Yuzu/ViewModels"
            }
            Import = [PSCustomObject]@{ 
                IsActive = $True 
                ManualMappingDirectory = "./Yuzu/Mappings"
                Data = "./Yuzu/_templates/data"
                ImagesDef = "/_client/images"
                ImagesDel = "./wwwroot/_client/images"
            }
        }
    }

    if($isWeb) {
        $yuzu.Yuzu.VmGeneration.AcceptUnsafeDirectory = $True 
        $yuzu.Yuzu.VmGeneration.Directory = "~/../CORE_NAMESPACE/ViewModels"
        $yuzu.Yuzu.Import.ManualMappingDirectory = "~/../CORE_NAMESPACE/yuzu/Mappings/"
    }

    $yuzuString = ConvertTo-Json $yuzu | Format-Json
    $yuzuString = $yuzuString.Substring(1, $yuzuString.Length-1) 

    $output = "$($AppSettings),", $yuzuString

    Set-Content ".\$($folder)\appsettings.Development.json" $output
}

function Add-Project-Dependencies-Simple {
    param (
        $Folder,
        [System.Collections.Specialized.OrderedDictionary] $dependencies
    )

    $pathToConfig = (Get-Item ".\$($folder)\UmbracoProject.csproj")

    $csproj = [xml](get-content $pathToConfig)

    foreach ($key in $dependencies.Keys) {
        $node = $csproj.CreateElement("PackageReference")
        $node.SetAttribute("Include", $key)
        $node.SetAttribute("Version", $dependencies[$key])
        $csproj.Project.ItemGroup[0].AppendChild($node) | Out-Null
    }

    $csproj.Save($pathToConfig.FullName)
}

function Add-Forced-Restart-Watcher {
    param (
        $Folder
    )

    $pathToConfig = (Get-Item ".\$($folder)\UmbracoProject.csproj")

    $csproj = [xml](get-content $pathToConfig)

    $node = $csproj.CreateElement("Watch")
    $node.SetAttribute("Include", "restart.txt")
    $csproj.Project.ItemGroup[0].AppendChild($node) | Out-Null

    $csproj.Save($pathToConfig.FullName)
}

function Update-Json {
    param (
        $Path,
        $Obj 
    )

    $output = [Newtonsoft.Json.JsonConvert]::SerializeObject($Obj)

    Set-Content $Path $output
}

function Update-Template-Meta-Simple {
    param (
        $Folder,
        [System.Collections.Hashtable] $properties
    )

    $pathToTemplateJson = ".\$($folder)\.template.config\template.json"
    $json = get-content $pathToTemplateJson
    $config = [Newtonsoft.Json.Linq.JObject]::Parse($json)

    foreach ($key in $properties.Keys) {
        $config[$key] = (ConvertTo-Json $properties[$key])
    }

    Update-Json -Path $pathToTemplateJson -Obj $config
}