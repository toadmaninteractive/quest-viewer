bin\igorc.exe ^
    -d ^
    -cs ^
    -p "igor\analytics" ^
    -o ..\QuestViewer\QuestViewer\Service\Scylla ^
    *.igor

if errorlevel 1 pause
