#define AppName "ZYC.Framework"

#ifndef Version
    Abort()
#endif

#ifndef Author
    Abort()
#endif

#ifndef Copyright
    Abort()
#endif

[Setup]
AppId={#AppName}
AppName={#AppName}
AppVersion={#Version}
AppPublisher={#Author}
WizardStyle=modern
DefaultDirName={userpf}\{#AppName}
UninstallDisplayIcon={app}\{#AppName}.exe
OutputBaseFilename={#AppName}.Setup
AppCopyright={#Copyright}
PrivilegesRequired=admin

SetupIconFile=app.ico
DisableWelcomePage=yes
DisableReadyPage=yes
DisableProgramGroupPage=yes

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppName}.exe"

[Files]
Source: "_bin\*"; DestDir: "{app}\"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppName}.exe"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppName}.exe"