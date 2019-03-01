echo -----------KEY HASH 生成-----------  
echo=  
echo=  
echo  当前工作路径为：%cd%  
echo -----------copy fotoyehan.keystore 到 jdk下------------
rem 拷贝目录 /s /e /y 说明：在复制文件的同时也复制空目录或子目录，如果目标路径已经有相同文件了，使用覆盖方式而不进行提示  
Xcopy D:\work\seeker\keystore\fotoyehan.keystore "D:\Program Files\Java\jdk1.8.0_161\bin" /s /e /y /i
rem CD切换不同盘符时候需要加上/d  
CD /D D:\  
CD D:\Program Files\Java\jdk1.8.0_161\bin
echo  当前工作路径为：%cd%  
echo -------------生成fotoyehan.txt 在 openssl\bin下 输入密码 fotoyehan ----------
keytool -exportcert -alias yehan -keystore fotoyehan.keystore > D:\openssl-0.9.8k_X64\bin\fotoyehan.txt 
echo -------------fotoyehan.txt生成完毕-----------------------  
CD D:\openssl-0.9.8k_X64\bin
echo  当前工作路径为：%cd% 
echo -------------生成fotoyehan_sha.txt-----------------------
openssl sha1 -binary fotoyehan.txt >fotoyehan_sha.txt
echo -------------fotoyehan_sha.txt生成完毕-----------------------  

echo -------------生成fotoyehan_base64.txt-----------------------
openssl base64 -in fotoyehan_sha.txt >fotoyehan_base64.txt
echo -------------fotoyehan_base64.txt生成完毕-----------------------  

echo 结束

PAUSE