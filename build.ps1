param(
	[Parameter (Mandatory=$true)]
	[string] $version,
	
	[Parameter (Mandatory=$true)]
	[string] $apikey
	)

function Check-Exit-Code{
	if ($LASTEXITCODE -eq 1) {
		exit /b $LASTEXITCODE
	}
}

&".\.paket\paket.bootstrapper.exe"
Check-Exit-Code

&".\.paket\paket.exe" "restore"
Check-Exit-Code

&".\packages\FAKE\tools\FAKE.exe" "build.fsx" "version=$version" "apikey=$apikey" 

Check-Exit-Code