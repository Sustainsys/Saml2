pushd ..\Kentor.AuthServices
del bin\Release\*.dll
nuget pack -build -outputdirectory ..\nuget
popd