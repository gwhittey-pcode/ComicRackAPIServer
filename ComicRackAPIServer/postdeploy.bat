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










tablet
about.html
BCRPlugin.zip
BCRVersion.txt
ComboTreeControls.dll
ComicRackAPIServer_icon.png
ComicRackAPIServer.dll
Imazen.WebP.dll
libwebp_x64.dll
libwebp_x86.dll
libwebp.dll
Linq2Rest.dll
Nancy.Authentication.Stateless.dll
Nancy.dll
Nancy.Hosting.Self.dll
Package.ini
Program.py
SQLite.Interop.32bit.dll
SQLite.Interop.64bit.dll
SQLite.Interop.dll
System.Data.SQLite.dll
System.Data.SQLite.Linq.dll