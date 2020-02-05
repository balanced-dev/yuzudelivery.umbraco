  
param($installPath, $toolsPath, $package, $project)


$p = Get-Project

function RecursiveProjectItem($items)
{
    foreach ($i in $items){

        $fullPath = $i.Properties.Item("FullPath").Value


        If($fullPath.EndsWith('App_Plugins\') -or $fullPath.EndsWith('Views\')) {  
            Write-Host "Deleting web specific folders:" "$fullPath"        
            $i.Remove()
            Remove-Item $fullPath -Recurse
        }
        else {
            RecursiveProjectItem $i.ProjectItems
        }

        If($fullPath.EndsWith('UmbracoModels\zGeneratedModels.cs')) {
            Write-Host "Adding Umbraco models builder tool:" "$fullPath"
            $i.Properties.Item("CustomTool").Value = 'UmbracoModelsBuilder'
            $i.Properties.Item("CustomToolNamespace").Value = 'YuzuDelivery.UmbracoModels'
        }

        If($fullPath.EndsWith('ViewModels\zGeneratedModels.cs')) {
            Write-Host "Adding Yuzu view models builder tool:" "$fullPath"            
            $i.Properties.Item("CustomTool").Value = 'YuzuViewModelGenerator'
            $i.Properties.Item("CustomToolNamespace").Value = 'YuzuDelivery.ViewModels'
        }
    }
}

if ($project) {

    RecursiveProjectItem $project.ProjectItems

}
