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

table : attributes = metas key = 'table' name = IDENT metaList = metadata? BRACE_L tableField* BRACE_R ;
tableField : attributes = metas fieldName = IDENT ':' (fieldType = singleType | arrayType = listType) ('=' fieldValue = scalarValue)? metaList = metadata? (fieldArrow = '=>' fieldMap = IDENT)? ';'? ;

struct : attributes = metas key = 'struct' name = IDENT metaList = metadata? BRACE_L structField* BRACE_R ;
structField : attributes = metas fieldName = IDENT ':' (fieldType = singleType | arrayType = listType) ('=' fieldValue = scalarValue)? metaList = metadata? (fieldArrow = '=>' fieldMap = IDENT)? ';'? ;

rpc : attributes = metas key = 'rpc_service' name = IDENT BRACE_L rpcField* BRACE_R ;
rpcField : attributes = metas fieldName = IDENT PARENTHESES_L fieldParam = IDENT PARENTHESES_R ':' fieldReturn = IDENT metaList = metadata? ';'? ;

enum : attributes = metas key = 'enum' name = IDENT (':' (singleType|listType))?  metaList = metadata? BRACE_L enumField* BRACE_R ;
enumField : attributes = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

union : attributes = metas key = 'union' name = IDENT metaList = metadata? BRACE_L unionField* BRACE_R ;
unionField : attributes = metas fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

metadata : PARENTHESES_L (metadataField (',' metadataField)*)? PARENTHESES_R ;
metadataField : metaName = IDENT (':' metaValue = singleValue)? ;

singleType : 'bool' | 'byte' | 'ubyte' | 'short' | 'ushort' | 'int' | 'uint' | 'float' | 'long' | 'ulong' | 'double' | 'int8' | 'uint8' | 'int16' | 'uint16' | 'int32' | 'uint32'| 'int64' | 'uint64' | 'float32' | 'float64' | 'string' | IDENT ;
listType : BRACKET_L type = singleType BRACKET_R;


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
