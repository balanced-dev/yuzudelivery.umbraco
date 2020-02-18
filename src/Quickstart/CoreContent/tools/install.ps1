  
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
    }
}

if ($project) {

    RecursiveProjectItem $project.ProjectItems

}
