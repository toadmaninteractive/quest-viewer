@echo off

bin\igorc.exe ^
    -v ^
    -cs ^
    -target-version 6.0 ^
    -p "igor\main" ^
    -o "generated" ^
    *.igor

if errorlevel 1 pause
