setlocal EnableDelayedExpansion

del /f /q *.nupkg
call nuget-pack.cmd

..\src\.nuget\nuget.exe push EasyMigrator.Core.?.?.?.nupkg %1
..\src\.nuget\nuget.exe push EasyMigrator.Core.?.?.?.symbols.nupkg %1
..\src\.nuget\nuget.exe push EasyMigrator.FluentMigrator.?.?.?.nupkg %1
..\src\.nuget\nuget.exe push EasyMigrator.FluentMigrator.?.?.?.symbols.nupkg %1
..\src\.nuget\nuget.exe push EasyMigrator.MigratorDotNet.?.?.?.nupkg %1
..\src\.nuget\nuget.exe push EasyMigrator.MigratorDotNet.?.?.?.symbols.nupkg %1
