@echo off

::cd into the folder
cd /d %GAME_PROJECT_OUTPUTGITFOLDER%

::
for /f "delims=" %%i in ('git describe --abbrev=0 --tags ') do set b=%%i
::
echo %b%

pause

exit 0