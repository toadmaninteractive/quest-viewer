@echo off

bin\igorc.exe ^
    -v ^
    -schema ^
    -p "igor\main" ^
    *.igor

if errorlevel 1 pause
