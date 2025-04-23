# PowerShell script to download and install ImageMagick

# Create a temporary directory
$tempDir = "$env:TEMP\ImageMagickInstall"
New-Item -ItemType Directory -Force -Path $tempDir | Out-Null

# Download ImageMagick installer
$installerUrl = "https://download.imagemagick.org/ImageMagick/download/binaries/ImageMagick-7.1.1-21-Q16-HDRI-x64-dll.exe"
$installerPath = "$tempDir\ImageMagick-Installer.exe"

Write-Host "Downloading ImageMagick installer..."
Invoke-WebRequest -Uri $installerUrl -OutFile $installerPath

# Run the installer with the necessary options
Write-Host "Installing ImageMagick..."
Start-Process -FilePath $installerPath -ArgumentList "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP- /TASKS=install_convert,install_devel,add_to_path" -Wait

Write-Host "ImageMagick installation completed. Please restart your computer to ensure the PATH is updated." 