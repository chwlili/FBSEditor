grammar Json;

jsonValue : strValue = STRING | intValue = INTEGER | floatValue = FLOAT | boolValue = BOOL | nullValue = 'null' | objectValue = jsonObject | arraryValue = jsonArray;

jsonArray : '[' (arrayElement += jsonValue (',' arrayElement += jsonValue)*)? ']';

jsonObject : '{' (props += jsonProp (',' props += jsonProp)*)? '}';

jsonProp : (propName = STRING | propName = IDENT) ':' propValue = jsonValue;

INTEGER : '-'?[0-9]+ ;

FLOAT : '-'? [0-9]+ '.' [0-9]+ (('e'|'E') ('+'|'-')? [0-9]+)? ;

BOOL : ('true' | 'false') ;

STRING : '"' .*? '"' ;

IDENT : [a-zA-Z_][a-zA-Z0-9_]*;

WS : [ \t\r\n]+ -> skip ;
