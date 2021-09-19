@echo OFF

echo ***** Running Unit Tests via OpenCover
..\packages\OpenCover.4.7.1221\tools\OpenCover.Console.exe "-target:..\packages\NUnit.ConsoleRunner.3.12.0\tools\nunit3-console.exe" "-targetargs:..\SharpQLite.Tests\bin\SharpQLite.Tests.dll --work:..\SharpQLite.Tests\bin\TestResults.xml" "-register:user" "-output:..\SharpQLite.Tests\bin\coverage.xml" "-filter:+[*]* -[*.Tests*]* -[*nunit*]*"

echo ***** Creating HTML-based report
..\packages\ReportGenerator.4.8.12\tools\net47\ReportGenerator.exe -reports:..\SharpQLite.Tests\bin\coverage.xml -targetdir:..\SharpQLite.Tests\bin\CodeCoverage

echo ***** Opening Browser
..\SharpQLite.Tests\bin\CodeCoverage\index.htm