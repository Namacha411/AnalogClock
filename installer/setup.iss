#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif
#ifndef SourceDir
  #define SourceDir "..\publish"
#endif
#ifndef OutputDir
  #define OutputDir "..\dist"
#endif

[Setup]
AppId={{609A46C0-F3C8-496B-92AC-8D92324DFD1C}
AppName=AnalogClock
AppVersion={#AppVersion}
AppPublisher=Tanaka Hideyuki
AppPublisherURL=https://github.com/Namacha411/AnalogClock/
AppSupportURL=https://github.com/Namacha411/AnalogClock/
AppUpdatesURL=https://github.com/Namacha411/AnalogClock/
DefaultDirName={localappdata}\Programs\AnalogClock
DefaultGroupName=AnalogClock
PrivilegesRequired=lowest
DisableProgramGroupPage=yes
OutputDir={#OutputDir}
OutputBaseFilename=AnalogClock-{#AppVersion}-win-x64-setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
LicenseFile=..\LICENSE
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\AnalogClock"; Filename: "{app}\AnalogClock.exe"
Name: "{group}\{cm:UninstallProgram,AnalogClock}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\AnalogClock"; Filename: "{app}\AnalogClock.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\AnalogClock.exe"; Description: "{cm:LaunchProgram,AnalogClock}"; Flags: nowait postinstall skipifsilent
