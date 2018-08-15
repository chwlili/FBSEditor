parser grammar TemplateParser;
options { tokenVocab = TemplateLexer;}

document : code*;

code : var | if | switch | while | dowhile | for | foreach | expr | text | BREAK | CONTINUE | RETURN;
text : TEXT;

var : keyword = VAR key = IDENT (EQUAL expr)? ;
if : keyword = IF PARENTHESES_L condition = expr PARENTHESES_R BRACE_L code* BRACE_R;
switch : keywordA = SWITCH PARENTHESES_L condition = expr PARENTHESES_R BRACE_L (keywordB += CASE expr COLON expr BREAK? )* BRACE_R;
while : keyword = WHILE PARENTHESES_L condition = expr PARENTHESES_R BRACE_L code* BRACE_R;
dowhile : keywordA = DO BRACE_L code* BRACE_R keywordB = WHILE PARENTHESES_L condition = expr PARENTHESES_R;
for : keyword = FOR PARENTHESES_L expr1 = expr? SEMICOLON expr2 = expr? SEMICOLON expr3=expr? PARENTHESES_R BRACE_L code* BRACE_R;
foreach : keywordA = FOREACH PARENTHESES_L code keywordB = IN code PARENTHESES_R BRACE_L code* BRACE_R;

expr : 
		 PARENTHESES_L r = expr PARENTHESES_R
		 //属性和函数
		 | call = exprCall | prop = exprProp
		 //一元运算
		 | op = SUB r = expr
	     | PARENTHESES_L t = IDENT PARENTHESES_R r = expr
		 | op = INCREMENT  r = expr | r = expr op = INCREMENT
		 | op = DECREMENT  r = expr | r = expr op = DECREMENT
		 | op = LOGIC_NOT r = expr
		 | op = BIT_INVERT r = expr
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
		 | l = expr op = BIT_AND r = expr
		 //按位异或
		 | l = expr op = BIT_XOR r = expr
		 //按位或
		 | l = expr op = BIT_OR r = expr
		 //逻辑与
		 | l = expr op = LOGIC_AND r = expr
		 //逻辑或
		 | l = expr op = LOGIC_OR r = expr
		 //字面值
		 | v = exprValue
		 ;


exprCall : names+=IDENT (op = DOT names+=IDENT)* PARENTHESES_L (args+=expr (COMMA args+=expr)*) PARENTHESES_R;
exprProp : names+=IDENT (op = DOT names+=IDENT)*;
exprValue : integerValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | stringValue = STRING;

//exprComma : values+=expr (COMMA values+=expr)+;




