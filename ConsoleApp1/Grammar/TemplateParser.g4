parser grammar TemplateParser;
options { tokenVocab = TemplateLexer;}

document : (TEXT | expr | if)*;

expr : exprCall | exprProp
	   | exprMinus | exprConvert | exprIncrement | exprDecrement | exprLogicNot | exprBitInvert
	   | exprMul |exprDiv | exprMod 
	   | exprAdd | exprSub
	   | exprBitShiftL | exprBitShiftR 
	   | exprLess | exprLessEqual | exprGreater | exprGreaterEqual 
	   | exprEqual | exprNotEqual
	   | exprBitAnd 
	   | exprBitXor
	   | exprBitOr
	   | exprLogicAnd
	   | exprLogicOr
	   | v=value;

//�����뺯��
exprCall : names+=IDENT (op = DOT names+=IDENT)* BRACEL (args+=expr (COMMA args+=expr)*) BRACER;
exprProp : names+=IDENT (op = DOT names+=IDENT)*;

//
exprMinus : op = SUB v = expr;
exprConvert : BRACEL type = IDENT BRACER v = expr;

//��������
exprMul : l = expr op = MUL r = expr;
exprDiv : l = expr op = DIV r = expr;
exprMod : l = expr op = MOD r = expr;
exprAdd : l = expr op = ADD r = expr;
exprSub : l = expr op = SUB r = expr;
exprIncrement : op = INCREMENT  v = expr | v = expr op = INCREMENT;
exprDecrement : op = DECREMENT  v = expr | v = expr op = DECREMENT;

//λ����
exprBitShiftL : l = expr op = SHIFTL r = expr;
exprBitShiftR : l = expr op = SHIFTR r = expr;
exprBitInvert : op = INVERT v = expr;
exprBitAnd : l = expr op = AND r = expr;
exprBitOr : l = expr op = OR r = expr;
exprBitXor : l = expr op = XOR r = expr;

//�Ƚ�����
exprLess : l = expr op = LESS r = expr;
exprGreater : l = expr op = GREATER r = expr;
exprLessEqual : l = expr op = LESSEQUAL r = expr;
exprGreaterEqual : l = expr op = GREATER r = expr;
exprEqual : l = expr op = EQUAL r = expr;
exprNotEqual : l = expr op = NOTEQUAL r = expr;

//�߼�����
exprLogicAnd : l = expr op = AND2 r = expr;
exprLogicOr : l = expr op = OR2 r = expr;
exprLogicNot : op = NOT v = expr;

exprComma : values+=expr (COMMA values+=expr)+;

value : integerValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | stringValue = STRING;

if : id=IF l=BRACEL exp = expr r=BRACER THEN (expr|TEXT)* END;


