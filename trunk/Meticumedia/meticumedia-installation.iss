; meticumedia setup script for Inno Setup Compiler

[Setup]
AppName=meticumedia
AppVersion=0.9.3
DefaultDirName={pf}\meticumedia
DefaultGroupName=meticumedia
UninstallDisplayIcon={app}\Meticumedia.exe
Compression=lzma2
SolidCompression=yes
OutputDir=Install

[Files]
Source: "Meticumedia.exe"; DestDir: "{app}"; Flags: ignoreversion;
Source: "Ionic.Zip.dll"; DestDir: "{app}"; Flags: ignoreversion;
Source: "Ookii.Dialogs.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion;
Source: "Xceed.Wpf.Toolkit.dll"; DestDir: "{app}"; Flags: ignoreversion;
Source: "wordsEn.txt"; DestDir: "{app}"; Flags: ignoreversion;
Source: "license.txt"; DestDir: "{app}"; Flags: ignoreversion;

[Icons]
Name: "{group}\meticumedia"; Filename: "{app}\Meticumedia.exe"
Name: "{commondesktop}\meticumedia"; Filename: "{app}\Meticumedia.exe"
