parser grammar Template;
options { tokenVocab = TemplateLexer;}

document : OPEN1 | CLOSE1;

//document : (code1 | code2) .*;

//code1 : .*? '<%' expr*? '%>';
//code2 : .*? '${' expr*? '}';
