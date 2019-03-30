@echo off

if exist Flatbuffer (   
 rd /s /Q Flatbuffer
)

md Flatbuffer

java -jar antlr-4.7.1-complete.jar -o Flatbuffer -Dlanguage=CSharp -visitor Flatbuffer.g4

if exist Json (
	rd /s /Q Json
)

md Json

java -jar antlr-4.7.1-complete.jar -o Json -Dlanguage=CSharp -visitor Json.g4

pause  