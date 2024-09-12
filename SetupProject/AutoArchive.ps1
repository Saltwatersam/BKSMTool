# Relative path to AssemblyInfo.cs from the main project
$assemblyInfoPath = "..\..\BKSMTool\Properties\AssemblyInfo.cs"

# Check if the AssemblyInfo.cs file exists
if (-Not (Test-Path $assemblyInfoPath)) {
    Write-Error "The AssemblyInfo.cs file was not found at path $assemblyInfoPath"
    exit 1
}

# Extract the version from AssemblyInfo.cs
$version = (Get-Content $assemblyInfoPath | Select-String 'AssemblyVersion' | ForEach-Object { $_.Line -replace '[^0-9.]', '' })

if (-Not $version) {
    Write-Error "Unable to extract the version from the AssemblyInfo.cs file"
    exit 1
}

# Construct the ZIP file name
$projectName = "BKSMTool"
$platform = "x64"  # Modify this if the target platform is different (e.g., x86)
$zipFileName = "$projectName" + "_" + $platform + "_" + $version + ".zip"

# Output path for the ZIP (Release folder of the setup project)
$builtOutputPath = "..\Release"

# Check if the output folder exists
if (-Not (Test-Path $builtOutputPath)) {
    Write-Error "The output folder $builtOutputPath does not exist."
    exit 1
}

# Create the ZIP archive from the setup project files
Compress-Archive -Path "$builtOutputPath\*" -DestinationPath "$builtOutputPath\$zipFileName" -Force

Write-Host "ZIP file successfully created: $builtOutputPath\$zipFileName"