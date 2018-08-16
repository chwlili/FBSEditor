parser grammar TemplateParser;
options { tokenVocab = TemplateLexer;}

document : code*;

code : var SEMICOLON? | if | switch | while | dowhile | for | foreach | expr SEMICOLON? | text | break | continue | return;

text : TEXT;
break : BREAK SEMICOLON?;
continue : CONTINUE SEMICOLON?;
return : RETURN SEMICOLON?;

var : keyword = VAR key = IDENT (SET value = expr)? ;
if : keyword = IF PARENTHESES_L condition = expr PARENTHESES_R BRACE_L code* BRACE_R;
switch : keywordA = SWITCH PARENTHESES_L condition = expr PARENTHESES_R BRACE_L (keywordB += CASE expr COLON expr BREAK? )* BRACE_R;
while : keyword = WHILE PARENTHESES_L condition = expr PARENTHESES_R BRACE_L code* BRACE_R;
dowhile : keywordA = DO BRACE_L code* BRACE_R keywordB = WHILE PARENTHESES_L condition = expr PARENTHESES_R;
for : keyword = FOR PARENTHESES_L (expr0 = var | expr1 = expr)? SEMICOLON expr2 = expr? SEMICOLON expr3=expr? PARENTHESES_R BRACE_L code* BRACE_R;
foreach : keywordA = FOREACH PARENTHESES_L code keywordB = IN code PARENTHESES_R BRACE_L code* BRACE_R;

expr : 
		 op = PARENTHESES_L r = expr PARENTHESES_R
		 //���Ժͺ���
		 | call = exprCall | prop = exprProp
		 //һԪ����
		 | op = SUB r = expr
	     //| PARENTHESES_L t = IDENT PARENTHESES_R r = expr
		 | op = INCREMENT  varName = exprProp | varName = exprProp op = INCREMENT
		 | op = DECREMENT  varName = exprProp | varName = exprProp op = DECREMENT
		 | op = LOGIC_NOT r = expr
		 | op = BIT_INVERT r = expr
		 //�˳�
		 | l = expr op = MUL r = expr
		 | l = expr op = DIV r = expr
		 | l = expr op = MOD r = expr
		 //�ӡ���
		 | l = expr op = ADD r = expr
		 | l = expr op = SUB r = expr
		 //���ơ�����
		 | l = expr op = SHIFTL r = expr
		 | l = expr op = SHIFTR r = expr
		 //С�ڡ�С�ڵ��ڡ����ڡ����ڵ���
		 | l = expr op = LESS r = expr
		 | l = expr op = LESSEQUAL r = expr
		 | l = expr op = GREATER r = expr
		 | l = expr op = GREATEREQUAL r = expr
		 //���ڡ�������
		 | l = expr op = EQUAL r = expr
		 | l = expr op = NOTEQUAL r = expr
		 //��λ��
		 | l = expr op = BIT_AND r = expr
		 //��λ���
		 | l = expr op = BIT_XOR r = expr
		 //��λ��
		 | l = expr op = BIT_OR r = expr
		 //�߼���
		 | l = expr op = LOGIC_AND r = expr
		 //�߼���
		 | l = expr op = LOGIC_OR r = expr
		 //��ֵ
		 | l = expr op = (SET|SET_DIV|SET_MUL|SET_MOD|SET_ADD|SET_SUB|SET_SHIFTL|SET_SHIFTR|SET_BIT_AND|SET_BIT_XOR|SET_BIT_OR) r = expr
		 //����ֵ
		 | v = exprValue
		 ;

exprCall : names+=IDENT (op = DOT names+=IDENT)* PARENTHESES_L (args+=expr (COMMA args+=expr)*) PARENTHESES_R;
exprProp : names+=IDENT (op = DOT names+=IDENT)*;
exprValue : integerValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | stringValue = STRING;

//exprComma : values+=expr (COMMA values+=expr)+;




