@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
set "PROJECT_PATH=%SCRIPT_DIR%src\ZYC.Framework.Build.NewModule\ZYC.Framework.Build.NewModule.csproj"
set "SOURCE_ROOT=%SCRIPT_DIR%src"

dotnet run --project "%PROJECT_PATH%" -- --src-root "%SOURCE_ROOT%" %*
exit /b %ERRORLEVEL%
