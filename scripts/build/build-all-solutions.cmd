@pushd %~dp0

@tasklist | find "MSBuild.exe" > nul
@if %errorLevel%==0 taskkill /IM MSBuild.exe /f

@call build-solution.cmd ..\..\_source\ApplicationPatcher.Self\ApplicationPatcher.Self.csproj %1 ..\..\_source\packages
@call ..\..\Build\ApplicationPatcher.Self\ApplicationPatcher.Self.exe
@call build-solution.cmd ..\..\_source\ApplicationPatcher.sln %1

@tasklist | find "MSBuild.exe" > nul
@if %errorLevel%==0 taskkill /IM MSBuild.exe /f

@if %1==Release del /S /Q ..\..\Build\*.pdb > nul

@popd