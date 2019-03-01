echo -----------KEY HASH 生成-----------  
echo=  
echo=  
echo  当前工作路径为：%cd%  
echo -----------copy debug.keystore 到 jdk下------------
rem 拷贝目录 /s /e /y 说明：在复制文件的同时也复制空目录或子目录，如果目标路径已经有相同文件了，使用覆盖方式而不进行提示  
Xcopy D:\Android_SDK\.android\debug.keystore D:\Program Files\Java\jdk1.8.0_161\bin  /s /e /y  
rem CD切换不同盘符时候需要加上/d  
CD /D D:\  
CD D:\Program Files\Java\jdk1.8.0_161\bin
echo  当前工作路径为：%cd%  
echo -------------生成debug.txt 在 openssl\bin下 输入密码 android ----------
keytool -exportcert -alias androiddebugkey -keystore debug.keystore > D:\openssl-0.9.8k_X64\bin\debug.txt 
echo -------------debug.txt生成完毕-----------------------  
CD D:\openssl-0.9.8k_X64\bin
echo  当前工作路径为：%cd% 
echo -------------生成debug_sha.txt-----------------------
openssl sha1 -binary debug.txt >debug_sha.txt
echo -------------debug_sha.txt生成完毕-----------------------  

echo -------------生成debug_base64.txt-----------------------
openssl base64 -in debug_sha.txt >debug_base64.txt
echo -------------debug_base64.txt生成完毕-----------------------  

echo 结束

PAUSE