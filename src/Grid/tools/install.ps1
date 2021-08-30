  
param($installPath, $toolsPath, $package, $project)

if ($project) {
    
    Write-Host "Replacing default DocTypeGridEditor previw renderer"

    $projectPath = (Get-Item $project.Properties.Item("FullPath").Value).FullName

    $yuzuDocTypeGridRenderer = Join-Path $projectPath "\App_Plugins\YuzuGrid\DocTypeGridEditorPreviewer.cshtml"
    Write-Host "Yuzu Renderer Path:" "$yuzuDocTypeGridRenderer"
    $defaultDocTypeGridRenderer = Join-Path $projectPath "\App_Plugins\DocTypeGridEditor\Render\DocTypeGridEditorPreviewer.cshtml"
    Write-Host "Default Renderer Path:" "$defaultDocTypeGridRenderer"

	Copy-Item $yuzuDocTypeGridRenderer -Destination $defaultDocTypeGridRenderer -Force

    Write-Host "Completed DocTypeGridEditor previw renderer replacement"
}
