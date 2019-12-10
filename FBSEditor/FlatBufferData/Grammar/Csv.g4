grammar Csv;

@members
{
	public string separators = ",";
	public bool IsSeparator(string ch)
	{
		return  !string.IsNullOrEmpty(separators) && !string.IsNullOrEmpty(ch) ? separators.Contains(ch):false;
	}
}

csvTab : (rows += csvRow (ROWEND|EOF))*;

csvRow : cols += csvCol (COLEND cols += csvCol)*;
csvCol : txt = txtField | str = strField | WS+;
txtField : WS? txt+=TEXT (txt+=TEXT|txt+=WS)*?  WS?;
strField : WS? txt = STRING WS?;

ROWEND : '\r'? '\n';

COLEND :   ','{IsSeparator(",")}?
		 | ';'{IsSeparator(";")}?
		 | '\t' {IsSeparator("\t")}?;

STRING : '"'( '""'| ~'"')* '"' ;

TEXT   :   ~[ ,\r\n]+ {IsSeparator(",")}?
		 | ~[ ;\r\n]+ {IsSeparator(";")}?
		 | ~[ \t\r\n]+ {IsSeparator("\t")}?
		 ;

WS : [ ]+;

