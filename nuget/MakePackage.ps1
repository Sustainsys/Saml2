param (
	[string]$version = "auto"
)

$ErrorActionPreference = "Stop"

$status = (git status)
$clean = $status| select-string "working directory clean"

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

function Increment-PatchNumber
{
	$assemblyVersionPattern = '^\[assembly: AssemblyVersion\("([0-9]+(\.([0-9]+|\*)){1,3})"\)'  
	$rawVersionNumberGroup = get-content VersionInfo.cs| select-string -pattern $assemblyVersionPattern | % { $_.Matches }

	$rawVersionNumber = $rawVersionNumberGroup.Groups[1].Value  
	$versionParts = $rawVersionNumber.Split('.')  
	$versionParts[2] = ([int]$versionParts[2]) + 1  
	$updatedAssemblyVersion = "{0}.{1}.{2}" -f $versionParts[0], $versionParts[1], $versionParts[2]

	return $updatedAssemblyVersion
}

function Set-Version($newVersion)
{
	$versionPattern = "[0-9]+(\.([0-9]+|\*)){1,3}"
	(get-content VersionInfo.cs) | % { 
		% { $_ -replace $versionPattern, $newVersion }
	} | set-content VersionInfo.cs	
}

if("$version" -eq "auto")
{
	$version = Increment-PatchNumber
}

Set-Version($version)

echo "Version updated to $version, commiting and tagging..."

git commit -a -m "Updated version number to $version for release."
git tag v$version

echo "Building package..."

nuget pack -build -outputdirectory nuget Kentor.AuthServices\Kentor.AuthServices.csproj
nuget pack -build -outputdirectory nuget Kentor.AuthServices.Mvc\Kentor.AuthServices.Mvc.csproj
nuget pack -build -outputdirectory nuget Kentor.AuthServices.Owin\Kentor.AuthServices.Owin.csproj

$version = Increment-PatchNumber
Set-Version($version)

echo "Version updated to $version for development, committing..."

git commit -a -m "Updated version number to $version for development."

popd
