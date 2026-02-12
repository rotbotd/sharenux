<#
.SYNOPSIS
    ShareX setup script — replaces ShareX.Setup.exe for CI/CD pipelines.

.DESCRIPTION
    Downloads required tools, creates output folders (Portable, Debug, Steam,
    MicrosoftStore), packages zip archives, compiles InnoSetup installers, and
    builds APPX packages. Designed to run directly in GitHub Actions without
    compiling any C# code.

.PARAMETER Job
    The setup job to run. Valid values:
    Release, Debug, Steam, MicrosoftStore, MicrosoftStoreDebug

.PARAMETER Platform
    Target platform. Valid values: x64, arm64

.PARAMETER Silent
    Suppress interactive actions (e.g. opening output directory).
#>

[CmdletBinding()]
param(
    [ValidateSet("Release", "Debug", "Steam", "MicrosoftStore", "MicrosoftStoreDebug")]
    [string]$Job = "Release",

    [ValidateSet("x64", "arm64")]
    [string]$Platform = "x64",

    [switch]$Silent
)

$ErrorActionPreference = "Stop"

# ── Tool versions & URLs ─────────────────────────────────────────────────────

$FFmpegVersion = "8.0"
$FFmpegDownloadURL = "https://github.com/ShareX/FFmpeg/releases/download/v$FFmpegVersion/ffmpeg-$FFmpegVersion-win-$Platform.zip"

$RecorderDevicesVersion = "0.12.10"
$RecorderDevicesDownloadURL = "https://github.com/ShareX/RecorderDevices/releases/download/v$RecorderDevicesVersion/recorder-devices-$RecorderDevicesVersion-setup.exe"

$ExifToolVersion = "13.29"
$ExifToolDownloadURL = "https://github.com/ShareX/ExifTool/releases/download/v$ExifToolVersion/exiftool-$ExifToolVersion-win64.zip"

$InnoSetupCompilerPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

# ── Job flag definitions ──────────────────────────────────────────────────────

$JobFlags = @{
    Release             = @("CreateSetup", "CreatePortable", "DownloadTools")
    Debug               = @("CreateDebug", "DownloadTools")
    Steam               = @("CreateSteamFolder", "DownloadTools")
    MicrosoftStore      = @("CreateMicrosoftStoreFolder", "CompileAppx", "DownloadTools")
    MicrosoftStoreDebug = @("CreateMicrosoftStoreDebugFolder", "CompileAppx", "DownloadTools")
}

$ActiveFlags = $JobFlags[$Job]

function HasFlag([string]$flag) {
    return $ActiveFlags -contains $flag
}

# ── Resolve paths ─────────────────────────────────────────────────────────────

Write-Host "ShareX setup started."
Write-Host "Job: $Job"

$ParentDir = Get-Location | Select-Object -ExpandProperty Path

if (-not (Test-Path (Join-Path $ParentDir "ShareX.sln"))) {
    # Fall back: script may be running from inside ShareX.Setup
    $ParentDir = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

    if (-not (Test-Path (Join-Path $ParentDir "ShareX.sln"))) {
        Write-Error "Cannot locate ShareX.sln. Run this script from the repository root."
        exit 1
    }
}

Write-Host "Parent directory: $ParentDir"

$RuntimeId = if ($Platform -eq "arm64") { "win-arm64" } else { "win-x64" }

# Determine configuration from job
switch ($Job) {
    "Debug"               { $Configuration = "Debug" }
    "Steam"               { $Configuration = "Steam" }
    "MicrosoftStore"      { $Configuration = "MicrosoftStore" }
    "MicrosoftStoreDebug" { $Configuration = "MicrosoftStoreDebug" }
    default               { $Configuration = "Release" }
}

Write-Host "Configuration: $Configuration"

$BinDir               = Join-Path $ParentDir "ShareX\bin\$Configuration\$RuntimeId"
$SteamLauncherDir     = Join-Path $ParentDir "ShareX.Steam\bin\$Configuration"
$OutputDir            = Join-Path $ParentDir "Output"
$PortableOutputDir    = Join-Path $OutputDir "ShareX-portable"
$DebugOutputDir       = Join-Path $OutputDir "ShareX-debug"
$SteamOutputDir       = Join-Path $OutputDir "ShareX-Steam"
$SteamUpdatesDir      = Join-Path $SteamOutputDir "Updates"
$MicrosoftStoreOutputDir      = Join-Path $OutputDir "ShareX-MicrosoftStore"
$MicrosoftStoreDebugOutputDir = Join-Path $OutputDir "ShareX-MicrosoftStore-debug"

$SetupDir             = Join-Path $ParentDir "ShareX.Setup"
$InnoSetupDir         = Join-Path $SetupDir "InnoSetup"
$MicrosoftStorePackageFilesDir = Join-Path $SetupDir "MicrosoftStore"

$FFmpegPath                = Join-Path $OutputDir "ffmpeg.exe"
$RecorderDevicesSetupPath  = Join-Path $OutputDir "recorder-devices-$RecorderDevicesVersion-setup.exe"
$ExifToolPath              = Join-Path $OutputDir "exiftool.exe"

# ── Read app version from Directory.build.props ───────────────────────────────

$propsPath = Join-Path $ParentDir "Directory.build.props"
$propsContent = Get-Content $propsPath -Raw
$versionMatch = [regex]::Match($propsContent, '<Version>([0-9]+(?:\.[0-9]+){1,3})</Version>')

if (-not $versionMatch.Success) {
    Write-Error "Could not read version from $propsPath"
    exit 1
}

$AppVersion = $versionMatch.Groups[1].Value
Write-Host "Application version: $AppVersion"

# Output file paths (depend on version)
$SetupPath                     = Join-Path $OutputDir "ShareX-$AppVersion-setup-$Platform.exe"
$PortableZipPath               = Join-Path $OutputDir "ShareX-$AppVersion-portable-$Platform.zip"
$DebugZipPath                  = Join-Path $OutputDir "ShareX-$AppVersion-debug-$Platform.zip"
$SteamZipPath                  = Join-Path $OutputDir "ShareX-$AppVersion-Steam-$Platform.zip"
$MicrosoftStoreAppxPath        = Join-Path $OutputDir "ShareX-$AppVersion-MicrosoftStore-$Platform.appx"
$MicrosoftStoreDebugAppxPath   = Join-Path $OutputDir "ShareX-$AppVersion-MicrosoftStore-debug-$Platform.appx"

# ── Helper functions ──────────────────────────────────────────────────────────

function Copy-FilteredFiles([string]$Source, [string]$Destination, [string]$Filter) {
    if (-not (Test-Path $Destination)) {
        New-Item -ItemType Directory -Path $Destination -Force | Out-Null
    }
    $files = Get-ChildItem -Path $Source -Filter $Filter -File -ErrorAction SilentlyContinue
    foreach ($f in $files) {
        Copy-Item $f.FullName -Destination $Destination -Force
    }
}

function Copy-SingleFile([string]$Source, [string]$Destination) {
    if (Test-Path $Source) {
        if (-not (Test-Path $Destination)) {
            New-Item -ItemType Directory -Path $Destination -Force | Out-Null
        }
        Copy-Item $Source -Destination $Destination -Force
    }
}

function Copy-DirectoryContents([string]$Source, [string]$Destination) {
    if (Test-Path $Source) {
        if (-not (Test-Path $Destination)) {
            New-Item -ItemType Directory -Path $Destination -Force | Out-Null
        }
        Copy-Item "$Source\*" -Destination $Destination -Recurse -Force
    }
}

function Download-File([string]$Url, [string]$OutPath) {
    if (-not (Test-Path (Split-Path $OutPath -Parent))) {
        New-Item -ItemType Directory -Path (Split-Path $OutPath -Parent) -Force | Out-Null
    }
    Write-Host "Downloading: $Url"
    Invoke-WebRequest -Uri $Url -OutFile $OutPath -UseBasicParsing
}

function Create-OutputFolder([string]$Source, [string]$Destination, [string]$JobType) {
    Write-Host "Creating folder: $Destination"

    if (Test-Path $Destination) {
        Remove-Item $Destination -Recurse -Force
    }
    New-Item -ItemType Directory -Path $Destination -Force | Out-Null

    # Core binaries
    Copy-FilteredFiles $Source $Destination "*.exe"
    Copy-FilteredFiles $Source $Destination "*.dll"
    Copy-FilteredFiles $Source $Destination "*.json"

    # Debug symbols
    if ($JobType -eq "CreateDebug" -or $JobType -eq "CreateMicrosoftStoreDebugFolder") {
        Copy-FilteredFiles $Source $Destination "*.pdb"
    }

    # Licenses
    $licenseSrc = Join-Path $ParentDir "Licenses"
    $licenseDst = Join-Path $Destination "Licenses"
    Copy-FilteredFiles $licenseSrc $licenseDst "*.txt"

    # RecorderDevices (not for Microsoft Store builds)
    if ($JobType -ne "CreateMicrosoftStoreFolder" -and $JobType -ne "CreateMicrosoftStoreDebugFolder") {
        Copy-SingleFile $RecorderDevicesSetupPath $Destination
    }

    # Icon
    Copy-SingleFile (Join-Path $Source "ShareX_File_Icon.ico") $Destination

    # Language resource DLLs
    $langPattern = '^[a-z]{2}(?:-[A-Z]{2})?$'
    Get-ChildItem -Path $Source -Directory -ErrorAction SilentlyContinue | Where-Object {
        $_.Name -match $langPattern
    } | ForEach-Object {
        $langCode = $_.Name
        $langDst = Join-Path $Destination "Languages\$langCode"
        Copy-FilteredFiles $_.FullName $langDst "*.resources.dll"
    }

    # FFmpeg
    Copy-SingleFile $FFmpegPath $Destination

    # ExifTool
    if (Test-Path $ExifToolPath) {
        Copy-SingleFile $ExifToolPath $Destination
        $exifFilesDir = Join-Path $OutputDir "exiftool_files"
        if (Test-Path $exifFilesDir) {
            Copy-DirectoryContents $exifFilesDir (Join-Path $Destination "exiftool_files")
        }
    }

    # Stickers
    $stickersDir = Join-Path $ParentDir "ShareX.ScreenCaptureLib\Stickers"
    Copy-DirectoryContents $stickersDir (Join-Path $Destination "Stickers")

    # Job-specific extras
    if ($JobType -eq "CreatePortable") {
        # Create empty portable marker file
        New-Item -ItemType File -Path (Join-Path $Destination "Portable") -Force | Out-Null
    }
    elseif ($JobType -eq "CreateMicrosoftStoreFolder" -or $JobType -eq "CreateMicrosoftStoreDebugFolder") {
        # Copy Microsoft Store package files
        Copy-DirectoryContents $MicrosoftStorePackageFilesDir $Destination

        # Patch AppxManifest.xml with platform
        $manifestPath = Join-Path $Destination "AppxManifest.xml"
        if (Test-Path $manifestPath) {
            $content = Get-Content $manifestPath -Raw
            $content = $content.Replace("{PLATFORM}", $Platform)
            Set-Content -Path $manifestPath -Value $content -NoNewline
        }
    }

    Write-Host "Folder created: $Destination"
}

function Create-ZipFile([string]$Source, [string]$ArchivePath) {
    Write-Host "Creating zip file: $ArchivePath"
    Compress-Archive -Path "$Source\*" -DestinationPath $ArchivePath -Force
}

# ── Main execution ────────────────────────────────────────────────────────────

# Clean output directory
if (Test-Path $OutputDir) {
    Write-Host "Cleaning output directory: $OutputDir"
    Remove-Item $OutputDir -Recurse -Force
}

New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

# Download tools
if (HasFlag "DownloadTools") {
    # FFmpeg
    if (-not (Test-Path $FFmpegPath)) {
        $ffmpegZipName = $FFmpegDownloadURL.Split('/')[-1]
        $ffmpegZipPath = Join-Path $OutputDir $ffmpegZipName
        Download-File $FFmpegDownloadURL $ffmpegZipPath

        Write-Host "Extracting: $ffmpegZipPath"
        $ffmpegTempDir = Join-Path $OutputDir "_ffmpeg_temp"
        Expand-Archive -Path $ffmpegZipPath -DestinationPath $ffmpegTempDir -Force
        # Find ffmpeg.exe inside the extracted content
        $ffmpegExe = Get-ChildItem -Path $ffmpegTempDir -Recurse -Filter "ffmpeg.exe" | Select-Object -First 1
        if ($ffmpegExe) {
            Copy-Item $ffmpegExe.FullName -Destination $FFmpegPath -Force
        }
        Remove-Item $ffmpegTempDir -Recurse -Force
        Remove-Item $ffmpegZipPath -Force
    }

    # RecorderDevices
    if (-not (Test-Path $RecorderDevicesSetupPath)) {
        Download-File $RecorderDevicesDownloadURL $RecorderDevicesSetupPath
    }

    # ExifTool
    if (-not (Test-Path $ExifToolPath)) {
        $exifZipName = $ExifToolDownloadURL.Split('/')[-1]
        $exifZipPath = Join-Path $OutputDir $exifZipName
        Download-File $ExifToolDownloadURL $exifZipPath

        Write-Host "Extracting: $exifZipPath"
        Expand-Archive -Path $exifZipPath -DestinationPath $OutputDir -Force
        Remove-Item $exifZipPath -Force
    }
}

# Create Setup (InnoSetup)
if (HasFlag "CreateSetup") {
    if (Test-Path $InnoSetupCompilerPath) {
        $issFile = "ShareX-setup.iss"
        Write-Host "Compiling setup file: $issFile"
        Push-Location $InnoSetupDir
        try {
            & $InnoSetupCompilerPath /Q "/DPlatform=$Platform" "$issFile" | Out-Host
            if ($LASTEXITCODE -ne 0) {
                Write-Error "InnoSetup compilation failed with exit code $LASTEXITCODE"
                exit 1
            }
        }
        finally {
            Pop-Location
        }
        Write-Host "Setup file compiled: $issFile"
    }
    else {
        Write-Warning "InnoSetup compiler is missing: $InnoSetupCompilerPath"
    }
}

# Create Portable
if (HasFlag "CreatePortable") {
    Create-OutputFolder $BinDir $PortableOutputDir "CreatePortable"
    Create-ZipFile $PortableOutputDir $PortableZipPath
}

# Create Debug
if (HasFlag "CreateDebug") {
    Create-OutputFolder $BinDir $DebugOutputDir "CreateDebug"
    Create-ZipFile $DebugOutputDir $DebugZipPath
}

# Create Steam folder
if (HasFlag "CreateSteamFolder") {
    Write-Host "Creating Steam folder: $SteamOutputDir"

    if (Test-Path $SteamOutputDir) {
        Remove-Item $SteamOutputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $SteamOutputDir -Force | Out-Null

    Copy-SingleFile (Join-Path $SteamLauncherDir "ShareX_Launcher.exe") $SteamOutputDir
    Copy-SingleFile (Join-Path $SteamLauncherDir "steam_appid.txt") $SteamOutputDir
    Copy-SingleFile (Join-Path $SteamLauncherDir "installscript.vdf") $SteamOutputDir
    Copy-FilteredFiles $SteamLauncherDir $SteamOutputDir "*.dll"

    Create-OutputFolder $BinDir $SteamUpdatesDir "CreateSteamFolder"
    Create-ZipFile $SteamOutputDir $SteamZipPath
}

# Create Microsoft Store folder
if (HasFlag "CreateMicrosoftStoreFolder") {
    Create-OutputFolder $BinDir $MicrosoftStoreOutputDir "CreateMicrosoftStoreFolder"

    if (HasFlag "CompileAppx") {
        # Locate MakeAppx.exe via registry
        $sdkInstallDir = (Get-ItemProperty -Path "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0" -Name "InstallationFolder" -ErrorAction SilentlyContinue).InstallationFolder
        $sdkProductVer = (Get-ItemProperty -Path "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0" -Name "ProductVersion" -ErrorAction SilentlyContinue).ProductVersion
        # Normalize version to 4-part
        $verParts = $sdkProductVer.Split('.')
        while ($verParts.Count -lt 4) { $verParts += "0" }
        $normalizedVer = $verParts -join '.'
        $MakeAppxPath = Join-Path $sdkInstallDir "bin\$normalizedVer\x64\makeappx.exe"
        Write-Host "Windows Kits directory: $(Split-Path $MakeAppxPath -Parent | Split-Path -Parent)"

        Write-Host "Compiling appx file: $MicrosoftStoreOutputDir"
        & $MakeAppxPath pack /d "$MicrosoftStoreOutputDir" /p "$MicrosoftStoreAppxPath" /l /o | Out-Host
        if ($LASTEXITCODE -ne 0) {
            Write-Error "MakeAppx failed with exit code $LASTEXITCODE"
            exit 1
        }
        Write-Host "Appx file compiled: $MicrosoftStoreAppxPath"
    }
}

# Create Microsoft Store Debug folder
if (HasFlag "CreateMicrosoftStoreDebugFolder") {
    Create-OutputFolder $BinDir $MicrosoftStoreDebugOutputDir "CreateMicrosoftStoreDebugFolder"

    if (HasFlag "CompileAppx") {
        # Locate MakeAppx.exe via registry (same as above)
        $sdkInstallDir = (Get-ItemProperty -Path "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0" -Name "InstallationFolder" -ErrorAction SilentlyContinue).InstallationFolder
        $sdkProductVer = (Get-ItemProperty -Path "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows\v10.0" -Name "ProductVersion" -ErrorAction SilentlyContinue).ProductVersion
        $verParts = $sdkProductVer.Split('.')
        while ($verParts.Count -lt 4) { $verParts += "0" }
        $normalizedVer = $verParts -join '.'
        $MakeAppxPath = Join-Path $sdkInstallDir "bin\$normalizedVer\x64\makeappx.exe"

        Write-Host "Compiling appx file: $MicrosoftStoreDebugOutputDir"
        & $MakeAppxPath pack /d "$MicrosoftStoreDebugOutputDir" /p "$MicrosoftStoreDebugAppxPath" /l /o | Out-Host
        if ($LASTEXITCODE -ne 0) {
            Write-Error "MakeAppx failed with exit code $LASTEXITCODE"
            exit 1
        }
        Write-Host "Appx file compiled: $MicrosoftStoreDebugAppxPath"
    }
}

Write-Host "ShareX setup successfully completed."
