del /f /q *.nupkg
..\src\.nuget\nuget.exe pack ..\src\EasyMigrator.Core\EasyMigrator.Core.csproj -sym
..\src\.nuget\nuget.exe pack ..\src\EasyMigrator.FluentMigrator\EasyMigrator.FluentMigrator.csproj -sym
..\src\.nuget\nuget.exe pack ..\src\EasyMigrator.MigratorDotNet\EasyMigrator.MigratorDotNet.csproj -sym