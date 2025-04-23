# PowerShell script to download and install ImageMagick

# Create a temporary directory
$tempDir = "$env:TEMP\ImageMagickInstall"
New-Item -ItemType Directory -Force -Path $tempDir | Out-Null

# Direct download link for ImageMagick installer
$installerUrl = "https://download.imagemagick.org/ImageMagick/download/binaries/ImageMagick-7.1.1-21-Q16-HDRI-x64-dll.exe"
$installerPath = "$tempDir\ImageMagick-Installer.exe"

Write-Host "Downloading ImageMagick installer..."
try {
    Invoke-WebRequest -Uri $installerUrl -OutFile $installerPath -UseBasicParsing
    Write-Host "Download completed successfully."
} catch {
    Write-Host "Failed to download ImageMagick installer: $_"
    Write-Host "Please download manually from: https://imagemagick.org/script/download.php#windows"
    exit 1
}

# Run the installer with the necessary options
Write-Host "Installing ImageMagick..."
try {
    Start-Process -FilePath $installerPath -ArgumentList "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP- /TASKS=install_convert,install_devel,add_to_path" -Wait
    Write-Host "ImageMagick installation completed successfully."
} catch {
    Write-Host "Failed to install ImageMagick: $_"
    Write-Host "Please install manually from the downloaded file."
    exit 1
}

Write-Host "ImageMagick installation completed. Please restart your computer to ensure the PATH is updated." 