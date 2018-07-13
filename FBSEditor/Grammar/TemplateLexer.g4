lexer grammar TemplateLexer;

OPEN1 : '<%' -> pushMode(CODE_MODE);
OPEN2 : '${' -> pushMode(PROP_MODE);

mode CODE_MODE;

CLOSE1 : '%>' -> popMode;
WS : [ \t\r\n]+ -> skip ;

mode PROP_MODE;

CLOSE2 : '}' -> popMode;
WS : [ \t\r\n]+ -> skip ;

