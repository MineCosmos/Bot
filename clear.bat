@echo off
setlocal

REM 删除当前目录下的"bin"和"obj"文件夹
if exist "%CD%\bin" (
    rmdir /s /q "%CD%\bin"
)
if exist "%CD%\obj" (
    rmdir /s /q "%CD%\obj"
)

REM 递归删除当前目录下的子级目录中的"bin"和"obj"文件夹
for /d /r %%G in (*) do (
    if exist "%%G\bin" (
        rmdir /s /q "%%G\bin"
    )
    if exist "%%G\obj" (
        rmdir /s /q "%%G\obj"
    )
)

echo 删除完成！
pause