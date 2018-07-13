@echo off

if exist TemplateLexer (   
 rd /s /Q TemplateLexer
)

md TemplateLexer

java -jar antlr-4.7.1-complete.jar -o TemplateLexer -Dlanguage=CSharp -visitor TemplateLexer.g4

@echo off
if exist TemplateLexer\TemplateLexer.tokens (
	copy /Y TemplateLexer\TemplateLexer.tokens TemplateLexer.tokens
)

pause  