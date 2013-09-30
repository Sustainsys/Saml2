@echo off
cd ..\Kentor.AuthServices
del bin\Release\*.dll
nuget pack -build -outputdirectory ..\nuget
cd ..\nuget