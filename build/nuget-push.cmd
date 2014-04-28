del /f /q *.nupkg
call nuget-pack.cmd
..\src\.nuget\nuget.exe push EasyMigrator*.nupkg %1