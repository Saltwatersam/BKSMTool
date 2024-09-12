@echo off

REM Define paths
set "assemblyInfoPath=..\..\BKSMTool\Properties\AssemblyInfo.cs"
set "builtOutputPath=..\Release"
set "projectName=BKSMTool"
set "platform=x64"  REM Change to x86 if needed

REM Check if AssemblyInfo.cs exists
if not exist "%assemblyInfoPath%" (
    echo The file %assemblyInfoPath% was not found.
    exit /b 1
)

REM Extract the version from AssemblyInfo.cs
for /f "tokens=2 delims== " %%A in ('findstr /R "AssemblyVersion" "%assemblyInfoPath%"') do (
    set "version=%%~A"
)

REM Clean up the version string (remove quotes and any trailing whitespace)
set "version=%version:~1,-1%"

REM Check if the version was extracted
if "%version%"=="" (
    echo Unable to extract the version from %assemblyInfoPath%.
    exit /b 1
)

REM Check if the output directory exists
if not exist "%builtOutputPath%" (
    echo The output folder %builtOutputPath% does not exist.
    exit /b 1
)

REM Define the name of the ZIP file
set "zipFileName=%builtOutputPath%\%projectName%_%platform%_%version%.zip"

REM Compress the folder using PowerShell as cmd doesn't have built-in zip support
powershell -Command "Compress-Archive -Path '%builtOutputPath%\*' -DestinationPath '%zipFileName%' -Force"

REM Check if the ZIP was created
if exist "%zipFileName%" (
    echo ZIP file successfully created: %zipFileName%
) else (
    echo Failed to create the ZIP file.
    exit /b 1
)