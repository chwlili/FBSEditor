parser grammar TemplateParser;
options { tokenVocab = TemplateLexer;}

document : (TEXT | expr | if)*;

if : id=IF l=BRACEL exp = expr r=BRACER THEN (expr|TEXT)* END;

expr : 
		 BRACEL r = expr BRACER
		 //���Ժͺ���
		 | call = exprCall | prop = exprProp
		 //һԪ����
		 | op = SUB r = expr
	     | BRACEL t = IDENT BRACER r = expr
		 | op = INCREMENT  r = expr | r = expr op = INCREMENT
		 | op = DECREMENT  r = expr | r = expr op = DECREMENT
		 | op = NOT r = expr
		 | op = INVERT r = expr
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
		 | l = expr op = AND r = expr
		 //��λ���
		 | l = expr op = XOR r = expr
		 //��λ��
		 | l = expr op = OR r = expr
		 //�߼���
		 | l = expr op = AND2 r = expr
		 //�߼���
		 | l = expr op = OR2 r = expr
		 //����ֵ
		 | v = exprValue
		 ;


exprCall : names+=IDENT (op = DOT names+=IDENT)* BRACEL (args+=expr (COMMA args+=expr)*) BRACER;
exprProp : names+=IDENT (op = DOT names+=IDENT)*;
exprValue : integerValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | stringValue = STRING;

//exprComma : values+=expr (COMMA values+=expr)+;




