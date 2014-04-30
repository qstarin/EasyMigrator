@echo off
setlocal EnableDelayedExpansion


for %%f in (*.nupkg) do (
	set "sf=%%f"
	if "!sf!" == "!sf:symbols.nupkg=!" (
		if "!sf!" == "!sf:EasyMigrator.MigratorDotNet=!" (
			..\src\.nuget\nuget.exe push %%f %1
		)
	)
)
