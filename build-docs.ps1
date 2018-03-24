Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)
    $zipfile = Resolve-Path $zipfile
    $outpath = Resolve-Path $outpath
    
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

Set-Location "docs_src"

if(!(Test-Path ".\Wyam")){
    New-Item -ItemType directory -Path ".\Wyam"
    Invoke-WebRequest "https://raw.githubusercontent.com/Wyamio/Wyam/master/RELEASE" -OutFile ".\Wyam\wyamversion.txt"
    $version = Get-Content ".\Wyam\wyamversion.txt"
    $filePath = "https://github.com/Wyamio/Wyam/releases/download/$version/Wyam-$version.zip"

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri $filePath -OutFile ".\Wyam\Wyam.zip"

    Unzip ".\Wyam\Wyam.zip" ".\Wyam"
}


&".\Wyam\wyam.exe" "--output" "..\docs"

Set-Location ".."