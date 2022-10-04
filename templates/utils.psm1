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

    $ViewImports = Get-Content ".\$($folder)\Views\_ViewImports.cshtml", ".\Yuzu\_ViewImports.cshtml"
    Set-Content ".\$($folder)\Views\_ViewImports.cshtml" $ViewImports
}

function Copy-Icon {
    param (
        $Folder
    )

    Copy-Item -Path ".\Yuzu\Icon\icon.png" -Destination ".\$($folder)\.template.config\icon.png"
}

function Copy-Yuzu-Folder {
    param (
        $Folder
    )

    $pathToYuzuDir = ".\$($folder)\Yuzu"

    Copy-Item -Path ".\Yuzu\Startup" -Destination $pathToYuzuDir -Recurse
}

function Copy-Yuzu-Core {
    Copy-Item -Path "..\Yuzu\Core\" -Destination ".\Core" -Recurse
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
        $Folder,
        [bool] $isWeb
    )

    $AppSettings = Get-Content -Path ".\$($folder)\appsettings.Development.json" -Raw

    $AppSettings = $AppSettings.Trim()
    $AppSettings = $AppSettings.Substring(0, $AppSettings.Length-3)

    $yuzu = [PSCustomObject]@{
        Yuzu = [PSCustomObject]@{
            Core = [PSCustomObject]@{ 
                Pages = "/Yuzu/_templates/src/pages"
                Partials = "/Yuzu/_templates/src/blocks"
                SchemaMeta = "/Yuzu/_templates/paths"
            }
            VmGeneration = [PSCustomObject]@{ 
                IsActive = $True 
                AcceptUnsafeDirectory = $False 
                Directory = "~/Yuzu/ViewModels"
            }
            Import = [PSCustomObject]@{ 
                IsActive = $True 
                Config = "/Yuzu/YuzuConfig.json"
                ManualMappingDirectory = "~/Yuzu/Mappings/"
                Data = "/Yuzu/_templates/data"
                ImagesDef = "/_client/images"
                ImagesDel = "/wwwroot/_client/images"
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

# TODO: No longer required after we have RCL's
function Copy-Forms-Partials {
    param (
        $Folder,
        [bool] $isCore
    )

    Copy-Item -Path "..\src\Forms\wwwroot\Views\Partials\*" -Destination ".\$($folder)\Views\Partials" -Recurse
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

function Update-Json {
    param (
        $Path,
        $Obj 
    )

    $output = [Newtonsoft.Json.JsonConvert]::SerializeObject($Obj)

    Set-Content $Path $output
}

function Update-Template-Meta {
    param (
        $Folder,
        [bool] $isCore,
        [bool] $isWeb
    )

    $pathToTemplateJson = ".\$($folder)\.template.config\template.json"
    $pathToCliJson = ".\$($folder)\.template.config\dotnetcli.host.json"
    $pathToIdeJson = ".\$($folder)\.template.config\ide.host.json"

    $json = get-content $pathToTemplateJson
    $config = [Newtonsoft.Json.Linq.JObject]::Parse($json)

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

        $json = get-content $pathToCliJson
        $cli = [Newtonsoft.Json.Linq.JObject]::Parse($json)

        $json = get-content $pathToIdeJson
        $ide = [Newtonsoft.Json.Linq.JObject]::Parse($json)

        $config["symbols"]["CoreNamespace"] = $config["symbols"]["UmbracoVersion"]
        $config["symbols"]["CoreNamespace"]["replaces"].Value = "CORE_NAMESPACE"
        $config["symbols"]["CoreNamespace"]["defaultValue"].Value = "Core"
        $config["symbols"]["CoreNamespace"]["description"].Value = "The namespace of the core application"

        $cli["symbolInfo"]["CoreNamespace"] = $cli["symbolInfo"]["UseHttpsRedirect"]
        $cli["symbolInfo"]["CoreNamespace"]["longName"].Value = "core-namespace"
        $cli["symbolInfo"]["CoreNamespace"]["shortName"].Value = "core"

        $ideElement = $ide["symbolInfo"][0]
        $ideElement["id"].Value = "CoreNamespace"
        #this was removed by umbraco for some reason
        #$ideElement["name"]["text"].Value = "The namespace of the core application"
        $ide["symbolInfo"].Add($ideElement)

        $config["groupIdentity"].Value = $groupIdentityWeb
        $config["description"].Value = $descriptionWeb
        $config["identity"].Value = $identityWeb
        $config["name"].Value = $nameWeb
        $config["shortName"].Value = $shortNameWeb
        $config["shortName"].Value = $shortNameWeb
        $config["defaultName"].Value = $defaultNameWeb

        Update-Json -Path $pathToCliJson -Obj $cli
        Update-Json -Path $pathToIdeJson -Obj $ide
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

    Update-Json -Path $pathToTemplateJson -Obj $config
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