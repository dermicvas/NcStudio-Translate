; =============================================================================
; NcStudioTranslate - Inno Setup Script
; =============================================================================
; Para compilar este instalador:
; 1. Baixe e instale o Inno Setup: https://jrsoftware.org/isdl.php
; 2. Abra este arquivo no Inno Setup Compiler
; 3. Clique em Build > Compile (Ctrl+F9)
; =============================================================================

#define MyAppName "NcStudio Translate"
#define MyAppVersion "1.0.0"
#define MyAppVersionInfo "1.0.0.0"
#define MyAppPublisher "dermicvas"
#define MyAppURL "https://github.com/dermicvas/NcStudio-Translate"
#define MyAppExeName "NcStudio Translate.exe"

[Setup]
; Identificador único da aplicação (gere um novo GUID para seu projeto)
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=no
; Caminho para o ícone do instalador (use o ico.ico do projeto)
SetupIconFile=..\NcStudioTranslate\src\Resources\ico.ico
; Ícone do desinstalador
UninstallDisplayIcon={app}\{#MyAppExeName}
; Imagem do assistente (opcional - 164x314 pixels)
; WizardImageFile=wizard.bmp
; Imagem pequena do assistente (opcional - 55x58 pixels)  
; WizardSmallImageFile=wizard-small.bmp
; Compressão
Compression=lzma2/ultra64
SolidCompression=yes
; Privilégios (para instalar em Program Files)
PrivilegesRequired=admin
; Informações de versão do instalador
VersionInfoVersion={#MyAppVersionInfo}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Instalador do {#MyAppName}
VersionInfoCopyright=Copyright © 2026 {#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersionInfo}
; Pasta de saída do instalador
OutputDir=..\installer\output
OutputBaseFilename=NcStudioTranslate_Setup_{#MyAppVersion}
; Estilo moderno do Windows
WizardStyle=modern
; Arquitetura
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
; Atalhos são criados automaticamente, sem checkbox opcional

[Files]
; Arquivos da aplicação (da pasta publish)
Source: "..\publish\NcStudio Translate.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publish\*.png"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\publish\ico.ico"; DestDir: "{app}"; Flags: ignoreversion
; Documentação
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LICENSE"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
; Atalho no Menu Iniciar (sempre criado)
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
; Atalho na Área de Trabalho (sempre criado)
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
; Opção para executar a aplicação após a instalação
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
; Associar extensão .resx com a aplicação (opcional)
; Root: HKCR; Subkey: ".resx\OpenWithProgids"; ValueType: string; ValueName: "NcStudioTranslate.resx"; ValueData: ""; Flags: uninsdeletevalue
; Root: HKCR; Subkey: "NcStudioTranslate.resx"; ValueType: string; ValueName: ""; ValueData: "Arquivo de Recursos .NET"; Flags: uninsdeletekey
; Root: HKCR; Subkey: "NcStudioTranslate.resx\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
; Root: HKCR; Subkey: "NcStudioTranslate.resx\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

