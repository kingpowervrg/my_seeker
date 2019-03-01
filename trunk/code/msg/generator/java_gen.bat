@echo off


::Java编译器路径
set JAVA_COMPILER_PATH=protoc.exe
::Java文件生成路径, 最后不要跟“\”符号
set JAVA_TARGET_PATH=../java_proto

::删除之前创建的文件

del /f /s /q %JAVA_TARGET_PATH%\*.* 

::遍历所有文件
for /f "delims=" %%i in ('dir /b "../proto/*.proto"') do ( 
    ::生成 Java 代码
    echo %JAVA_COMPILER_PATH%  --proto_path=../proto/ --java_out=%JAVA_TARGET_PATH% %%i
    %JAVA_COMPILER_PATH% --proto_path=../proto/ --java_out=%JAVA_TARGET_PATH% %%i
    
)

echo 协议生成完毕。

pause