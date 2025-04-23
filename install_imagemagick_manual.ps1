# PowerShell script to open ImageMagick download page

$downloadUrl = "https://imagemagick.org/script/download.php#windows"

Write-Host "Opening ImageMagick download page in your default browser..."
Write-Host "Please follow these steps:"
Write-Host "1. Download the Windows Binary Release (64-bit) from the page that opens"
Write-Host "2. Run the installer"
Write-Host "3. During installation, make sure to:"
Write-Host "   - Check 'Install legacy utilities (e.g. convert)'"
Write-Host "   - Check 'Add application directory to your system path'"
Write-Host "4. Complete the installation and restart your computer"

# Open the download page in the default browser
Start-Process $downloadUrl 