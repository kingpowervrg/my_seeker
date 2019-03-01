::生成客户端消息

set CLIENT_DIR=..\client\Seeker\Assets\Scripts\Game\Message\

::生成descriptor 
.\generator\protoc.exe -I=proto --descriptor_set_out=.\generator\msg.desc.bin msg.proto --csharp_out=%CLIENT_DIR%

copy .\generator\msg.desc.bin .\generator\metadata\msg.desc.bin
del .\generator\msg.desc.bin

del .\generator\DescriptorProtoFile.cs

::move .\generator\Options.cs %CLIENT_DIR%\Options.cs

copy .\proto\msg.proto .\generator\msg.proto
copy .\proto\options.proto .\generator\options.proto

PUSHD generator
::生成CSharp .\proto\msg.proto
ProtoGen.exe -output_directory=.\ google\protobuf\descriptor.proto .\options.proto msg.proto --protoc_dir=.\   
rem move .\Msg.cs ..\%CLIENT_DIR%\Msg.cs
del  .\options.proto
del  .\msg.proto

PUSHD bin

CSharpProtocolGenerator.exe ..\metadata\msg.desc.bin ..\msg.proto ..\generatedCode

POPD

copy .\generatedCode\*.cs ..\%CLIENT_DIR%


::build.bat

pause