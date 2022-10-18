Write-Output "`n"
Write-Output "Copying files to delivery app"

Remove-Item -Recurse -Force ..\testProject\wwwroot\_client\
Remove-Item -Recurse -Force ..\testProject\yuzu\_templates\
Remove-Item -Recurse -Force ..\testProject\wwwroot\yuzu-def-ui\

xcopy /s /q /y dist\\_client ..\\testProject\\wwwroot\\_client\\
xcopy /s /q /y dist\\yuzu-def-ui ..\\testProject\\wwwroot\\yuzu-def-ui\\
xcopy /s /q /y dist\\_templates ..\\testProject\\yuzu\\_templates\\
xcopy /s /q /y dist\\yuzu.html ..\\testProject\\wwwroot\\
