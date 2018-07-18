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

exprCall : names+=IDENT (op = DOT names+=IDENT)* BRACEL (args+=expr (COMMA args+=expr)*) BRACER;
exprProp : names+=IDENT (op = DOT names+=IDENT)*;

exprMinus : op = SUB value = expr;
exprConvert : BRACEL type = IDENT BRACER value = expr;
exprIncrement : op = INCREMENT  value = expr | value = expr op = INCREMENT;
exprDecrement : op = DECREMENT  value = expr | value = expr op = DECREMENT;
exprLogicNot : op = NOT value = expr;
exprBitInvert : op = INVERT value = expr;

exprMul : l = expr op = MUL r = expr;
exprDiv : l = expr op = DIV r = expr;
exprMod : l = expr op = MOD r = expr;

exprAdd : l = expr op = ADD r = expr;
exprSub : l = expr op = SUB r = expr;

exprShiftL : l = expr op = SHIFTL r = expr;
exprShiftR : l = expr op = SHIFTR r = expr;

exprComma : values+=expr (COMMA values+=expr)+;

value : integerValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | stringValue = STRING;

if : id=IF l=BRACEL exp = expr r=BRACER THEN (expr|TEXT)* END;


