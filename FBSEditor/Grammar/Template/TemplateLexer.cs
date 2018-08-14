//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from TemplateLexer.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class TemplateLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		OPEN=1, TEXT=2, CLOSE=3, INTEGER=4, FLOAT=5, BOOL=6, STRING=7, BRACE_L=8, 
		BRACE_R=9, INCREMENT=10, DECREMENT=11, MUL=12, DIV=13, MOD=14, ADD=15, 
		SUB=16, PARENTHESES_L=17, PARENTHESES_R=18, DOT=19, COMMA=20, NOTEQUAL=21, 
		LOGIC_NOT=22, BIT_INVERT=23, SHIFTL=24, SHIFTR=25, LESS=26, LESSEQUAL=27, 
		GREATER=28, GREATEREQUAL=29, EQUAL=30, LOGIC_AND=31, LOGIC_OR=32, BIT_AND=33, 
		BIT_OR=34, BIT_XOR=35, COLON=36, SEMICOLON=37, VAR=38, IF=39, SWITCH=40, 
		CASE=41, WHILE=42, DO=43, FOR=44, FOREACH=45, IN=46, BREACK=47, CONTINUE=48, 
		IDENT=49, COMMENT=50, WS=51;
	public const int
		CODE_MODE=1;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE", "CODE_MODE"
	};

	public static readonly string[] ruleNames = {
		"OPEN", "TEXT", "CLOSE", "INTEGER", "FLOAT", "BOOL", "STRING", "BRACE_L", 
		"BRACE_R", "INCREMENT", "DECREMENT", "MUL", "DIV", "MOD", "ADD", "SUB", 
		"PARENTHESES_L", "PARENTHESES_R", "DOT", "COMMA", "NOTEQUAL", "LOGIC_NOT", 
		"BIT_INVERT", "SHIFTL", "SHIFTR", "LESS", "LESSEQUAL", "GREATER", "GREATEREQUAL", 
		"EQUAL", "LOGIC_AND", "LOGIC_OR", "BIT_AND", "BIT_OR", "BIT_XOR", "COLON", 
		"SEMICOLON", "VAR", "IF", "SWITCH", "CASE", "WHILE", "DO", "FOR", "FOREACH", 
		"IN", "BREACK", "CONTINUE", "IDENT", "COMMENT", "WS"
	};


	protected bool IsBeginTag()
	{
		return (InputStream.LA(1)=='<' && InputStream.LA(2)=='%');
	}


	public TemplateLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public TemplateLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, null, null, null, null, null, null, null, "'{'", "'}'", "'++'", 
		"'--'", "'*'", "'/'", "'%'", "'+'", "'-'", "'('", "')'", "'.'", "','", 
		"'!='", "'!'", "'~'", "'<<'", "'>>'", "'<'", "'<='", "'>'", "'>='", "'=='", 
		"'&&'", "'||'", "'&'", "'|'", "'^'", "':'", "';'", "'var'", "'if'", "'switch'", 
		"'case'", "'while'", "'do'", "'for'", "'foreach'", "'in'", "'break'", 
		"'continue'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "OPEN", "TEXT", "CLOSE", "INTEGER", "FLOAT", "BOOL", "STRING", "BRACE_L", 
		"BRACE_R", "INCREMENT", "DECREMENT", "MUL", "DIV", "MOD", "ADD", "SUB", 
		"PARENTHESES_L", "PARENTHESES_R", "DOT", "COMMA", "NOTEQUAL", "LOGIC_NOT", 
		"BIT_INVERT", "SHIFTL", "SHIFTR", "LESS", "LESSEQUAL", "GREATER", "GREATEREQUAL", 
		"EQUAL", "LOGIC_AND", "LOGIC_OR", "BIT_AND", "BIT_OR", "BIT_XOR", "COLON", 
		"SEMICOLON", "VAR", "IF", "SWITCH", "CASE", "WHILE", "DO", "FOR", "FOREACH", 
		"IN", "BREACK", "CONTINUE", "IDENT", "COMMENT", "WS"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "TemplateLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static TemplateLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 1 : return TEXT_sempred(_localctx, predIndex);
		}
		return true;
	}
	private bool TEXT_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return !IsBeginTag();
		}
		return true;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x35', '\x155', '\b', '\x1', '\b', '\x1', '\x4', '\x2', 
		'\t', '\x2', '\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', 
		'\x5', '\t', '\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', 
		'\x4', '\b', '\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', 
		'\x4', '\v', '\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', 
		'\x4', '\xE', '\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', 
		'\x10', '\x4', '\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', 
		'\x13', '\t', '\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', 
		'\x15', '\x4', '\x16', '\t', '\x16', '\x4', '\x17', '\t', '\x17', '\x4', 
		'\x18', '\t', '\x18', '\x4', '\x19', '\t', '\x19', '\x4', '\x1A', '\t', 
		'\x1A', '\x4', '\x1B', '\t', '\x1B', '\x4', '\x1C', '\t', '\x1C', '\x4', 
		'\x1D', '\t', '\x1D', '\x4', '\x1E', '\t', '\x1E', '\x4', '\x1F', '\t', 
		'\x1F', '\x4', ' ', '\t', ' ', '\x4', '!', '\t', '!', '\x4', '\"', '\t', 
		'\"', '\x4', '#', '\t', '#', '\x4', '$', '\t', '$', '\x4', '%', '\t', 
		'%', '\x4', '&', '\t', '&', '\x4', '\'', '\t', '\'', '\x4', '(', '\t', 
		'(', '\x4', ')', '\t', ')', '\x4', '*', '\t', '*', '\x4', '+', '\t', '+', 
		'\x4', ',', '\t', ',', '\x4', '-', '\t', '-', '\x4', '.', '\t', '.', '\x4', 
		'/', '\t', '/', '\x4', '\x30', '\t', '\x30', '\x4', '\x31', '\t', '\x31', 
		'\x4', '\x32', '\t', '\x32', '\x4', '\x33', '\t', '\x33', '\x4', '\x34', 
		'\t', '\x34', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x6', '\x3', 
		's', '\n', '\x3', '\r', '\x3', '\xE', '\x3', 't', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x5', '\x6', '\x5', '~', '\n', '\x5', '\r', '\x5', '\xE', '\x5', '\x7F', 
		'\x3', '\x6', '\x6', '\x6', '\x83', '\n', '\x6', '\r', '\x6', '\xE', '\x6', 
		'\x84', '\x3', '\x6', '\x3', '\x6', '\x6', '\x6', '\x89', '\n', '\x6', 
		'\r', '\x6', '\xE', '\x6', '\x8A', '\x3', '\x6', '\x3', '\x6', '\x5', 
		'\x6', '\x8F', '\n', '\x6', '\x3', '\x6', '\x6', '\x6', '\x92', '\n', 
		'\x6', '\r', '\x6', '\xE', '\x6', '\x93', '\x5', '\x6', '\x96', '\n', 
		'\x6', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', 
		'\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x5', '\a', '\xA1', 
		'\n', '\a', '\x3', '\b', '\x3', '\b', '\a', '\b', '\xA5', '\n', '\b', 
		'\f', '\b', '\xE', '\b', '\xA8', '\v', '\b', '\x3', '\b', '\x3', '\b', 
		'\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', '\x3', '\v', '\x3', 
		'\v', '\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', '\r', 
		'\x3', '\r', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', '\x3', '\xF', '\x3', 
		'\x10', '\x3', '\x10', '\x3', '\x11', '\x3', '\x11', '\x3', '\x12', '\x3', 
		'\x12', '\x3', '\x13', '\x3', '\x13', '\x3', '\x14', '\x3', '\x14', '\x3', 
		'\x15', '\x3', '\x15', '\x3', '\x16', '\x3', '\x16', '\x3', '\x16', '\x3', 
		'\x17', '\x3', '\x17', '\x3', '\x18', '\x3', '\x18', '\x3', '\x19', '\x3', 
		'\x19', '\x3', '\x19', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1A', '\x3', 
		'\x1B', '\x3', '\x1B', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1C', '\x3', 
		'\x1D', '\x3', '\x1D', '\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1E', '\x3', 
		'\x1F', '\x3', '\x1F', '\x3', '\x1F', '\x3', ' ', '\x3', ' ', '\x3', ' ', 
		'\x3', '!', '\x3', '!', '\x3', '!', '\x3', '\"', '\x3', '\"', '\x3', '#', 
		'\x3', '#', '\x3', '$', '\x3', '$', '\x3', '%', '\x3', '%', '\x3', '&', 
		'\x3', '&', '\x3', '\'', '\x3', '\'', '\x3', '\'', '\x3', '\'', '\x3', 
		'(', '\x3', '(', '\x3', '(', '\x3', ')', '\x3', ')', '\x3', ')', '\x3', 
		')', '\x3', ')', '\x3', ')', '\x3', ')', '\x3', '*', '\x3', '*', '\x3', 
		'*', '\x3', '*', '\x3', '*', '\x3', '+', '\x3', '+', '\x3', '+', '\x3', 
		'+', '\x3', '+', '\x3', '+', '\x3', ',', '\x3', ',', '\x3', ',', '\x3', 
		'-', '\x3', '-', '\x3', '-', '\x3', '-', '\x3', '.', '\x3', '.', '\x3', 
		'.', '\x3', '.', '\x3', '.', '\x3', '.', '\x3', '.', '\x3', '.', '\x3', 
		'/', '\x3', '/', '\x3', '/', '\x3', '\x30', '\x3', '\x30', '\x3', '\x30', 
		'\x3', '\x30', '\x3', '\x30', '\x3', '\x30', '\x3', '\x31', '\x3', '\x31', 
		'\x3', '\x31', '\x3', '\x31', '\x3', '\x31', '\x3', '\x31', '\x3', '\x31', 
		'\x3', '\x31', '\x3', '\x31', '\x3', '\x32', '\x3', '\x32', '\a', '\x32', 
		'\x12E', '\n', '\x32', '\f', '\x32', '\xE', '\x32', '\x131', '\v', '\x32', 
		'\x3', '\x33', '\x3', '\x33', '\x3', '\x33', '\x3', '\x33', '\a', '\x33', 
		'\x137', '\n', '\x33', '\f', '\x33', '\xE', '\x33', '\x13A', '\v', '\x33', 
		'\x3', '\x33', '\x5', '\x33', '\x13D', '\n', '\x33', '\x3', '\x33', '\x3', 
		'\x33', '\x3', '\x33', '\x3', '\x33', '\x3', '\x33', '\a', '\x33', '\x144', 
		'\n', '\x33', '\f', '\x33', '\xE', '\x33', '\x147', '\v', '\x33', '\x3', 
		'\x33', '\x3', '\x33', '\x5', '\x33', '\x14B', '\n', '\x33', '\x3', '\x33', 
		'\x3', '\x33', '\x3', '\x34', '\x6', '\x34', '\x150', '\n', '\x34', '\r', 
		'\x34', '\xE', '\x34', '\x151', '\x3', '\x34', '\x3', '\x34', '\x4', '\xA6', 
		'\x145', '\x2', '\x35', '\x4', '\x3', '\x6', '\x4', '\b', '\x5', '\n', 
		'\x6', '\f', '\a', '\xE', '\b', '\x10', '\t', '\x12', '\n', '\x14', '\v', 
		'\x16', '\f', '\x18', '\r', '\x1A', '\xE', '\x1C', '\xF', '\x1E', '\x10', 
		' ', '\x11', '\"', '\x12', '$', '\x13', '&', '\x14', '(', '\x15', '*', 
		'\x16', ',', '\x17', '.', '\x18', '\x30', '\x19', '\x32', '\x1A', '\x34', 
		'\x1B', '\x36', '\x1C', '\x38', '\x1D', ':', '\x1E', '<', '\x1F', '>', 
		' ', '@', '!', '\x42', '\"', '\x44', '#', '\x46', '$', 'H', '%', 'J', 
		'&', 'L', '\'', 'N', '(', 'P', ')', 'R', '*', 'T', '+', 'V', ',', 'X', 
		'-', 'Z', '.', '\\', '/', '^', '\x30', '`', '\x31', '\x62', '\x32', '\x64', 
		'\x33', '\x66', '\x34', 'h', '\x35', '\x4', '\x2', '\x3', '\t', '\x3', 
		'\x2', '\x32', ';', '\x4', '\x2', 'G', 'G', 'g', 'g', '\x4', '\x2', '-', 
		'-', '/', '/', '\x5', '\x2', '\x43', '\\', '\x61', '\x61', '\x63', '|', 
		'\x6', '\x2', '\x32', ';', '\x43', '\\', '\x61', '\x61', '\x63', '|', 
		'\x4', '\x2', '\f', '\f', '\xF', '\xF', '\x5', '\x2', '\v', '\f', '\xF', 
		'\xF', '\"', '\"', '\x2', '\x162', '\x2', '\x4', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x6', '\x3', '\x2', '\x2', '\x2', '\x3', '\b', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\n', '\x3', '\x2', '\x2', '\x2', '\x3', '\f', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\xE', '\x3', '\x2', '\x2', '\x2', '\x3', 
		'\x10', '\x3', '\x2', '\x2', '\x2', '\x3', '\x12', '\x3', '\x2', '\x2', 
		'\x2', '\x3', '\x14', '\x3', '\x2', '\x2', '\x2', '\x3', '\x16', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\x18', '\x3', '\x2', '\x2', '\x2', '\x3', 
		'\x1A', '\x3', '\x2', '\x2', '\x2', '\x3', '\x1C', '\x3', '\x2', '\x2', 
		'\x2', '\x3', '\x1E', '\x3', '\x2', '\x2', '\x2', '\x3', ' ', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\"', '\x3', '\x2', '\x2', '\x2', '\x3', '$', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '&', '\x3', '\x2', '\x2', '\x2', '\x3', '(', 
		'\x3', '\x2', '\x2', '\x2', '\x3', '*', '\x3', '\x2', '\x2', '\x2', '\x3', 
		',', '\x3', '\x2', '\x2', '\x2', '\x3', '.', '\x3', '\x2', '\x2', '\x2', 
		'\x3', '\x30', '\x3', '\x2', '\x2', '\x2', '\x3', '\x32', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\x34', '\x3', '\x2', '\x2', '\x2', '\x3', '\x36', 
		'\x3', '\x2', '\x2', '\x2', '\x3', '\x38', '\x3', '\x2', '\x2', '\x2', 
		'\x3', ':', '\x3', '\x2', '\x2', '\x2', '\x3', '<', '\x3', '\x2', '\x2', 
		'\x2', '\x3', '>', '\x3', '\x2', '\x2', '\x2', '\x3', '@', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\x42', '\x3', '\x2', '\x2', '\x2', '\x3', '\x44', 
		'\x3', '\x2', '\x2', '\x2', '\x3', '\x46', '\x3', '\x2', '\x2', '\x2', 
		'\x3', 'H', '\x3', '\x2', '\x2', '\x2', '\x3', 'J', '\x3', '\x2', '\x2', 
		'\x2', '\x3', 'L', '\x3', '\x2', '\x2', '\x2', '\x3', 'N', '\x3', '\x2', 
		'\x2', '\x2', '\x3', 'P', '\x3', '\x2', '\x2', '\x2', '\x3', 'R', '\x3', 
		'\x2', '\x2', '\x2', '\x3', 'T', '\x3', '\x2', '\x2', '\x2', '\x3', 'V', 
		'\x3', '\x2', '\x2', '\x2', '\x3', 'X', '\x3', '\x2', '\x2', '\x2', '\x3', 
		'Z', '\x3', '\x2', '\x2', '\x2', '\x3', '\\', '\x3', '\x2', '\x2', '\x2', 
		'\x3', '^', '\x3', '\x2', '\x2', '\x2', '\x3', '`', '\x3', '\x2', '\x2', 
		'\x2', '\x3', '\x62', '\x3', '\x2', '\x2', '\x2', '\x3', '\x64', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\x66', '\x3', '\x2', '\x2', '\x2', '\x3', 
		'h', '\x3', '\x2', '\x2', '\x2', '\x4', 'j', '\x3', '\x2', '\x2', '\x2', 
		'\x6', 'r', '\x3', '\x2', '\x2', '\x2', '\b', 'v', '\x3', '\x2', '\x2', 
		'\x2', '\n', '}', '\x3', '\x2', '\x2', '\x2', '\f', '\x82', '\x3', '\x2', 
		'\x2', '\x2', '\xE', '\xA0', '\x3', '\x2', '\x2', '\x2', '\x10', '\xA2', 
		'\x3', '\x2', '\x2', '\x2', '\x12', '\xAB', '\x3', '\x2', '\x2', '\x2', 
		'\x14', '\xAD', '\x3', '\x2', '\x2', '\x2', '\x16', '\xAF', '\x3', '\x2', 
		'\x2', '\x2', '\x18', '\xB2', '\x3', '\x2', '\x2', '\x2', '\x1A', '\xB5', 
		'\x3', '\x2', '\x2', '\x2', '\x1C', '\xB7', '\x3', '\x2', '\x2', '\x2', 
		'\x1E', '\xB9', '\x3', '\x2', '\x2', '\x2', ' ', '\xBB', '\x3', '\x2', 
		'\x2', '\x2', '\"', '\xBD', '\x3', '\x2', '\x2', '\x2', '$', '\xBF', '\x3', 
		'\x2', '\x2', '\x2', '&', '\xC1', '\x3', '\x2', '\x2', '\x2', '(', '\xC3', 
		'\x3', '\x2', '\x2', '\x2', '*', '\xC5', '\x3', '\x2', '\x2', '\x2', ',', 
		'\xC7', '\x3', '\x2', '\x2', '\x2', '.', '\xCA', '\x3', '\x2', '\x2', 
		'\x2', '\x30', '\xCC', '\x3', '\x2', '\x2', '\x2', '\x32', '\xCE', '\x3', 
		'\x2', '\x2', '\x2', '\x34', '\xD1', '\x3', '\x2', '\x2', '\x2', '\x36', 
		'\xD4', '\x3', '\x2', '\x2', '\x2', '\x38', '\xD6', '\x3', '\x2', '\x2', 
		'\x2', ':', '\xD9', '\x3', '\x2', '\x2', '\x2', '<', '\xDB', '\x3', '\x2', 
		'\x2', '\x2', '>', '\xDE', '\x3', '\x2', '\x2', '\x2', '@', '\xE1', '\x3', 
		'\x2', '\x2', '\x2', '\x42', '\xE4', '\x3', '\x2', '\x2', '\x2', '\x44', 
		'\xE7', '\x3', '\x2', '\x2', '\x2', '\x46', '\xE9', '\x3', '\x2', '\x2', 
		'\x2', 'H', '\xEB', '\x3', '\x2', '\x2', '\x2', 'J', '\xED', '\x3', '\x2', 
		'\x2', '\x2', 'L', '\xEF', '\x3', '\x2', '\x2', '\x2', 'N', '\xF1', '\x3', 
		'\x2', '\x2', '\x2', 'P', '\xF5', '\x3', '\x2', '\x2', '\x2', 'R', '\xF8', 
		'\x3', '\x2', '\x2', '\x2', 'T', '\xFF', '\x3', '\x2', '\x2', '\x2', 'V', 
		'\x104', '\x3', '\x2', '\x2', '\x2', 'X', '\x10A', '\x3', '\x2', '\x2', 
		'\x2', 'Z', '\x10D', '\x3', '\x2', '\x2', '\x2', '\\', '\x111', '\x3', 
		'\x2', '\x2', '\x2', '^', '\x119', '\x3', '\x2', '\x2', '\x2', '`', '\x11C', 
		'\x3', '\x2', '\x2', '\x2', '\x62', '\x122', '\x3', '\x2', '\x2', '\x2', 
		'\x64', '\x12B', '\x3', '\x2', '\x2', '\x2', '\x66', '\x14A', '\x3', '\x2', 
		'\x2', '\x2', 'h', '\x14F', '\x3', '\x2', '\x2', '\x2', 'j', 'k', '\a', 
		'>', '\x2', '\x2', 'k', 'l', '\a', '\'', '\x2', '\x2', 'l', 'm', '\x3', 
		'\x2', '\x2', '\x2', 'm', 'n', '\b', '\x2', '\x2', '\x2', 'n', 'o', '\b', 
		'\x2', '\x3', '\x2', 'o', '\x5', '\x3', '\x2', '\x2', '\x2', 'p', 'q', 
		'\x6', '\x3', '\x2', '\x2', 'q', 's', '\v', '\x2', '\x2', '\x2', 'r', 
		'p', '\x3', '\x2', '\x2', '\x2', 's', 't', '\x3', '\x2', '\x2', '\x2', 
		't', 'r', '\x3', '\x2', '\x2', '\x2', 't', 'u', '\x3', '\x2', '\x2', '\x2', 
		'u', '\a', '\x3', '\x2', '\x2', '\x2', 'v', 'w', '\a', '\'', '\x2', '\x2', 
		'w', 'x', '\a', '@', '\x2', '\x2', 'x', 'y', '\x3', '\x2', '\x2', '\x2', 
		'y', 'z', '\b', '\x4', '\x4', '\x2', 'z', '{', '\b', '\x4', '\x3', '\x2', 
		'{', '\t', '\x3', '\x2', '\x2', '\x2', '|', '~', '\t', '\x2', '\x2', '\x2', 
		'}', '|', '\x3', '\x2', '\x2', '\x2', '~', '\x7F', '\x3', '\x2', '\x2', 
		'\x2', '\x7F', '}', '\x3', '\x2', '\x2', '\x2', '\x7F', '\x80', '\x3', 
		'\x2', '\x2', '\x2', '\x80', '\v', '\x3', '\x2', '\x2', '\x2', '\x81', 
		'\x83', '\t', '\x2', '\x2', '\x2', '\x82', '\x81', '\x3', '\x2', '\x2', 
		'\x2', '\x83', '\x84', '\x3', '\x2', '\x2', '\x2', '\x84', '\x82', '\x3', 
		'\x2', '\x2', '\x2', '\x84', '\x85', '\x3', '\x2', '\x2', '\x2', '\x85', 
		'\x86', '\x3', '\x2', '\x2', '\x2', '\x86', '\x88', '\a', '\x30', '\x2', 
		'\x2', '\x87', '\x89', '\t', '\x2', '\x2', '\x2', '\x88', '\x87', '\x3', 
		'\x2', '\x2', '\x2', '\x89', '\x8A', '\x3', '\x2', '\x2', '\x2', '\x8A', 
		'\x88', '\x3', '\x2', '\x2', '\x2', '\x8A', '\x8B', '\x3', '\x2', '\x2', 
		'\x2', '\x8B', '\x95', '\x3', '\x2', '\x2', '\x2', '\x8C', '\x8E', '\t', 
		'\x3', '\x2', '\x2', '\x8D', '\x8F', '\t', '\x4', '\x2', '\x2', '\x8E', 
		'\x8D', '\x3', '\x2', '\x2', '\x2', '\x8E', '\x8F', '\x3', '\x2', '\x2', 
		'\x2', '\x8F', '\x91', '\x3', '\x2', '\x2', '\x2', '\x90', '\x92', '\t', 
		'\x2', '\x2', '\x2', '\x91', '\x90', '\x3', '\x2', '\x2', '\x2', '\x92', 
		'\x93', '\x3', '\x2', '\x2', '\x2', '\x93', '\x91', '\x3', '\x2', '\x2', 
		'\x2', '\x93', '\x94', '\x3', '\x2', '\x2', '\x2', '\x94', '\x96', '\x3', 
		'\x2', '\x2', '\x2', '\x95', '\x8C', '\x3', '\x2', '\x2', '\x2', '\x95', 
		'\x96', '\x3', '\x2', '\x2', '\x2', '\x96', '\r', '\x3', '\x2', '\x2', 
		'\x2', '\x97', '\x98', '\a', 'v', '\x2', '\x2', '\x98', '\x99', '\a', 
		't', '\x2', '\x2', '\x99', '\x9A', '\a', 'w', '\x2', '\x2', '\x9A', '\xA1', 
		'\a', 'g', '\x2', '\x2', '\x9B', '\x9C', '\a', 'h', '\x2', '\x2', '\x9C', 
		'\x9D', '\a', '\x63', '\x2', '\x2', '\x9D', '\x9E', '\a', 'n', '\x2', 
		'\x2', '\x9E', '\x9F', '\a', 'u', '\x2', '\x2', '\x9F', '\xA1', '\a', 
		'g', '\x2', '\x2', '\xA0', '\x97', '\x3', '\x2', '\x2', '\x2', '\xA0', 
		'\x9B', '\x3', '\x2', '\x2', '\x2', '\xA1', '\xF', '\x3', '\x2', '\x2', 
		'\x2', '\xA2', '\xA6', '\a', '$', '\x2', '\x2', '\xA3', '\xA5', '\v', 
		'\x2', '\x2', '\x2', '\xA4', '\xA3', '\x3', '\x2', '\x2', '\x2', '\xA5', 
		'\xA8', '\x3', '\x2', '\x2', '\x2', '\xA6', '\xA7', '\x3', '\x2', '\x2', 
		'\x2', '\xA6', '\xA4', '\x3', '\x2', '\x2', '\x2', '\xA7', '\xA9', '\x3', 
		'\x2', '\x2', '\x2', '\xA8', '\xA6', '\x3', '\x2', '\x2', '\x2', '\xA9', 
		'\xAA', '\a', '$', '\x2', '\x2', '\xAA', '\x11', '\x3', '\x2', '\x2', 
		'\x2', '\xAB', '\xAC', '\a', '}', '\x2', '\x2', '\xAC', '\x13', '\x3', 
		'\x2', '\x2', '\x2', '\xAD', '\xAE', '\a', '\x7F', '\x2', '\x2', '\xAE', 
		'\x15', '\x3', '\x2', '\x2', '\x2', '\xAF', '\xB0', '\a', '-', '\x2', 
		'\x2', '\xB0', '\xB1', '\a', '-', '\x2', '\x2', '\xB1', '\x17', '\x3', 
		'\x2', '\x2', '\x2', '\xB2', '\xB3', '\a', '/', '\x2', '\x2', '\xB3', 
		'\xB4', '\a', '/', '\x2', '\x2', '\xB4', '\x19', '\x3', '\x2', '\x2', 
		'\x2', '\xB5', '\xB6', '\a', ',', '\x2', '\x2', '\xB6', '\x1B', '\x3', 
		'\x2', '\x2', '\x2', '\xB7', '\xB8', '\a', '\x31', '\x2', '\x2', '\xB8', 
		'\x1D', '\x3', '\x2', '\x2', '\x2', '\xB9', '\xBA', '\a', '\'', '\x2', 
		'\x2', '\xBA', '\x1F', '\x3', '\x2', '\x2', '\x2', '\xBB', '\xBC', '\a', 
		'-', '\x2', '\x2', '\xBC', '!', '\x3', '\x2', '\x2', '\x2', '\xBD', '\xBE', 
		'\a', '/', '\x2', '\x2', '\xBE', '#', '\x3', '\x2', '\x2', '\x2', '\xBF', 
		'\xC0', '\a', '*', '\x2', '\x2', '\xC0', '%', '\x3', '\x2', '\x2', '\x2', 
		'\xC1', '\xC2', '\a', '+', '\x2', '\x2', '\xC2', '\'', '\x3', '\x2', '\x2', 
		'\x2', '\xC3', '\xC4', '\a', '\x30', '\x2', '\x2', '\xC4', ')', '\x3', 
		'\x2', '\x2', '\x2', '\xC5', '\xC6', '\a', '.', '\x2', '\x2', '\xC6', 
		'+', '\x3', '\x2', '\x2', '\x2', '\xC7', '\xC8', '\a', '#', '\x2', '\x2', 
		'\xC8', '\xC9', '\a', '?', '\x2', '\x2', '\xC9', '-', '\x3', '\x2', '\x2', 
		'\x2', '\xCA', '\xCB', '\a', '#', '\x2', '\x2', '\xCB', '/', '\x3', '\x2', 
		'\x2', '\x2', '\xCC', '\xCD', '\a', '\x80', '\x2', '\x2', '\xCD', '\x31', 
		'\x3', '\x2', '\x2', '\x2', '\xCE', '\xCF', '\a', '>', '\x2', '\x2', '\xCF', 
		'\xD0', '\a', '>', '\x2', '\x2', '\xD0', '\x33', '\x3', '\x2', '\x2', 
		'\x2', '\xD1', '\xD2', '\a', '@', '\x2', '\x2', '\xD2', '\xD3', '\a', 
		'@', '\x2', '\x2', '\xD3', '\x35', '\x3', '\x2', '\x2', '\x2', '\xD4', 
		'\xD5', '\a', '>', '\x2', '\x2', '\xD5', '\x37', '\x3', '\x2', '\x2', 
		'\x2', '\xD6', '\xD7', '\a', '>', '\x2', '\x2', '\xD7', '\xD8', '\a', 
		'?', '\x2', '\x2', '\xD8', '\x39', '\x3', '\x2', '\x2', '\x2', '\xD9', 
		'\xDA', '\a', '@', '\x2', '\x2', '\xDA', ';', '\x3', '\x2', '\x2', '\x2', 
		'\xDB', '\xDC', '\a', '@', '\x2', '\x2', '\xDC', '\xDD', '\a', '?', '\x2', 
		'\x2', '\xDD', '=', '\x3', '\x2', '\x2', '\x2', '\xDE', '\xDF', '\a', 
		'?', '\x2', '\x2', '\xDF', '\xE0', '\a', '?', '\x2', '\x2', '\xE0', '?', 
		'\x3', '\x2', '\x2', '\x2', '\xE1', '\xE2', '\a', '(', '\x2', '\x2', '\xE2', 
		'\xE3', '\a', '(', '\x2', '\x2', '\xE3', '\x41', '\x3', '\x2', '\x2', 
		'\x2', '\xE4', '\xE5', '\a', '~', '\x2', '\x2', '\xE5', '\xE6', '\a', 
		'~', '\x2', '\x2', '\xE6', '\x43', '\x3', '\x2', '\x2', '\x2', '\xE7', 
		'\xE8', '\a', '(', '\x2', '\x2', '\xE8', '\x45', '\x3', '\x2', '\x2', 
		'\x2', '\xE9', '\xEA', '\a', '~', '\x2', '\x2', '\xEA', 'G', '\x3', '\x2', 
		'\x2', '\x2', '\xEB', '\xEC', '\a', '`', '\x2', '\x2', '\xEC', 'I', '\x3', 
		'\x2', '\x2', '\x2', '\xED', '\xEE', '\a', '<', '\x2', '\x2', '\xEE', 
		'K', '\x3', '\x2', '\x2', '\x2', '\xEF', '\xF0', '\a', '=', '\x2', '\x2', 
		'\xF0', 'M', '\x3', '\x2', '\x2', '\x2', '\xF1', '\xF2', '\a', 'x', '\x2', 
		'\x2', '\xF2', '\xF3', '\a', '\x63', '\x2', '\x2', '\xF3', '\xF4', '\a', 
		't', '\x2', '\x2', '\xF4', 'O', '\x3', '\x2', '\x2', '\x2', '\xF5', '\xF6', 
		'\a', 'k', '\x2', '\x2', '\xF6', '\xF7', '\a', 'h', '\x2', '\x2', '\xF7', 
		'Q', '\x3', '\x2', '\x2', '\x2', '\xF8', '\xF9', '\a', 'u', '\x2', '\x2', 
		'\xF9', '\xFA', '\a', 'y', '\x2', '\x2', '\xFA', '\xFB', '\a', 'k', '\x2', 
		'\x2', '\xFB', '\xFC', '\a', 'v', '\x2', '\x2', '\xFC', '\xFD', '\a', 
		'\x65', '\x2', '\x2', '\xFD', '\xFE', '\a', 'j', '\x2', '\x2', '\xFE', 
		'S', '\x3', '\x2', '\x2', '\x2', '\xFF', '\x100', '\a', '\x65', '\x2', 
		'\x2', '\x100', '\x101', '\a', '\x63', '\x2', '\x2', '\x101', '\x102', 
		'\a', 'u', '\x2', '\x2', '\x102', '\x103', '\a', 'g', '\x2', '\x2', '\x103', 
		'U', '\x3', '\x2', '\x2', '\x2', '\x104', '\x105', '\a', 'y', '\x2', '\x2', 
		'\x105', '\x106', '\a', 'j', '\x2', '\x2', '\x106', '\x107', '\a', 'k', 
		'\x2', '\x2', '\x107', '\x108', '\a', 'n', '\x2', '\x2', '\x108', '\x109', 
		'\a', 'g', '\x2', '\x2', '\x109', 'W', '\x3', '\x2', '\x2', '\x2', '\x10A', 
		'\x10B', '\a', '\x66', '\x2', '\x2', '\x10B', '\x10C', '\a', 'q', '\x2', 
		'\x2', '\x10C', 'Y', '\x3', '\x2', '\x2', '\x2', '\x10D', '\x10E', '\a', 
		'h', '\x2', '\x2', '\x10E', '\x10F', '\a', 'q', '\x2', '\x2', '\x10F', 
		'\x110', '\a', 't', '\x2', '\x2', '\x110', '[', '\x3', '\x2', '\x2', '\x2', 
		'\x111', '\x112', '\a', 'h', '\x2', '\x2', '\x112', '\x113', '\a', 'q', 
		'\x2', '\x2', '\x113', '\x114', '\a', 't', '\x2', '\x2', '\x114', '\x115', 
		'\a', 'g', '\x2', '\x2', '\x115', '\x116', '\a', '\x63', '\x2', '\x2', 
		'\x116', '\x117', '\a', '\x65', '\x2', '\x2', '\x117', '\x118', '\a', 
		'j', '\x2', '\x2', '\x118', ']', '\x3', '\x2', '\x2', '\x2', '\x119', 
		'\x11A', '\a', 'k', '\x2', '\x2', '\x11A', '\x11B', '\a', 'p', '\x2', 
		'\x2', '\x11B', '_', '\x3', '\x2', '\x2', '\x2', '\x11C', '\x11D', '\a', 
		'\x64', '\x2', '\x2', '\x11D', '\x11E', '\a', 't', '\x2', '\x2', '\x11E', 
		'\x11F', '\a', 'g', '\x2', '\x2', '\x11F', '\x120', '\a', '\x63', '\x2', 
		'\x2', '\x120', '\x121', '\a', 'm', '\x2', '\x2', '\x121', '\x61', '\x3', 
		'\x2', '\x2', '\x2', '\x122', '\x123', '\a', '\x65', '\x2', '\x2', '\x123', 
		'\x124', '\a', 'q', '\x2', '\x2', '\x124', '\x125', '\a', 'p', '\x2', 
		'\x2', '\x125', '\x126', '\a', 'v', '\x2', '\x2', '\x126', '\x127', '\a', 
		'k', '\x2', '\x2', '\x127', '\x128', '\a', 'p', '\x2', '\x2', '\x128', 
		'\x129', '\a', 'w', '\x2', '\x2', '\x129', '\x12A', '\a', 'g', '\x2', 
		'\x2', '\x12A', '\x63', '\x3', '\x2', '\x2', '\x2', '\x12B', '\x12F', 
		'\t', '\x5', '\x2', '\x2', '\x12C', '\x12E', '\t', '\x6', '\x2', '\x2', 
		'\x12D', '\x12C', '\x3', '\x2', '\x2', '\x2', '\x12E', '\x131', '\x3', 
		'\x2', '\x2', '\x2', '\x12F', '\x12D', '\x3', '\x2', '\x2', '\x2', '\x12F', 
		'\x130', '\x3', '\x2', '\x2', '\x2', '\x130', '\x65', '\x3', '\x2', '\x2', 
		'\x2', '\x131', '\x12F', '\x3', '\x2', '\x2', '\x2', '\x132', '\x133', 
		'\a', '\x31', '\x2', '\x2', '\x133', '\x134', '\a', '\x31', '\x2', '\x2', 
		'\x134', '\x138', '\x3', '\x2', '\x2', '\x2', '\x135', '\x137', '\n', 
		'\a', '\x2', '\x2', '\x136', '\x135', '\x3', '\x2', '\x2', '\x2', '\x137', 
		'\x13A', '\x3', '\x2', '\x2', '\x2', '\x138', '\x136', '\x3', '\x2', '\x2', 
		'\x2', '\x138', '\x139', '\x3', '\x2', '\x2', '\x2', '\x139', '\x13C', 
		'\x3', '\x2', '\x2', '\x2', '\x13A', '\x138', '\x3', '\x2', '\x2', '\x2', 
		'\x13B', '\x13D', '\a', '\xF', '\x2', '\x2', '\x13C', '\x13B', '\x3', 
		'\x2', '\x2', '\x2', '\x13C', '\x13D', '\x3', '\x2', '\x2', '\x2', '\x13D', 
		'\x13E', '\x3', '\x2', '\x2', '\x2', '\x13E', '\x14B', '\a', '\f', '\x2', 
		'\x2', '\x13F', '\x140', '\a', '\x31', '\x2', '\x2', '\x140', '\x141', 
		'\a', ',', '\x2', '\x2', '\x141', '\x145', '\x3', '\x2', '\x2', '\x2', 
		'\x142', '\x144', '\v', '\x2', '\x2', '\x2', '\x143', '\x142', '\x3', 
		'\x2', '\x2', '\x2', '\x144', '\x147', '\x3', '\x2', '\x2', '\x2', '\x145', 
		'\x146', '\x3', '\x2', '\x2', '\x2', '\x145', '\x143', '\x3', '\x2', '\x2', 
		'\x2', '\x146', '\x148', '\x3', '\x2', '\x2', '\x2', '\x147', '\x145', 
		'\x3', '\x2', '\x2', '\x2', '\x148', '\x149', '\a', ',', '\x2', '\x2', 
		'\x149', '\x14B', '\a', '\x31', '\x2', '\x2', '\x14A', '\x132', '\x3', 
		'\x2', '\x2', '\x2', '\x14A', '\x13F', '\x3', '\x2', '\x2', '\x2', '\x14B', 
		'\x14C', '\x3', '\x2', '\x2', '\x2', '\x14C', '\x14D', '\b', '\x33', '\x3', 
		'\x2', '\x14D', 'g', '\x3', '\x2', '\x2', '\x2', '\x14E', '\x150', '\t', 
		'\b', '\x2', '\x2', '\x14F', '\x14E', '\x3', '\x2', '\x2', '\x2', '\x150', 
		'\x151', '\x3', '\x2', '\x2', '\x2', '\x151', '\x14F', '\x3', '\x2', '\x2', 
		'\x2', '\x151', '\x152', '\x3', '\x2', '\x2', '\x2', '\x152', '\x153', 
		'\x3', '\x2', '\x2', '\x2', '\x153', '\x154', '\b', '\x34', '\x5', '\x2', 
		'\x154', 'i', '\x3', '\x2', '\x2', '\x2', '\x13', '\x2', '\x3', 't', '\x7F', 
		'\x84', '\x8A', '\x8E', '\x93', '\x95', '\xA0', '\xA6', '\x12F', '\x138', 
		'\x13C', '\x145', '\x14A', '\x151', '\x6', '\x4', '\x3', '\x2', '\x2', 
		'\x3', '\x2', '\x4', '\x2', '\x2', '\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
