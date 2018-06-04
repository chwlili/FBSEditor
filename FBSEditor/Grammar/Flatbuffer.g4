grammar Flatbuffer;

schema : ( include | namespace | attribute | rootType | fileExtension | fileIdentifier | table | struct | enum | union | rpc | objectValue | comment)* ;

include : key = 'include' STRING ';'? ;

namespace : key = 'namespace' (IDENT ( '.' IDENT )*) ';'? ;

attribute : key = 'attribute' STRING ';'? ;

rootType : key = 'root_type' IDENT ';'? ;

fileExtension : key = 'file_extension' STRING  ';'? ;

fileIdentifier : key = 'file_identifier' STRING ';'? ;


comment : text = COMMENT;

string : text = STRING;

metas : (bindMeta | indexMeta | nullableMeta | referenceMeta)*;
bindMeta : '[' key='Bind' '(' path = string ')' ']' comment* ;
indexMeta : '[' key='Index' '(' (fields += IDENT (',' fields += IDENT)*)? ')' ']' comment* ;
nullableMeta : '[' key='Nullable' ('(' val = BOOL? ')')? ']' comment* ;
referenceMeta : '[' key='Reference' '(' path = string ')' ']' comment* ;

table : comment*  meta = metas key = 'table' name = IDENT metadata? '{' tableField* '}' ;
tableField : comment*  meta = metas fieldName = IDENT ':' fieldType = type ('=' fieldValue = scalarValue)? metadata? ('=>' fieldMap = IDENT)? ';'? ;

struct : comment*  meta = metas key = 'struct' name = IDENT metadata? '{' structField* '}' ;
structField : comment*  meta = metas fieldName = IDENT ':' fieldType = type ('=' fieldValue = scalarValue)? metadata? ';'? ;

rpc : comment*  meta = metas key = 'rpc_service' name = IDENT '{' rpcField* '}' ;
rpcField : comment*  meta = metas fieldName = IDENT '(' fieldParam = IDENT ')' ':' fieldReturn = IDENT metadata? ';'? ;

enum : comment*  meta = metas key = 'enum' name = IDENT (':' type)?  metadata? '{' enumField* '}' ;
enumField : comment*  meta = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

union : comment*  meta = metas key = 'union' name = IDENT metadata? '{' unionField* '}' ;
unionField : comment*  meta = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

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

COMMENT : '//' ~[\r\n]* '\r'? '\n';

IDENT : [a-zA-Z_][a-zA-Z0-9_]* ;

WS : [ \t\r\n]+ -> skip ;
