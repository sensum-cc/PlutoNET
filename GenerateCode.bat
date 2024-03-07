@echo off
dotnet run --project ./src/CodeGenerator/CodeGenerator.csproj
echo Generated code. Moving generated files to PlutoNET project
for %%f in (*.cs) do move /Y "%%f" "%~dp0src\PlutoNET\GeneratedCode\%%~nxf"
for %%f in (*.cpp) do del "%%f"
echo Everything ready. Press any key to close..
pause >nul