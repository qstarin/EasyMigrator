call nuget-pack.cmd
..\src\.nuget\nuget.exe push EasyMigrator.Core*.nupkg %1
..\src\.nuget\nuget.exe push EasyMigrator.FluentMigrator*.nupkg %1