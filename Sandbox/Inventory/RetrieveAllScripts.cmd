@echo off
setlocal enabledelayedexpansion

REM Set the source folder to the current directory
set "source_folder=%cd%"

REM Create a temporary file to store the combined content
set "temp_file=%temp%\combined_content.txt"

REM Delete the temporary file if it already exists
if exist "%temp_file%" del "%temp_file%"

REM Loop through all .cs files in the source folder and its subfolders
for /r "%source_folder%" %%f in (*.cs) do (
    REM Get the base name of the file (without extension)
    set "base_name=%%~nf"
    REM Append the class name header
    echo // !base_name!.cs >> "%temp_file%"
    
    REM Append the contents of each .cs file to the temporary file
    echo Processing file: %%f
    type "%%f" >> "%temp_file%"
    
    REM Add a newline between files
    echo. >> "%temp_file%"
)

REM Copy the combined content to the clipboard
type "%temp_file%" | clip

REM Delete the temporary file
del "%temp_file%"

echo All .cs files have been combined and copied to the clipboard.

endlocal