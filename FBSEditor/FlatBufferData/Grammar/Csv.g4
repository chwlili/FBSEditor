grammar Csv;
csvTab : ( rows += csvRow (ROWEND rows += csvRow )* )?;
csvRow : ( cols += csvCol (COLEND cols += csvCol)* )? '\r'? '\n';//( cols += CsvCol (COLEND cols += CsvCol)* )?;
csvCol : TEXT | STRING | ;

ROWEND : '\n';
COLEND : [,;\t];
STRING : '"'( '""'| ~'"')* '"' ;
TEXT   : ~[ ] ~[,;\t\r\n"]+? ~[ ];
WS : [ ]+ -> skip;

