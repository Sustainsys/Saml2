$ErrorActionPreference = "Stop"

$status = (git status)
$clean = $status| select-string "working tree clean"

if ("$clean" -eq "")
{
  echo "Working copy is not clean. Cannot proceed."
  exit
}

pushd ..
del Sustainsys.Saml2\bin\Release\*.dll
del Sustainsys.Saml2.Mvc\bin\Release\*.dll
del Sustainsys.Saml2.Owin\bin\Release\*.dll
del Sustainsys.Saml2.HttpModule\bin\Release\*.dll
del Sustainsys.Saml2.AspNetCore2\bin\Release\*.dll

echo "Creating nuspec files..."

$releaseNotes = "<releaseNotes>`n $((get-content nuget\ReleaseNotes.txt) -join "`n`")`n    </releaseNotes>"
function Create-Nuspec($projectName)
{
    (gc nuget\$projectName.nuspec) | 
		% { $_ -replace "<releaseNotes />", $releaseNotes } |
		set-content $projectName\$projectName.nuspec
}

Create-Nuspec("Sustainsys.Saml2")
Create-Nuspec("Sustainsys.Saml2.Mvc")
Create-Nuspec("Sustainsys.Saml2.Owin")
Create-Nuspec("Sustainsys.Saml2.HttpModule")
Create-Nuspec("Sustainsys.Saml2.AspNetCore2")

echo "Building packages..."

nuget pack -build -outputdirectory nuget Sustainsys.Saml2\Sustainsys.Saml2.csproj
nuget pack -build -outputdirectory nuget Sustainsys.Saml2.Mvc\Sustainsys.Saml2.Mvc.csproj
nuget pack -build -outputdirectory nuget Sustainsys.Saml2.Owin\Sustainsys.Saml2.Owin.csproj
nuget pack -build -outputdirectory nuget Sustainsys.Saml2.HttpModule\Sustainsys.Saml2.HttpModule.csproj
nuget pack -build -outputdirectory nuget Sustainsys.Saml2.AspNetCore2\Sustainsys.Saml2.AspNetCore2.csproj

popd
