bin\igorc.exe ^
    -d ^
    -v ^
    -schema ^
    -set enabled ^
    -set root_type='AnalyticsEvent' ^
    -output-file analytics_schema.json ^
    -p "igor\analytics" ^
    analytics.igor

if errorlevel 1 pause
