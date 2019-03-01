set curdir=%~dp0
cd /d %curdir%
cd Utf8Json.UniversalCodeGenerator\win-x64
del "%curdir%\..\client\Seeker\Assets\Scripts\Data\JsonData\Utf8JsonGenerated.cs"

Utf8Json.UniversalCodeGenerator.exe -d "%curdir%\..\client\Seeker\Assets\Scripts\Data\JsonData" -o "%curdir%\..\client\Seeker\Assets\Scripts\Data\JsonData\Utf8JsonGenerated.cs"
pause