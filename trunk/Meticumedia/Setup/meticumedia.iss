; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

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
Source: "Meticumedia.exe"; DestDir: "{app}"
Source: "wordsEn.txt"; DestDir: "{app}"
Source: "license.txt"; DestDir: "{app}";

[Icons]
Name: "{group}\meticumedia"; Filename: "{app}\Meticumedia.exe"
Name: "{commondesktop}\My Program"; Filename: "{app}\Meticumedia.exe"