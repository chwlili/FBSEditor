@echo off

if exist Template (   
 rd /s /Q Template
)

md Template

java -jar antlr-4.7.1-complete.jar -o Template -Dlanguage=CSharp -visitor Template.g4

pause  