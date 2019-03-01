#!/bin/sh
echo on
. ConfigDownloader.sh

cd CSharpPack/bin 
/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono CSharpPack.exe

