@ECHO off

SET ADB_PATH=%cd%\adb

SET QFIL_PATH="C:\Program Files (x86)\Qualcomm\QPST\bin"
SET QFIL_LOG_PATH=C:\QFIL\log\flat_log.txt
SET QFIL_COM_PORT=7

SET TARGET_FW_VER=Cabello_OSSA_130s_2021_01_27_133_debug
<<<<<<< HEAD
SET FW_PATH=C:\Users\avc\Desktop\Cabello_SR2_2021_05_03_62_release_Flat
=======
SET FW_PATH=C:\Users\avc\Desktop\Cabello_SR2_2021_05_06_66_release_Flat
>>>>>>> 445ad1982fefbb42fbe62933601e7532b1589a5e

SET TARGET_SKU_ID=Generic


REM flash device firmware
%QFIL_PATH%\qfil.exe -Mode=3 -downloadflat -COM=%QFIL_COM_PORT% -Programmer=true;"%FW_PATH%\emmc\prog_firehose_ddr.elf" -deviceType="emmc" - VALIDATIONMODE=2 -SWITCHTOFIREHOSETIMEOUT=50 -RESETTIMEOUT=500 -RESETDELAYTIME=5 -RESETAFTERDOWNLOAD=true -MaxPayloadSizeToTargetInBytes=true;49152 -searchpath="%FW_PATH%\emmc" -Rawprogram="rawprogram_unsparse.xml" -Patch="patch0.xml" -logfilepath="%QFIL_LOG_PATH%"

REM wait device boot
%ADB_PATH%\adb.exe devices
%ADB_PATH%\adb.exe wait-for-device

REM check device firmware version
FOR /F "tokens=* USEBACKQ" %%F IN (`%ADB_PATH%\adb.exe shell getprop ro.build.version.incremental`) DO (
SET device_fw_ver=%%F
)

ECHO Device firmware version: %device_fw_ver%
IF %device_fw_ver% == %TARGET_FW_VER% (ECHO Check firmware version: SUCCESS) ELSE ECHO Check firmware version: FAIL

REM check device SKU ID
FOR /F "tokens=* USEBACKQ" %%F IN (`%ADB_PATH%\adb.exe shell su 0 cat /avc_info/sku_id`) DO (
SET device_sku_id=%%F
)

ECHO Device SKU ID: %device_sku_id%
IF %device_sku_id% == %TARGET_SKU_ID% (ECHO Check SKU ID: SUCCESS) ELSE ECHO Check SKU ID: FAIL

PAUSE