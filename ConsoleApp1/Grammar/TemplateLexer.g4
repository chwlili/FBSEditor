lexer grammar TemplateLexer;

@members
{
protected bool IsBeginTag()
{
	return (InputStream.LA(1)=='<' && InputStream.LA(2)=='%');
}
}


OPEN : '<%' -> mode(CODE_MODE),skip;

TEXT  : ({!IsBeginTag()}? .)+;

mode CODE_MODE;

CLOSE : '%>' -> mode(DEFAULT_MODE),skip;

INTEGER : [0-9]+ ;

FLOAT : [0-9]+ '.' [0-9]+ (('e'|'E') ('+'|'-')? [0-9]+)? ;

BOOL : ('true' | 'false') ;

STRING : '"' .*? '"' ;

INCREMENT:'++';
DECREMENT:'--';
MUL:'*';
DIV:'/';
MOD:'%';
ADD:'+';
SUB:'-';
BRACEL:'(';
BRACER:')';
IF:'if';
THEN:'then';
END:'end';
DOT:'.';
COMMA:',';
NOT:'!';
INVERT:'~';
SHIFTL:'<<';
SHIFTR:'>>';

IDENT : [a-zA-Z_][a-zA-Z0-9_]*;

COMMENT : (('//' ~[\r\n]* '\r'? '\n')|('/*' .*? '*/')) -> channel(HIDDEN) ;


WS : [ \t\r\n]+ -> skip ;