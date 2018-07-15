lexer grammar TemplateLexer;

OPEN1 : '<%' -> mode(CODE_MODE);
OPEN2 : '${' -> mode(PROP_MODE);
TEXT  : ({(InputStream.LA(1)!='<' && InputStream.LA(2)!='%') && (InputStream.LA(1)!='$' && InputStream.LA(2)!='{')}? .)+;

mode CODE_MODE;

CLOSE1 : '%>' -> mode(DEFAULT_MODE);
TEXT1  : ({InputStream.LA(1)!='%' && InputStream.LA(2)!='>'}? .)+;
WS1 : [ \t\r\n]+ -> skip ;

mode PROP_MODE;

CLOSE2 : '}' -> mode(DEFAULT_MODE);
TEXT2  : ({InputStream.LA(1)!='}'}? .)+;
WS2 : [ \t\r\n]+ -> skip ; 