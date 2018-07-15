parser grammar Template;
options { tokenVocab = TemplateLexer;}

document : (TEXT | range1 | range2)*;

range1 : OPEN1 TEXT1 CLOSE1;
range2 : OPEN2 TEXT2 CLOSE2;

//document : (code1 | code2) .*;

//code1 : .*? '<%' expr*? '%>';
//code2 : .*? '${' expr*? '}';
