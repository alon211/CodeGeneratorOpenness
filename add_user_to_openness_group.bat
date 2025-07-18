@echo off
REM Batch file to automatically add current computer user to openness user group
REM Requires administrator privileges to run

echo Adding current computer user to openness user group...
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

REM Check if openness user group exists, create if not exists
net localgroup openness >nul 2>&1
if %errorlevel% neq 0 (
    echo Openness user group does not exist, creating...
    net localgroup openness /add
    if %errorlevel% equ 0 (
        echo Openness user group created successfully!
    ) else (
        echo Failed to create openness user group! Please check permissions.
        pause
        exit /b 1
    )
) else (
    echo Openness user group already exists.
)
echo.

REM Add target user to openness user group
echo Adding user %TARGET_USER% to openness user group...
net localgroup openness "%TARGET_USER%" /add

if %errorlevel% equ 0 (
    echo Success! User %TARGET_USER% has been added to openness user group.
    echo.
    echo Note: Changes will take effect on next login, or you can restart the computer for immediate effect.
) else (
    echo Failed! Unable to add user to openness user group.
    echo Possible reasons:
    echo 1. No administrator privileges
    echo 2. User is already in the group
    echo 3. Username does not exist
    echo 4. Domain user format may need adjustment
    echo.
    echo Trying alternative method with local username...
    net localgroup openness "%USERNAME%" /add
    if %errorlevel% equ 0 (
        echo Success! Local user %USERNAME% has been added to openness user group.
    ) else (
        echo Failed with both methods. Please check user permissions and group existence.
    )
)

echo.
echo Press any key to exit...
pause >nul