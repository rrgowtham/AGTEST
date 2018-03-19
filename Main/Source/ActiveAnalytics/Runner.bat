SonarQube.Scanner.MSBuild.exe begin /key:"ahm:active-analytics:dev" /name:"active-analytics" /s:C:\Source\ActiveAnalytics\Main\Source\ActiveAnalytics\SonarQube.Analysis.xml
"C:\Program Files (x86)\msbuild\14.0\Bin\MSBuild.exe" /t:Rebuild
SonarQube.Scanner.MSBuild.exe end