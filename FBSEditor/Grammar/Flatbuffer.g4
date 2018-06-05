grammar Flatbuffer;

schema : ( include | namespace | attribute | rootType | fileExtension | fileIdentifier | table | struct | enum | union | rpc | objectValue)* ;

include : key = 'include' STRING ';'? ;

namespace : key = 'namespace' (IDENT ( '.' IDENT )*) ';'? ;

attribute : key = 'attribute' STRING ';'? ;

rootType : key = 'root_type' IDENT ';'? ;

fileExtension : key = 'file_extension' STRING  ';'? ;

fileIdentifier : key = 'file_identifier' STRING ';'? ;


string : text = STRING;

metas : (bindMeta | indexMeta | nullableMeta | referenceMeta)*;
bindMeta : '[' key='Bind' '(' path = string ')' ']' ;
indexMeta : '[' key='Index' '(' (fields += IDENT (',' fields += IDENT)*)? ')' ']'  ;
nullableMeta : '[' key='Nullable' ('(' val = BOOL? ')')? ']'  ;
referenceMeta : '[' key='Reference' '(' path = string ')' ']'  ;

table : meta = metas key = 'table' name = IDENT metadata? '{' tableField* '}' ;
tableField : meta = metas fieldName = IDENT ':' fieldType = type ('=' fieldValue = scalarValue)? metadata? (fieldArrow = '=>' fieldMap = IDENT)? ';'? ;

struct : meta = metas key = 'struct' name = IDENT metadata? '{' structField* '}' ;
structField : meta = metas fieldName = IDENT ':' fieldType = type ('=' fieldValue = scalarValue)? metadata? ';'? ;

rpc : meta = metas key = 'rpc_service' name = IDENT '{' rpcField* '}' ;
rpcField : meta = metas fieldName = IDENT '(' fieldParam = IDENT ')' ':' fieldReturn = IDENT metadata? ';'? ;

enum : meta = metas key = 'enum' name = IDENT (':' type)?  metadata? '{' enumField* '}' ;
enumField : meta = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

union : meta = metas key = 'union' name = IDENT metadata? '{' unionField* '}' ;
unionField : meta = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

metadata : '(' (metadataField (',' metadataField)*)? ')' ;
metadataField : IDENT (':' singleValue)? ;

type : 'bool' | 'byte' | 'ubyte' | 'short' | 'ushort' | 'int' | 'uint' | 'float' | 'long' | 'ulong' | 'double' | 'int8' | 'uint8' | 'int16' | 'uint16' | 'int32' | 'uint32'| 'int64' | 'uint64' | 'float32' | 'float64' | 'string' | IDENT | '[' type ']' ;



objectValue : '{' (objectValueField (',' objectValueField)*)? '}' ;
objectValueField : IDENT ':' value ;

arrayValue : '[' (value (',' value)*)? ']' ;

value : singleValue | objectValue | arrayValue ;

singleValue : scalarValue | STRING ;

scalarValue:INTEGER | FLOAT | BOOL ;



INTEGER : '-'?[0-9]+ ;

FLOAT : '-'? [0-9]+ '.' [0-9]+ (('e'|'E') ('+'|'-')? [0-9]+)? ;

BOOL : ('true' | 'false') ;

STRING : '"' .*? '"' ;

IDENT : [a-zA-Z_][a-zA-Z0-9_]*;

COMMENT : '//' ~[\r\n]* '\r'? '\n' -> channel(HIDDEN) ;
COMMENT1 : '/*' .*? '*/' -> channel(HIDDEN) ;
//COMMENT2 : '//' .*? EOF -> skip;

WS : [ \t\r\n]+ -> skip ;
