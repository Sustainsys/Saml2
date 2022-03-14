@echo off
REM ** Be sure to install these nugets:
REM ** NUnit.ConsoleRunner
REM ** OpenCover
REM ** ReportGenerator
REM **
REM ** All paths should be entered without quotes

REM ** SET TestResultsFileProjectName=CalculatorResults
SET TestResultsFileProjectName=TestResults

REM ** SET DLLToTestRelativePath=Calculator\bin\Debug\MyCalc.dll
SET DLLToTestRelativeFolder=Tests\Tests.NETFramework\bin\Debug
SET DLLToTestRelativePath=%DLLToTestRelativeFolder%\Tests.NETFramework.dll

REM ** Filters Wiki https://github.com/opencover/opencover/wiki/Usage
REM ** SET Filters=+[Calculator]* -[Calculator]CalculatorTests.*
SET Filters=+[Sustainsys.Saml2*]* -[Tests.NETFramework].*

SET OpenCoverFolderName=OpenCover.4.6.519
SET NUnitConsoleRunnerFolderName=NUnit.ConsoleRunner.3.6.1
SET ReportGeneratorFolderName=ReportGenerator.3.1.2
SET VSTestPath=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe

REM *****************************************************************

REM Create a 'GeneratedReports' folder if it does not exist
if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"

REM Remove any previous test execution files to prevent issues overwriting
IF EXIST "%~dp0%TestResultsFileProjectName%.trx" del "%~dp0%TestResultsFileProjectName%.trx%"

REM Remove any previously created test output directories
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"

REM Run the tests against the targeted output
call :RunOpenCoverUnitTestMetrics

REM Generate the report output based on the test results
if %errorlevel% equ 0 (
 call :RunReportGeneratorOutput
)

REM Launch the report
if %errorlevel% equ 0 (
 call :RunLaunchReport
)
exit /b %errorlevel%

:RunOpenCoverUnitTestMetrics
rem -targetdir:"%~dp0%DLLToTestRelativeFolder%" ^
rem -searchdirs:"%~dp0%DLLToTestRelativeFolder%" ^

"%~dp0packages\%OpenCoverFolderName%\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"%VSTestPath%" ^
-targetargs:"\"%~dp0%DLLToTestRelativePath%\"" ^
-filter:"%Filters%" ^
-mergebyhash ^
-skipautoprops ^
-excludebyattribute:"System.CodeDom.Compiler.GeneratedCodeAttribute" ^
-output:"%~dp0GeneratedReports\%TestResultsFileProjectName%.xml"
exit /b %errorlevel%

:RunReportGeneratorOutput
"%~dp0packages\%ReportGeneratorFolderName%\tools\ReportGenerator.exe" ^
-reports:"%~dp0GeneratedReports\%TestResultsFileProjectName%.xml" ^
-targetdir:"%~dp0GeneratedReports\ReportGenerator Output"
exit /b %errorlevel%

:RunLaunchReport
start "report" "%~dp0GeneratedReports\ReportGenerator Output\index.htm"
exit /b %errorlevel%
