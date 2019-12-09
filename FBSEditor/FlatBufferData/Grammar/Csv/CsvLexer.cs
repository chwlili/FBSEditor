//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Csv.g4 by ANTLR 4.7.1

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
public partial class CsvLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		ROWEND=1, COLEND=2, STRING=3, TEXT=4, WS=5;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"ROWEND", "COLEND", "STRING", "TEXT", "WS"
	};


		public string separators = ",";
		public bool IsSeparator(string ch)
		{
			return  !string.IsNullOrEmpty(separators) && !string.IsNullOrEmpty(ch) ? separators.Contains(ch):false;
		}


	public CsvLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public CsvLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
	};
	private static readonly string[] _SymbolicNames = {
		null, "ROWEND", "COLEND", "STRING", "TEXT", "WS"
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

	public override string GrammarFileName { get { return "Csv.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static CsvLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 1 : return COLEND_sempred(_localctx, predIndex);
		case 3 : return TEXT_sempred(_localctx, predIndex);
		}
		return true;
	}
	private bool COLEND_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return IsSeparator(",");
		case 1: return IsSeparator(";");
		case 2: return IsSeparator("\t");
		}
		return true;
	}
	private bool TEXT_sempred(RuleContext _localctx, int predIndex) {
		switch (predIndex) {
		case 3: return IsSeparator(",");
		case 4: return IsSeparator(";");
		case 5: return IsSeparator("\t");
		}
		return true;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\a', '>', '\b', '\x1', '\x4', '\x2', '\t', '\x2', '\x4', 
		'\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', 
		'\x4', '\x6', '\t', '\x6', '\x3', '\x2', '\x5', '\x2', '\xF', '\n', '\x2', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x5', '\x3', '\x19', '\n', 
		'\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\a', '\x4', 
		'\x1F', '\n', '\x4', '\f', '\x4', '\xE', '\x4', '\"', '\v', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x5', '\x6', '\x5', '\'', '\n', '\x5', '\r', 
		'\x5', '\xE', '\x5', '(', '\x3', '\x5', '\x3', '\x5', '\x6', '\x5', '-', 
		'\n', '\x5', '\r', '\x5', '\xE', '\x5', '.', '\x3', '\x5', '\x3', '\x5', 
		'\x6', '\x5', '\x33', '\n', '\x5', '\r', '\x5', '\xE', '\x5', '\x34', 
		'\x3', '\x5', '\x5', '\x5', '\x38', '\n', '\x5', '\x3', '\x6', '\x6', 
		'\x6', ';', '\n', '\x6', '\r', '\x6', '\xE', '\x6', '<', '\x2', '\x2', 
		'\a', '\x3', '\x3', '\x5', '\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', 
		'\x3', '\x2', '\a', '\x3', '\x2', '$', '$', '\x6', '\x2', '\f', '\f', 
		'\xF', '\xF', '\"', '\"', '.', '.', '\x6', '\x2', '\f', '\f', '\xF', '\xF', 
		'\"', '\"', '=', '=', '\x5', '\x2', '\v', '\f', '\xF', '\xF', '\"', '\"', 
		'\x3', '\x2', '\"', '\"', '\x2', 'H', '\x2', '\x3', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\xE', '\x3', '\x2', '\x2', '\x2', '\x5', 
		'\x18', '\x3', '\x2', '\x2', '\x2', '\a', '\x1A', '\x3', '\x2', '\x2', 
		'\x2', '\t', '\x37', '\x3', '\x2', '\x2', '\x2', '\v', ':', '\x3', '\x2', 
		'\x2', '\x2', '\r', '\xF', '\a', '\xF', '\x2', '\x2', '\xE', '\r', '\x3', 
		'\x2', '\x2', '\x2', '\xE', '\xF', '\x3', '\x2', '\x2', '\x2', '\xF', 
		'\x10', '\x3', '\x2', '\x2', '\x2', '\x10', '\x11', '\a', '\f', '\x2', 
		'\x2', '\x11', '\x4', '\x3', '\x2', '\x2', '\x2', '\x12', '\x13', '\a', 
		'.', '\x2', '\x2', '\x13', '\x19', '\x6', '\x3', '\x2', '\x2', '\x14', 
		'\x15', '\a', '=', '\x2', '\x2', '\x15', '\x19', '\x6', '\x3', '\x3', 
		'\x2', '\x16', '\x17', '\a', '\v', '\x2', '\x2', '\x17', '\x19', '\x6', 
		'\x3', '\x4', '\x2', '\x18', '\x12', '\x3', '\x2', '\x2', '\x2', '\x18', 
		'\x14', '\x3', '\x2', '\x2', '\x2', '\x18', '\x16', '\x3', '\x2', '\x2', 
		'\x2', '\x19', '\x6', '\x3', '\x2', '\x2', '\x2', '\x1A', ' ', '\a', '$', 
		'\x2', '\x2', '\x1B', '\x1C', '\a', '$', '\x2', '\x2', '\x1C', '\x1F', 
		'\a', '$', '\x2', '\x2', '\x1D', '\x1F', '\n', '\x2', '\x2', '\x2', '\x1E', 
		'\x1B', '\x3', '\x2', '\x2', '\x2', '\x1E', '\x1D', '\x3', '\x2', '\x2', 
		'\x2', '\x1F', '\"', '\x3', '\x2', '\x2', '\x2', ' ', '\x1E', '\x3', '\x2', 
		'\x2', '\x2', ' ', '!', '\x3', '\x2', '\x2', '\x2', '!', '#', '\x3', '\x2', 
		'\x2', '\x2', '\"', ' ', '\x3', '\x2', '\x2', '\x2', '#', '$', '\a', '$', 
		'\x2', '\x2', '$', '\b', '\x3', '\x2', '\x2', '\x2', '%', '\'', '\n', 
		'\x3', '\x2', '\x2', '&', '%', '\x3', '\x2', '\x2', '\x2', '\'', '(', 
		'\x3', '\x2', '\x2', '\x2', '(', '&', '\x3', '\x2', '\x2', '\x2', '(', 
		')', '\x3', '\x2', '\x2', '\x2', ')', '*', '\x3', '\x2', '\x2', '\x2', 
		'*', '\x38', '\x6', '\x5', '\x5', '\x2', '+', '-', '\n', '\x4', '\x2', 
		'\x2', ',', '+', '\x3', '\x2', '\x2', '\x2', '-', '.', '\x3', '\x2', '\x2', 
		'\x2', '.', ',', '\x3', '\x2', '\x2', '\x2', '.', '/', '\x3', '\x2', '\x2', 
		'\x2', '/', '\x30', '\x3', '\x2', '\x2', '\x2', '\x30', '\x38', '\x6', 
		'\x5', '\x6', '\x2', '\x31', '\x33', '\n', '\x5', '\x2', '\x2', '\x32', 
		'\x31', '\x3', '\x2', '\x2', '\x2', '\x33', '\x34', '\x3', '\x2', '\x2', 
		'\x2', '\x34', '\x32', '\x3', '\x2', '\x2', '\x2', '\x34', '\x35', '\x3', 
		'\x2', '\x2', '\x2', '\x35', '\x36', '\x3', '\x2', '\x2', '\x2', '\x36', 
		'\x38', '\x6', '\x5', '\a', '\x2', '\x37', '&', '\x3', '\x2', '\x2', '\x2', 
		'\x37', ',', '\x3', '\x2', '\x2', '\x2', '\x37', '\x32', '\x3', '\x2', 
		'\x2', '\x2', '\x38', '\n', '\x3', '\x2', '\x2', '\x2', '\x39', ';', '\t', 
		'\x6', '\x2', '\x2', ':', '\x39', '\x3', '\x2', '\x2', '\x2', ';', '<', 
		'\x3', '\x2', '\x2', '\x2', '<', ':', '\x3', '\x2', '\x2', '\x2', '<', 
		'=', '\x3', '\x2', '\x2', '\x2', '=', '\f', '\x3', '\x2', '\x2', '\x2', 
		'\f', '\x2', '\xE', '\x18', '\x1E', ' ', '(', '.', '\x34', '\x37', '<', 
		'\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
