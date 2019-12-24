grammar Csv;

@members
{
	public string separators = ",";
	public bool IsSeparator(string ch)
	{
		return  !string.IsNullOrEmpty(separators) && !string.IsNullOrEmpty(ch) ? separators.Contains(ch):false;
	}
	public bool IgnoreSpace = false;
}

csvTab    : rows += csvRow*;
csvRow    : (cols += csvCol)* end = csvEndCol;
csvCol	  : content = csvTxt? COLEND;
csvEndCol : content = csvTxt? ROWEND;
csvTxt    : 
			{IgnoreSpace}? WS? (txt += STRING | txt += TEXT) WS? | 
			txt += WS? (txt += STRING | txt += TEXT) txt += WS? |
			WS;

COLEND :   ',';
ROWEND : '\r'? '\n';
STRING : '"'( '""'| ~'"')* '"';
TEXT   : ~[ ,\r\n]+ (' '+ ~[ ,\r\n]+)*;
WS : [ ]+;

