$url = 'https://dot.net/v1/dotnet-install.ps1'
$script = Join-Path $env:TEMP 'dotnet-install.ps1'
Invoke-WebRequest -Uri $url -OutFile $script
& $script -Channel 8.0 -InstallDir (Join-Path $env:LOCALAPPDATA 'dotnet')
