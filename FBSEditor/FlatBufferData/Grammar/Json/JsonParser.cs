//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Json.g4 by ANTLR 4.7.1

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
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class JsonParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, INTEGER=8, FLOAT=9, 
		BOOL=10, STRING=11, IDENT=12, WS=13;
	public const int
		RULE_root = 0, RULE_array = 1, RULE_struct = 2, RULE_prop = 3, RULE_value = 4;
	public static readonly string[] ruleNames = {
		"root", "array", "struct", "prop", "value"
	};

	private static readonly string[] _LiteralNames = {
		null, "'['", "','", "']'", "'{'", "'}'", "':'", "'null'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, "INTEGER", "FLOAT", "BOOL", 
		"STRING", "IDENT", "WS"
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

	public override string GrammarFileName { get { return "Json.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static JsonParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public JsonParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public JsonParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}
	public partial class RootContext : ParserRuleContext {
		public ArrayContext array() {
			return GetRuleContext<ArrayContext>(0);
		}
		public StructContext @struct() {
			return GetRuleContext<StructContext>(0);
		}
		public RootContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_root; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterRoot(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitRoot(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitRoot(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public RootContext root() {
		RootContext _localctx = new RootContext(Context, State);
		EnterRule(_localctx, 0, RULE_root);
		try {
			State = 12;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__0:
				EnterOuterAlt(_localctx, 1);
				{
				State = 10; array();
				}
				break;
			case T__3:
				EnterOuterAlt(_localctx, 2);
				{
				State = 11; @struct();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ArrayContext : ParserRuleContext {
		public ValueContext[] value() {
			return GetRuleContexts<ValueContext>();
		}
		public ValueContext value(int i) {
			return GetRuleContext<ValueContext>(i);
		}
		public ArrayContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_array; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterArray(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitArray(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitArray(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ArrayContext array() {
		ArrayContext _localctx = new ArrayContext(Context, State);
		EnterRule(_localctx, 2, RULE_array);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 14; Match(T__0);
			State = 23;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__3) | (1L << T__6) | (1L << INTEGER) | (1L << FLOAT) | (1L << BOOL) | (1L << STRING))) != 0)) {
				{
				State = 15; value();
				State = 20;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__1) {
					{
					{
					State = 16; Match(T__1);
					State = 17; value();
					}
					}
					State = 22;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
			}

			State = 25; Match(T__2);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class StructContext : ParserRuleContext {
		public PropContext _prop;
		public IList<PropContext> _props = new List<PropContext>();
		public PropContext[] prop() {
			return GetRuleContexts<PropContext>();
		}
		public PropContext prop(int i) {
			return GetRuleContext<PropContext>(i);
		}
		public StructContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_struct; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterStruct(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitStruct(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitStruct(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public StructContext @struct() {
		StructContext _localctx = new StructContext(Context, State);
		EnterRule(_localctx, 4, RULE_struct);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 27; Match(T__3);
			State = 36;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==STRING || _la==IDENT) {
				{
				State = 28; _localctx._prop = prop();
				_localctx._props.Add(_localctx._prop);
				State = 33;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__1) {
					{
					{
					State = 29; Match(T__1);
					State = 30; _localctx._prop = prop();
					_localctx._props.Add(_localctx._prop);
					}
					}
					State = 35;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
			}

			State = 38; Match(T__4);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class PropContext : ParserRuleContext {
		public IToken propName;
		public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public ITerminalNode STRING() { return GetToken(JsonParser.STRING, 0); }
		public ITerminalNode IDENT() { return GetToken(JsonParser.IDENT, 0); }
		public PropContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_prop; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterProp(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitProp(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitProp(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public PropContext prop() {
		PropContext _localctx = new PropContext(Context, State);
		EnterRule(_localctx, 6, RULE_prop);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 42;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case STRING:
				{
				State = 40; _localctx.propName = Match(STRING);
				}
				break;
			case IDENT:
				{
				State = 41; _localctx.propName = Match(IDENT);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			State = 44; Match(T__5);
			State = 45; value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ValueContext : ParserRuleContext {
		public IToken strValue;
		public IToken intValue;
		public IToken floatValue;
		public IToken boolValue;
		public IToken nullValue;
		public StructContext structValue;
		public ArrayContext arraryValue;
		public ITerminalNode STRING() { return GetToken(JsonParser.STRING, 0); }
		public ITerminalNode INTEGER() { return GetToken(JsonParser.INTEGER, 0); }
		public ITerminalNode FLOAT() { return GetToken(JsonParser.FLOAT, 0); }
		public ITerminalNode BOOL() { return GetToken(JsonParser.BOOL, 0); }
		public StructContext @struct() {
			return GetRuleContext<StructContext>(0);
		}
		public ArrayContext array() {
			return GetRuleContext<ArrayContext>(0);
		}
		public ValueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_value; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterValue(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitValue(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitValue(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ValueContext value() {
		ValueContext _localctx = new ValueContext(Context, State);
		EnterRule(_localctx, 8, RULE_value);
		try {
			State = 54;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case STRING:
				EnterOuterAlt(_localctx, 1);
				{
				State = 47; _localctx.strValue = Match(STRING);
				}
				break;
			case INTEGER:
				EnterOuterAlt(_localctx, 2);
				{
				State = 48; _localctx.intValue = Match(INTEGER);
				}
				break;
			case FLOAT:
				EnterOuterAlt(_localctx, 3);
				{
				State = 49; _localctx.floatValue = Match(FLOAT);
				}
				break;
			case BOOL:
				EnterOuterAlt(_localctx, 4);
				{
				State = 50; _localctx.boolValue = Match(BOOL);
				}
				break;
			case T__6:
				EnterOuterAlt(_localctx, 5);
				{
				State = 51; _localctx.nullValue = Match(T__6);
				}
				break;
			case T__3:
				EnterOuterAlt(_localctx, 6);
				{
				State = 52; _localctx.structValue = @struct();
				}
				break;
			case T__0:
				EnterOuterAlt(_localctx, 7);
				{
				State = 53; _localctx.arraryValue = array();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\xF', ';', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x4', 
		'\x6', '\t', '\x6', '\x3', '\x2', '\x3', '\x2', '\x5', '\x2', '\xF', '\n', 
		'\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\a', '\x3', 
		'\x15', '\n', '\x3', '\f', '\x3', '\xE', '\x3', '\x18', '\v', '\x3', '\x5', 
		'\x3', '\x1A', '\n', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\a', '\x4', '\"', '\n', '\x4', 
		'\f', '\x4', '\xE', '\x4', '%', '\v', '\x4', '\x5', '\x4', '\'', '\n', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x5', 
		'\x5', '-', '\n', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', 
		'\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', 
		'\x6', '\x3', '\x6', '\x5', '\x6', '\x39', '\n', '\x6', '\x3', '\x6', 
		'\x2', '\x2', '\a', '\x2', '\x4', '\x6', '\b', '\n', '\x2', '\x2', '\x2', 
		'\x41', '\x2', '\xE', '\x3', '\x2', '\x2', '\x2', '\x4', '\x10', '\x3', 
		'\x2', '\x2', '\x2', '\x6', '\x1D', '\x3', '\x2', '\x2', '\x2', '\b', 
		',', '\x3', '\x2', '\x2', '\x2', '\n', '\x38', '\x3', '\x2', '\x2', '\x2', 
		'\f', '\xF', '\x5', '\x4', '\x3', '\x2', '\r', '\xF', '\x5', '\x6', '\x4', 
		'\x2', '\xE', '\f', '\x3', '\x2', '\x2', '\x2', '\xE', '\r', '\x3', '\x2', 
		'\x2', '\x2', '\xF', '\x3', '\x3', '\x2', '\x2', '\x2', '\x10', '\x19', 
		'\a', '\x3', '\x2', '\x2', '\x11', '\x16', '\x5', '\n', '\x6', '\x2', 
		'\x12', '\x13', '\a', '\x4', '\x2', '\x2', '\x13', '\x15', '\x5', '\n', 
		'\x6', '\x2', '\x14', '\x12', '\x3', '\x2', '\x2', '\x2', '\x15', '\x18', 
		'\x3', '\x2', '\x2', '\x2', '\x16', '\x14', '\x3', '\x2', '\x2', '\x2', 
		'\x16', '\x17', '\x3', '\x2', '\x2', '\x2', '\x17', '\x1A', '\x3', '\x2', 
		'\x2', '\x2', '\x18', '\x16', '\x3', '\x2', '\x2', '\x2', '\x19', '\x11', 
		'\x3', '\x2', '\x2', '\x2', '\x19', '\x1A', '\x3', '\x2', '\x2', '\x2', 
		'\x1A', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x1B', '\x1C', '\a', '\x5', 
		'\x2', '\x2', '\x1C', '\x5', '\x3', '\x2', '\x2', '\x2', '\x1D', '&', 
		'\a', '\x6', '\x2', '\x2', '\x1E', '#', '\x5', '\b', '\x5', '\x2', '\x1F', 
		' ', '\a', '\x4', '\x2', '\x2', ' ', '\"', '\x5', '\b', '\x5', '\x2', 
		'!', '\x1F', '\x3', '\x2', '\x2', '\x2', '\"', '%', '\x3', '\x2', '\x2', 
		'\x2', '#', '!', '\x3', '\x2', '\x2', '\x2', '#', '$', '\x3', '\x2', '\x2', 
		'\x2', '$', '\'', '\x3', '\x2', '\x2', '\x2', '%', '#', '\x3', '\x2', 
		'\x2', '\x2', '&', '\x1E', '\x3', '\x2', '\x2', '\x2', '&', '\'', '\x3', 
		'\x2', '\x2', '\x2', '\'', '(', '\x3', '\x2', '\x2', '\x2', '(', ')', 
		'\a', '\a', '\x2', '\x2', ')', '\a', '\x3', '\x2', '\x2', '\x2', '*', 
		'-', '\a', '\r', '\x2', '\x2', '+', '-', '\a', '\xE', '\x2', '\x2', ',', 
		'*', '\x3', '\x2', '\x2', '\x2', ',', '+', '\x3', '\x2', '\x2', '\x2', 
		'-', '.', '\x3', '\x2', '\x2', '\x2', '.', '/', '\a', '\b', '\x2', '\x2', 
		'/', '\x30', '\x5', '\n', '\x6', '\x2', '\x30', '\t', '\x3', '\x2', '\x2', 
		'\x2', '\x31', '\x39', '\a', '\r', '\x2', '\x2', '\x32', '\x39', '\a', 
		'\n', '\x2', '\x2', '\x33', '\x39', '\a', '\v', '\x2', '\x2', '\x34', 
		'\x39', '\a', '\f', '\x2', '\x2', '\x35', '\x39', '\a', '\t', '\x2', '\x2', 
		'\x36', '\x39', '\x5', '\x6', '\x4', '\x2', '\x37', '\x39', '\x5', '\x4', 
		'\x3', '\x2', '\x38', '\x31', '\x3', '\x2', '\x2', '\x2', '\x38', '\x32', 
		'\x3', '\x2', '\x2', '\x2', '\x38', '\x33', '\x3', '\x2', '\x2', '\x2', 
		'\x38', '\x34', '\x3', '\x2', '\x2', '\x2', '\x38', '\x35', '\x3', '\x2', 
		'\x2', '\x2', '\x38', '\x36', '\x3', '\x2', '\x2', '\x2', '\x38', '\x37', 
		'\x3', '\x2', '\x2', '\x2', '\x39', '\v', '\x3', '\x2', '\x2', '\x2', 
		'\t', '\xE', '\x16', '\x19', '#', '&', ',', '\x38',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
