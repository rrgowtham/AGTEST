SonarQube.Scanner.MSBuild.exe begin /k:"ahm:active-analytics:dev" /n:"active-analytics" /v:"1.0" /s:C:\GIT\ActiveAnalytics\Main\Source\ActiveAnalytics\SonarQube.Analysis.xml /d:sonar.verbose=true /d:sonar.sources=. /d:sonar.dotnet.visualstudio.solution.file=ActiveHealthPortal.sln
::Build the project before covering code
"C:\Program Files (x86)\msbuild\14.0\Bin\MSBuild.exe"
::Run Code coverage with unit test in parallel
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" collect /output:"TestResults\VisualStudio.coverage" "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe" /testcontainer:"AHP.UnitTests\bin\Debug\AHP.UnitTests.dll" /resultsfile:TestResults/MSTestResults.trx
::Collect the results of code coverage and convert to xml
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" analyze /output:"TestResults\VisualStudio.coveragexml" "TestResults\VisualStudio.coverage"
MSBuild.SonarQube.Runner.exe end