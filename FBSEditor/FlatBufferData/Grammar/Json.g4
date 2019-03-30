grammar Json;

root : array | struct;

array : '[' (value (',' value)*)? ']';

struct : '{' (props += prop (',' props += prop)*)? '}';

prop : (propName = STRING | propName = IDENT) ':' value;

value : strValue = STRING | intValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | nullValue = 'null' | structValue = struct | arraryValue = array;

INTEGER : '-'?[0-9]+ ;

FLOAT : '-'? [0-9]+ '.' [0-9]+ (('e'|'E') ('+'|'-')? [0-9]+)? ;

BOOL : ('true' | 'false') ;

STRING : '"' .*? '"' ;

IDENT : [a-zA-Z_][a-zA-Z0-9_]*;

WS : [ \t\r\n]+ -> skip ;
