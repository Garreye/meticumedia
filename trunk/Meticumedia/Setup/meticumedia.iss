; meticumedia setup script for Inno Setup Compiler

[Setup]
AppName=meticumedia
AppVersion=1.8.1
DefaultDirName={pf}\meticumedia
DefaultGroupName=meticumedia
UninstallDisplayIcon={app}\Meticumedia.exe
Compression=lzma2
SolidCompression=yes
OutputDir=Install

[Files]
Source: "Meticumedia.exe"; DestDir: "{app}"; Flags: ignoreversion;
Source: "wordsEn.txt"; DestDir: "{app}"; Flags: ignoreversion;
Source: "license.txt"; DestDir: "{app}"; Flags: ignoreversion;

[Icons]
Name: "{group}\meticumedia"; Filename: "{app}\Meticumedia.exe"
Name: "{commondesktop}\meticumedia"; Filename: "{app}\Meticumedia.exe"