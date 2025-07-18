@echo off
REM Batch file to automatically add current computer user to Siemens TIA Openness user group
REM Requires administrator privileges to run

REM Check if running as administrator
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: This script must be run as Administrator!
    echo Please right-click and select "Run as administrator"
    pause
    exit /b 1
)

echo Adding current computer user to Siemens TIA Openness user group...
echo.

REM Get current computer username
set CURRENT_USER=%COMPUTERNAME%\%USERNAME%
echo Current computer user: %CURRENT_USER%
echo.

REM Also get domain user if exists
for /f "tokens=2 delims==" %%i in ('wmic computersystem get username /value ^| find "="') do set DOMAIN_USER=%%i
if defined DOMAIN_USER (
    echo Domain user detected: %DOMAIN_USER%
    set TARGET_USER=%DOMAIN_USER%
) else (
    set TARGET_USER=%USERNAME%
)
echo Target user to add: %TARGET_USER%
echo.

REM Check if Siemens TIA Openness user group exists, create if not exists
echo Checking if Siemens TIA Openness user group exists...
net localgroup "Siemens TIA Openness" >nul 2>&1
if %errorlevel% neq 0 (
    echo Siemens TIA Openness user group does not exist, creating...
    net localgroup "Siemens TIA Openness" /add /comment:"Siemens TIA Portal Openness API user group"
    if %errorlevel% equ 0 (
        echo Siemens TIA Openness user group created successfully!
    ) else (
        echo Failed to create Siemens TIA Openness user group! Please check permissions.
        pause
        exit /b 1
    )
) else (
    echo Siemens TIA Openness user group already exists.
)
echo.

REM Check if user is already in the group
net localgroup "Siemens TIA Openness" | findstr /i "%TARGET_USER%" >nul 2>&1
if %errorlevel% equ 0 (
    echo User %TARGET_USER% is already in the Siemens TIA Openness group.
    echo No action needed.
    goto :end
)

REM Add target user to Siemens TIA Openness user group
echo Adding user %TARGET_USER% to Siemens TIA Openness user group...
net localgroup "Siemens TIA Openness" "%TARGET_USER%" /add

if %errorlevel% equ 0 (
    echo Success! User %TARGET_USER% has been added to Siemens TIA Openness user group.
    echo.
    echo Note: Changes will take effect on next login, or you can restart the computer for immediate effect.
) else (
    echo Failed! Unable to add user to Siemens TIA Openness user group.
    echo Possible reasons:
    echo 1. No administrator privileges
    echo 2. Username does not exist
    echo 3. Domain user format may need adjustment
    echo.
    echo Trying alternative method with local username...
    net localgroup "Siemens TIA Openness" "%USERNAME%" /add
    if %errorlevel% equ 0 (
        echo Success! Local user %USERNAME% has been added to Siemens TIA Openness user group.
    ) else (
        echo Failed with both methods. Manual troubleshooting required.
        echo.
        echo Current user information:
        echo - Local username: %USERNAME%
        echo - Computer name: %COMPUTERNAME%
        echo - Domain user: %DOMAIN_USER%
        echo.
        echo Please try manually with: net localgroup "Siemens TIA Openness" "[username]" /add
    )
)

:end
echo.
echo Displaying current Siemens TIA Openness group members:
net localgroup "Siemens TIA Openness"
echo.
echo Press any key to exit...
pause >nul