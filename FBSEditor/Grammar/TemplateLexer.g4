lexer grammar TemplateLexer;

@members
{
protected bool IsBeginTag()
{
	return (InputStream.LA(1)=='<' && InputStream.LA(2)=='%');
}
}


OPEN : '<%' -> mode(CODE_MODE),channel(HIDDEN);

TEXT  : ({!IsBeginTag()}? .)+;

mode CODE_MODE;

CLOSE : '%>' -> mode(DEFAULT_MODE),channel(HIDDEN);

INTEGER : [0-9]+ ;

FLOAT : [0-9]+ '.' [0-9]+ (('e'|'E') ('+'|'-')? [0-9]+)? ;

BOOL : ('true' | 'false') ;

SET_DIV:'/=';
SET_MUL:'*=';
SET_MOD:'%=';
SET_ADD:'+=';
SET_SUB:'-=';
SET_SHIFTL:'<<=';
SET_SHIFTR:'>>=';
SET_BIT_AND:'&=';
SET_BIT_XOR:'^=';
SET_BIT_OR:'|=';
STRING : '"' .*? '"' ;
BRACE_L : '{';
BRACE_R : '}';
INCREMENT:'++';
DECREMENT:'--';
MUL:'*';
DIV:'/';
MOD:'%';
ADD:'+';
SUB:'-';
PARENTHESES_L:'(';
PARENTHESES_R:')';
DOT:'.';
COMMA:',';
NOTEQUAL : '!=';
LOGIC_NOT:'!';
BIT_INVERT:'~';
SHIFTL:'<<';
SHIFTR:'>>';
LESS : '<';
LESSEQUAL : '<=';
GREATER : '>';
GREATEREQUAL : '>=';
EQUAL : '==';
LOGIC_AND : '&&';
LOGIC_OR : '||';
BIT_AND : '&';
BIT_OR : '|';
BIT_XOR : '^';
SET:'=';

COLON : ':';
SEMICOLON : ';';

VAR : 'var';
IF:'if';
SWITCH:'switch';
CASE:'case';
WHILE:'while';
DO:'do';
FOR:'for';
FOREACH:'foreach';
IN:'in';
BREAK:'break';
CONTINUE:'continue';
RETURN:'return';
IDENT : [a-zA-Z_][a-zA-Z0-9_]*;

COMMENT : (('//' ~[\r\n]* '\r'? '\n')|('/*' .*? '*/')) -> channel(HIDDEN) ;


WS : [ \t\r\n]+ -> skip ;