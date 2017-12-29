$ErrorActionPreference = "Stop"

$status = (git status)
$clean = $status| select-string "working tree clean"

if ("$clean" -eq "")
{
  echo "Working copy is not clean. Cannot proceed."
  exit
}

$master = $status | select-string "On branch master"

if ("$master" -eq "")
{
  echo "Releases are only allowed from the master branch."
  exit
}

pushd ..
del Kentor.AuthServices\bin\Release\*.dll
del Kentor.AuthServices.Mvc\bin\Release\*.dll
del Kentor.AuthServices.Owin\bin\Release\*.dll
del Kentor.AuthServices.HttpModule\bin\Release\*.dll
del Sustainsys.Saml2.AspNetCore2\bin\Release\*.dll

echo "Creating nuspec files..."

$releaseNotes = "<releaseNotes>`n $((get-content nuget\ReleaseNotes.txt) -join "`n`")`n    </releaseNotes>"
function Create-Nuspec($projectName)
{
    (gc nuget\$projectName.nuspec) | 
		% { $_ -replace "<releaseNotes />", $releaseNotes } |
		set-content $projectName\$projectName.nuspec
}

Create-Nuspec("Kentor.AuthServices")
Create-Nuspec("Kentor.AuthServices.Mvc")
Create-Nuspec("Kentor.AuthServices.Owin")
Create-Nuspec("Kentor.AuthServices.HttpModule")
Create-Nuspec("Sustainsys.Saml2.AspNetCore2")

echo "Building packages..."

nuget pack -build -outputdirectory nuget Kentor.AuthServices\Kentor.AuthServices.csproj
nuget pack -build -outputdirectory nuget Kentor.AuthServices.Mvc\Kentor.AuthServices.Mvc.csproj
nuget pack -build -outputdirectory nuget Kentor.AuthServices.Owin\Kentor.AuthServices.Owin.csproj
nuget pack -build -outputdirectory nuget Kentor.AuthServices.HttpModule\Kentor.AuthServices.HttpModule.csproj
nuget pack -build -outputdirectory nuget Sustainsys.Saml2.AspNetCore2\Sustainsys.Saml2.AspNetCore2.csproj

popd
