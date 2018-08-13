lexer grammar TemplateLexer;

@members
{
protected bool IsBeginTag()
{
	return (InputStream.LA(1)=='<' && InputStream.LA(2)=='%');
}
}


OPEN : '<%' -> mode(CODE_MODE);//,skip;

TEXT  : ({!IsBeginTag()}? .)+;

mode CODE_MODE;

CLOSE : '%>' -> mode(DEFAULT_MODE);//,skip;

INTEGER : [0-9]+ ;

FLOAT : [0-9]+ '.' [0-9]+ (('e'|'E') ('+'|'-')? [0-9]+)? ;

BOOL : ('true' | 'false') ;

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
IF:'if';
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


IDENT : [a-zA-Z_][a-zA-Z0-9_]*;

COMMENT : (('//' ~[\r\n]* '\r'? '\n')|('/*' .*? '*/')) -> channel(HIDDEN) ;


WS : [ \t\r\n]+ -> skip ;