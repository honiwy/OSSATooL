@ECHO off

SET ADB_PATH=%cd%\adb

%ADB_PATH%\adb.exe devices
%ADB_PATH%\adb.exe wait-for-device
%ADB_PATH%\adb.exe shell "LD_LIBRARY_PATH=/vendor/lib64/hw KmInstallKeybox /data/keybox_6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4.xml 6e81d3a6-61d4-46d2-bed4-77d43e2a2eb4 true"

PAUSE