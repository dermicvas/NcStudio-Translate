@echo off
REM =============================================================================
REM Build and Create Installer Script
REM =============================================================================
echo.
echo ============================================
echo   NcStudio Translate - Build Installer
echo ============================================
echo.

REM Define paths
set "PROJECT_DIR=%~dp0"
set "INNO_SETUP="

REM Try common Inno Setup locations (system + per-user install)
if exist "%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" set "INNO_SETUP=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
if "%INNO_SETUP%"=="" if exist "%ProgramFiles%\Inno Setup 6\ISCC.exe" set "INNO_SETUP=%ProgramFiles%\Inno Setup 6\ISCC.exe"
if "%INNO_SETUP%"=="" if exist "%LocalAppData%\Programs\Inno Setup 6\ISCC.exe" set "INNO_SETUP=%LocalAppData%\Programs\Inno Setup 6\ISCC.exe"

REM Convert to short path to avoid parentheses parsing issues in batch
if not "%INNO_SETUP%"=="" for %%I in ("%INNO_SETUP%") do set "INNO_SETUP=%%~sI"

REM Step 1: Clean previous publish
echo [1/3] Limpando publicacao anterior...
if exist "%PROJECT_DIR%publish" rmdir /s /q "%PROJECT_DIR%publish"

REM Step 2: Publish application
echo [2/3] Publicando aplicacao...
dotnet publish "%PROJECT_DIR%NcStudioTranslate\NcStudioTranslate.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false -o "%PROJECT_DIR%publish"

if %ERRORLEVEL% NEQ 0 (
    echo ERRO: Falha ao publicar a aplicacao!
    pause
    exit /b 1
)

REM Garantir que recursos usados em runtime estejam junto do .exe
if exist "%PROJECT_DIR%NcStudioTranslate\src\Resources\ico.ico" copy /y "%PROJECT_DIR%NcStudioTranslate\src\Resources\ico.ico" "%PROJECT_DIR%publish\ico.ico" >nul
if exist "%PROJECT_DIR%NcStudioTranslate\src\Resources\logo.png" copy /y "%PROJECT_DIR%NcStudioTranslate\src\Resources\logo.png" "%PROJECT_DIR%publish\logo.png" >nul

REM Step 3: Build installer
echo [3/3] Criando instalador...
if exist "%INNO_SETUP%" (
    "%INNO_SETUP%" "%PROJECT_DIR%installer\NcStudioTranslate.iss"
    if %ERRORLEVEL% NEQ 0 (
        echo ERRO: Falha ao criar o instalador!
        pause
        exit /b 1
    )
    echo.
    echo ============================================
    echo   Instalador criado com sucesso!
    echo   Local: installer\output\
    echo ============================================
) else (
    echo.
    echo AVISO: Inno Setup nao encontrado em:
    echo %INNO_SETUP%
    echo.
    echo Instale o Inno Setup de: https://jrsoftware.org/isdl.php
    echo Ou compile manualmente o arquivo: installer\NcStudioTranslate.iss
)

echo.
pause
