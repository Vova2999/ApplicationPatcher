@echo off
pushd %~dp0
call scripts\run-project.bat ApplicationPatcher.sln
popd
@echo on