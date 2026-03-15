@echo off
setlocal enabledelayedexpansion

:: --- Configuration ---
set "REPO_ROOT=%~dp0"
:: Remove trailing backslash
set "REPO_ROOT=%REPO_ROOT:~0,-1%"
set "PUBLISH_DIR=%REPO_ROOT%\Metrobones\bin\Release\net9.0\publish\wwwroot"
set "BRANCH=pwa-publish"
set "REMOTE=origin"
set "TEMP_INDEX=%REPO_ROOT%\.git\tmp-pwa-index"

:: --- Parse arguments ---
set "MESSAGE="
set "NO_PUSH=0"

:parse_args
if "%~1"=="" goto done_args
if /i "%~1"=="--no-push" ( set "NO_PUSH=1" & shift & goto parse_args )
if /i "%~1"=="-m" ( set "MESSAGE=%~2" & shift & shift & goto parse_args )
if /i "%~1"=="--message" ( set "MESSAGE=%~2" & shift & shift & goto parse_args )
shift
goto parse_args
:done_args

if "%MESSAGE%"=="" (
    for /f "tokens=*" %%d in ('powershell -NoProfile -Command "Get-Date -Format \"yyyy-MM-dd HH:mm:ss\""') do set "MESSAGE=Publish PWA - %%d"
)

:: --- Build ---
echo Building Release publish...
dotnet publish "%REPO_ROOT%\Metrobones" --configuration Release
if errorlevel 1 (
    echo ERROR: dotnet publish failed.
    exit /b 1
)

:: --- Validation ---
if not exist "%PUBLISH_DIR%" (
    echo ERROR: Publish directory not found: %PUBLISH_DIR%
    exit /b 1
)

set "FILE_COUNT=0"
for /f %%f in ('dir /s /b /a-d "%PUBLISH_DIR%" 2^>nul ^| find /c /v ""') do set "FILE_COUNT=%%f"
if "%FILE_COUNT%"=="0" (
    echo ERROR: Publish directory is empty: %PUBLISH_DIR%
    exit /b 1
)

echo Publishing %FILE_COUNT% files from:
echo   %PUBLISH_DIR%
echo to branch '%BRANCH%'...

:: --- Clean up leftover temp index ---
if exist "%TEMP_INDEX%" del /f "%TEMP_INDEX%"

:: --- Use a temporary index ---
set "GIT_INDEX_FILE=%TEMP_INDEX%"

:: Check if branch exists and get parent commit
set "PARENT_COMMIT="
set "PARENT_ARGS="

for /f "tokens=*" %%h in ('git -C "%REPO_ROOT%" rev-parse --verify "refs/heads/%BRANCH%" 2^>nul') do set "PARENT_COMMIT=%%h"

if defined PARENT_COMMIT (
    set "PARENT_ARGS=-p %PARENT_COMMIT%"
    git -C "%REPO_ROOT%" read-tree %PARENT_COMMIT%
    if errorlevel 1 goto cleanup_fail
)

:: Stage all files from publish directory
git --work-tree="%PUBLISH_DIR%" --git-dir="%REPO_ROOT%\.git" add -A
if errorlevel 1 goto cleanup_fail

:: Check for changes
if defined PARENT_COMMIT (
    git -C "%REPO_ROOT%" diff-index --cached --quiet %PARENT_COMMIT% >nul 2>&1
    if not errorlevel 1 (
        echo No changes to publish - branch is already up to date.
        goto cleanup_ok
    )
)

:: Create tree
for /f "tokens=*" %%t in ('git -C "%REPO_ROOT%" write-tree') do set "TREE=%%t"
if not defined TREE goto cleanup_fail
echo Tree:   %TREE%

:: Create commit
for /f "tokens=*" %%c in ('echo %MESSAGE% ^| git -C "%REPO_ROOT%" commit-tree %TREE% %PARENT_ARGS%') do set "COMMIT=%%c"
if not defined COMMIT goto cleanup_fail
echo Commit: %COMMIT%

:: Update branch ref
git -C "%REPO_ROOT%" update-ref "refs/heads/%BRANCH%" %COMMIT%
if errorlevel 1 goto cleanup_fail
echo Updated refs/heads/%BRANCH% -^> %COMMIT%

:: --- Cleanup temp index ---
set "GIT_INDEX_FILE="
if exist "%TEMP_INDEX%" del /f "%TEMP_INDEX%"

:: --- Push ---
if "%NO_PUSH%"=="1" (
    echo Done!
    echo Push manually with: git push %REMOTE% %BRANCH%
    exit /b 0
)

echo Pushing to %REMOTE%/%BRANCH%...
git -C "%REPO_ROOT%" push %REMOTE% %BRANCH% --force-with-lease
if errorlevel 1 (
    echo ERROR: Push failed. Retry with: git push %REMOTE% %BRANCH%
    exit /b 1
)
echo Done!
exit /b 0

:cleanup_fail
set "GIT_INDEX_FILE="
if exist "%TEMP_INDEX%" del /f "%TEMP_INDEX%"
echo ERROR: Failed during git operations.
exit /b 1

:cleanup_ok
set "GIT_INDEX_FILE="
if exist "%TEMP_INDEX%" del /f "%TEMP_INDEX%"
exit /b 0
