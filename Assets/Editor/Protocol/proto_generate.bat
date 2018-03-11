color 0A && echo off

rem protoc³ÌÐòÃû
set "PROTOC_EXE=protoc.exe"

%PROTOC_EXE% --version

set "WORK_DIR=%cd%"
set "CS_OUT_PATH=%WORK_DIR%\CS"

for /f "delims=" %%i in ('dir /b "*.proto"') do (
	echo.generate %%i
	"%PROTOC_EXE%" --proto_path="%WORK_DIR%" --csharp_out="%CS_OUT_PATH%" %%i
)

pause