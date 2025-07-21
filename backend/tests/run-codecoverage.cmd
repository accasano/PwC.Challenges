@echo off

set reportGeneratorVersion=5.4.9
set netVersion=net9.0
set name=PwC.CarRental
set project=%cd%\PwC.CarRental.UnitTests\PwC.CarRental.UnitTests.csproj

dotnet test "%project%" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="%cd%\..\coverage.%name%.xml" /p:Platform=x64 /p:Configuration=Debug /p:ExcludeByFile="**/program.cs"

dotnet "%UserProfile%\.nuget\packages\reportgenerator\%reportGeneratorVersion%\tools\%netVersion%\ReportGenerator.dll" -reports:"%cd%\..\coverage*.xml" -targetdir:"%cd%\report" -title:"%name%" -reporttypes:Html;MarkdownSummary;MarkdownAssembliesSummary -classfilters:"+PwC.CarRental.Domain.Aggregates.RentalSystem"

if EXIST "%cd%\..\coverage.%name%.xml" ( del "%cd%\..\coverage.%name%.xml" )