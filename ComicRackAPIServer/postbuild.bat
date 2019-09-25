xcopy "%~1FreeImage\FreeImage.32bit.dll" "%~2" /y
xcopy "%~1FreeImage\FreeImage.64bit.dll" "%~2" /y
xcopy "%~1SQLite\SQLite.Interop.32bit.dll" "%~2" /y
xcopy "%~1SQLite\SQLite.Interop.64bit.dll" "%~2" /y

xcopy "%~1libwebp\libwebp_x86.dll" "%~2" /y
xcopy "%~1libwebp\libwebp_x86.dll" "%~2" /y

IF NOT EXIST "%~2\tablet\index.html" GOTO copy_tablet

exit /b 0

:copy_tablet
rem md "%~2\tablet"
rem xcopy "%~1tablet\*.*" "%~2\tablet" /y /E
mklink /D /J "%~2\tablet" "%~1tablet"