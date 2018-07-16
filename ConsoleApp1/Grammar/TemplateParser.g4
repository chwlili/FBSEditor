parser grammar TemplateParser;
options { tokenVocab = TemplateLexer;}

document : (TEXT | expr | if)*;

expr :
		 props+=IDENT (op=DOT props+=IDENT)*
	   | BRACEL brace=expr BRACER
	   | l=expr op=(MUL|DIV) r=expr 
	   | l=expr op=(ADD|SUB) r=expr
	   | op=(ADD2|SUB2) r=expr
	   | op=(ADD|SUB) r=expr
	   | v=value;

value : integerValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | stringValue = STRING;

if : id=IF l=BRACEL exp = expr r=BRACER THEN (expr|TEXT)* END;


