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
bindMeta : BRACKET_L key='Bind' PARENTHESES_L path = string PARENTHESES_R BRACKET_R ;
indexMeta : BRACKET_L key='Index' PARENTHESES_L (fields += IDENT (',' fields += IDENT)*)? PARENTHESES_R BRACKET_R  ;
nullableMeta : BRACKET_L key='Nullable' (PARENTHESES_L val = BOOL? PARENTHESES_R)? BRACKET_R  ;
referenceMeta : BRACKET_L key='Reference' PARENTHESES_L path = string PARENTHESES_R BRACKET_R  ;

table : meta = metas key = 'table' name = IDENT metadata? BRACE_L tableField* BRACE_R ;
tableField : meta = metas fieldName = IDENT ':' fieldType = type ('=' fieldValue = scalarValue)? metadata? (fieldArrow = '=>' fieldMap = IDENT)? ';'? ;

struct : meta = metas key = 'struct' name = IDENT metadata? BRACE_L structField* BRACE_R ;
structField : meta = metas fieldName = IDENT ':' fieldType = type ('=' fieldValue = scalarValue)? metadata? ';'? ;

rpc : meta = metas key = 'rpc_service' name = IDENT BRACE_L rpcField* BRACE_R ;
rpcField : meta = metas fieldName = IDENT PARENTHESES_L fieldParam = IDENT PARENTHESES_R ':' fieldReturn = IDENT metadata? ';'? ;

enum : meta = metas key = 'enum' name = IDENT (':' type)?  metadata? BRACE_L enumField* BRACE_R ;
enumField : meta = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

union : meta = metas key = 'union' name = IDENT metadata? BRACE_L unionField* BRACE_R ;
unionField : meta = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

metadata : PARENTHESES_L (metadataField (',' metadataField)*)? PARENTHESES_R ;
metadataField : IDENT (':' singleValue)? ;

type : 'bool' | 'byte' | 'ubyte' | 'short' | 'ushort' | 'int' | 'uint' | 'float' | 'long' | 'ulong' | 'double' | 'int8' | 'uint8' | 'int16' | 'uint16' | 'int32' | 'uint32'| 'int64' | 'uint64' | 'float32' | 'float64' | 'string' | IDENT | BRACKET_L type BRACKET_R ;



objectValue : BRACE_L (objectValueField (',' objectValueField)*)? BRACE_R ;
objectValueField : IDENT ':' value ;

arrayValue : BRACKET_L (value (',' value)*)? BRACKET_R ;

value : singleValue | objectValue | arrayValue ;

singleValue : scalarValue | STRING ;

scalarValue:INTEGER | FLOAT | BOOL ;

BRACE_L : '{';
BRACE_R : '}';
BRACKET_L : '[';
BRACKET_R : ']';
PARENTHESES_L : '(';
PARENTHESES_R : ')';

INTEGER : '-'?[0-9]+ ;

FLOAT : '-'? [0-9]+ '.' [0-9]+ (('e'|'E') ('+'|'-')? [0-9]+)? ;

BOOL : ('true' | 'false') ;

STRING : '"' .*? '"' ;

IDENT : [a-zA-Z_][a-zA-Z0-9_]*;

COMMENT : (('//' ~[\r\n]* '\r'? '\n')|('/*' .*? '*/')) -> channel(HIDDEN) ;

WS : [ \t\r\n]+ -> skip ;
