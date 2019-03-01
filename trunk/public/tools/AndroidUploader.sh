#!/bin/bash
#cd ~/Documents/Seeker/conan/trunk/release/android
#cd ./../release/android
cd ./../../release/android

files=`ls *.apk`
for file in ${files};do
	/usr/local/bin/sshpass -p "123456" scp ${file} root@10.0.107.229:/opt/package
done