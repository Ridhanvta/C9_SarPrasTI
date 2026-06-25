; Script Inno Setup untuk Manajemen Sarpras TI
; Pastikan file "CRRuntime_64bit_13_0_30.msi" diletakkan satu folder dengan file .iss ini sebelum melakukan kompilasi Inno Setup.

[Setup]
AppName=Sistem Manajemen SarPras TI
AppVersion=1.0.0
DefaultDirName={pf}\ManajemenSarPras
DefaultGroupName=Manajemen SarPras TI
OutputDir=OutputInstaller
OutputBaseFilename=Setup_SarPrasTI
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
; 1. Menyalin semua file hasil build (termasuk .exe, ExcelDataReader.dll, dll)
Source: "bin\Debug\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; 2. Membungkus installer Crystal Reports (SAP) ke dalam file temporary saat instalasi
; Ubah nama file di bawah ini jika versi CRRuntime Anda berbeda.
Source: "CR13SP40MSI64_0-80007712.MSI"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall

[Icons]
Name: "{group}\Sistem Manajemen SarPras TI"; Filename: "{app}\ManajemenSarPras.exe"
Name: "{commondesktop}\Sistem Manajemen SarPras TI"; Filename: "{app}\ManajemenSarPras.exe"

[Run]
; 3. Mengeksekusi instalasi Crystal Reports secara otomatis (Silent/Passive mode)
Filename: "msiexec.exe"; Parameters: "/i ""{tmp}\CR13SP40MSI64_0-80007712.MSI"" /passive /norestart"; Description: "Menginstall komponen SAP Crystal Reports..."; StatusMsg: "Memasang dependensi laporan... (Tunggu sebentar)"; Flags: waituntilterminated runascurrentuser

; 4. Menjalankan aplikasi setelah instalasi selesai
Filename: "{app}\ManajemenSarPras.exe"; Description: "Jalankan Sistem Manajemen SarPras TI"; Flags: nowait postinstall skipifsilent
