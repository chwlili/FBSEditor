grammar Flatbuffer;

schema : ( include | namespace | attribute | rootType | fileExtension | fileIdentifier | table | struct | enum | union | rpc | objectValue)* ;

include : key = 'include' val = STRING ';'? ;

namespace : key = 'namespace' (IDENT ( '.' IDENT )*) ';'? ;

attribute : key = 'attribute' STRING ';'? ;

rootType : key = 'root_type' val = IDENT ';'? ;

fileExtension : key = 'file_extension' val = STRING  ';'? ;

fileIdentifier : key = 'file_identifier' val = STRING ';'? ;

string : text = STRING;

table : attr* key = 'table' name = IDENT metaList = metadata? BRACE_L tableField* BRACE_R ;
tableField : attr* fieldName = IDENT ':' (fieldType = singleType | arrayType = listType) ('=' fieldValue = scalarValue)? metaList = metadata? (fieldArrow = '=>' fieldMap = STRING)? ';'? ;

struct : attr* key = 'struct' name = IDENT metaList = metadata? BRACE_L structField* BRACE_R ;
structField : attr* fieldName = IDENT ':' (fieldType = singleType | arrayType = listType) ('=' fieldValue = scalarValue)? metaList = metadata? (fieldArrow = '=>' fieldMap = STRING)? ';'? ;

rpc : attr* key = 'rpc_service' name = IDENT BRACE_L rpcField* BRACE_R ;
rpcField : attr* fieldName = IDENT PARENTHESES_L fieldParam = IDENT PARENTHESES_R ':' fieldReturn = IDENT metaList = metadata? ';'? ;

enum : attr* key = 'enum' name = IDENT (':' baseType = singleType)?  metaList = metadata? BRACE_L enumField* BRACE_R ;
enumField : attr* fieldName = IDENT ('=' fieldValue = INTEGER)? ','?;

union : attr* key = 'union' name = IDENT metaList = metadata? BRACE_L unionField* BRACE_R ;
unionField : attr* fieldName = IDENT ':' fieldType = singleType ','?;

metadata : PARENTHESES_L (metadataField (',' metadataField)*)? PARENTHESES_R ;
metadataField : metaName = IDENT (':' metaValue = singleValue)? ;

attr : BRACKET_L key = IDENT (PARENTHESES_L (attrField (',' attrField)*)? PARENTHESES_R)? BRACKET_R;
attrField : (attrName = IDENT '=')? attrValue = attrFieldValue;
attrFieldValue : vid = IDENT | vstr = STRING | vint = INTEGER | vfloat = FLOAT | vbool = BOOL;

singleType : 'bool' | 'byte' | 'ubyte' | 'short' | 'ushort' | 'int' | 'uint' | 'float' | 'long' | 'ulong' | 'double' | 'int8' | 'uint8' | 'int16' | 'uint16' | 'int32' | 'uint32'| 'int64' | 'uint64' | 'float32' | 'float64' | 'string' | IDENT('.'IDENT)*;
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
