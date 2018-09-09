@echo off

if exist Template (   
 rd /s /Q Template
)

md Template

java -jar antlr-4.7.1-complete.jar -o Template -Dlanguage=CSharp -visitor TemplateLexer.g4

if exist Template\TemplateLexer.tokens (
	copy /Y Template\TemplateLexer.tokens TemplateLexer.tokens
)

java -jar antlr-4.7.1-complete.jar -o Template -Dlanguage=CSharp -visitor TemplateParser.g4

pause  