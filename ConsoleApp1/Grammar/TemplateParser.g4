parser grammar TemplateParser;
options { tokenVocab = TemplateLexer;}

document : (TEXT | expr | if)*;

if : id=IF l=BRACEL exp = expr r=BRACER THEN (expr|TEXT)* END;

expr : 
		 BRACEL r = expr BRACER
		 //属性和函数
		 | call = exprCall | prop = exprProp
		 //一元运算
		 | op = SUB r = expr
	     | BRACEL t = IDENT BRACER r = expr
		 | op = INCREMENT  r = expr | r = expr op = INCREMENT
		 | op = DECREMENT  r = expr | r = expr op = DECREMENT
		 | op = NOT r = expr
		 | op = INVERT r = expr
		 //乘除
		 | l = expr op = MUL r = expr
		 | l = expr op = DIV r = expr
		 | l = expr op = MOD r = expr
		 //加、减
		 | l = expr op = ADD r = expr
		 | l = expr op = SUB r = expr
		 //左移、右移
		 | l = expr op = SHIFTL r = expr
		 | l = expr op = SHIFTR r = expr
		 //小于、小于等于、大于、大于等于
		 | l = expr op = LESS r = expr
		 | l = expr op = LESSEQUAL r = expr
		 | l = expr op = GREATER r = expr
		 | l = expr op = GREATEREQUAL r = expr
		 //等于、不等于
		 | l = expr op = EQUAL r = expr
		 | l = expr op = NOTEQUAL r = expr
		 //按位与
		 | l = expr op = AND r = expr
		 //按位异或
		 | l = expr op = XOR r = expr
		 //按位或
		 | l = expr op = OR r = expr
		 //逻辑与
		 | l = expr op = AND2 r = expr
		 //逻辑或
		 | l = expr op = OR2 r = expr
		 //字面值
		 | v = exprValue
		 ;


exprCall : names+=IDENT (op = DOT names+=IDENT)* BRACEL (args+=expr (COMMA args+=expr)*) BRACER;
exprProp : names+=IDENT (op = DOT names+=IDENT)*;
exprValue : integerValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | stringValue = STRING;

//exprComma : values+=expr (COMMA values+=expr)+;




