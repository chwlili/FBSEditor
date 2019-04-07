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
		RULE_jsonValue = 0, RULE_jsonArray = 1, RULE_jsonObject = 2, RULE_jsonProp = 3;
	public static readonly string[] ruleNames = {
		"jsonValue", "jsonArray", "jsonObject", "jsonProp"
	};

	private static readonly string[] _LiteralNames = {
		null, "'null'", "'['", "','", "']'", "'{'", "'}'", "':'"
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
	public partial class JsonValueContext : ParserRuleContext {
		public IToken strValue;
		public IToken intValue;
		public IToken floatValue;
		public IToken boolValue;
		public IToken nullValue;
		public JsonObjectContext objectValue;
		public JsonArrayContext arraryValue;
		public ITerminalNode STRING() { return GetToken(JsonParser.STRING, 0); }
		public ITerminalNode INTEGER() { return GetToken(JsonParser.INTEGER, 0); }
		public ITerminalNode FLOAT() { return GetToken(JsonParser.FLOAT, 0); }
		public ITerminalNode BOOL() { return GetToken(JsonParser.BOOL, 0); }
		public JsonObjectContext jsonObject() {
			return GetRuleContext<JsonObjectContext>(0);
		}
		public JsonArrayContext jsonArray() {
			return GetRuleContext<JsonArrayContext>(0);
		}
		public JsonValueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_jsonValue; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterJsonValue(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitJsonValue(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitJsonValue(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public JsonValueContext jsonValue() {
		JsonValueContext _localctx = new JsonValueContext(Context, State);
		EnterRule(_localctx, 0, RULE_jsonValue);
		try {
			State = 15;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case STRING:
				EnterOuterAlt(_localctx, 1);
				{
				State = 8; _localctx.strValue = Match(STRING);
				}
				break;
			case INTEGER:
				EnterOuterAlt(_localctx, 2);
				{
				State = 9; _localctx.intValue = Match(INTEGER);
				}
				break;
			case FLOAT:
				EnterOuterAlt(_localctx, 3);
				{
				State = 10; _localctx.floatValue = Match(FLOAT);
				}
				break;
			case BOOL:
				EnterOuterAlt(_localctx, 4);
				{
				State = 11; _localctx.boolValue = Match(BOOL);
				}
				break;
			case T__0:
				EnterOuterAlt(_localctx, 5);
				{
				State = 12; _localctx.nullValue = Match(T__0);
				}
				break;
			case T__4:
				EnterOuterAlt(_localctx, 6);
				{
				State = 13; _localctx.objectValue = jsonObject();
				}
				break;
			case T__1:
				EnterOuterAlt(_localctx, 7);
				{
				State = 14; _localctx.arraryValue = jsonArray();
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

	public partial class JsonArrayContext : ParserRuleContext {
		public JsonValueContext _jsonValue;
		public IList<JsonValueContext> _arrayElement = new List<JsonValueContext>();
		public JsonValueContext[] jsonValue() {
			return GetRuleContexts<JsonValueContext>();
		}
		public JsonValueContext jsonValue(int i) {
			return GetRuleContext<JsonValueContext>(i);
		}
		public JsonArrayContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_jsonArray; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterJsonArray(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitJsonArray(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitJsonArray(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public JsonArrayContext jsonArray() {
		JsonArrayContext _localctx = new JsonArrayContext(Context, State);
		EnterRule(_localctx, 2, RULE_jsonArray);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 17; Match(T__1);
			State = 26;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__1) | (1L << T__4) | (1L << INTEGER) | (1L << FLOAT) | (1L << BOOL) | (1L << STRING))) != 0)) {
				{
				State = 18; _localctx._jsonValue = jsonValue();
				_localctx._arrayElement.Add(_localctx._jsonValue);
				State = 23;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__2) {
					{
					{
					State = 19; Match(T__2);
					State = 20; _localctx._jsonValue = jsonValue();
					_localctx._arrayElement.Add(_localctx._jsonValue);
					}
					}
					State = 25;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
			}

			State = 28; Match(T__3);
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

	public partial class JsonObjectContext : ParserRuleContext {
		public JsonPropContext _jsonProp;
		public IList<JsonPropContext> _props = new List<JsonPropContext>();
		public JsonPropContext[] jsonProp() {
			return GetRuleContexts<JsonPropContext>();
		}
		public JsonPropContext jsonProp(int i) {
			return GetRuleContext<JsonPropContext>(i);
		}
		public JsonObjectContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_jsonObject; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterJsonObject(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitJsonObject(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitJsonObject(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public JsonObjectContext jsonObject() {
		JsonObjectContext _localctx = new JsonObjectContext(Context, State);
		EnterRule(_localctx, 4, RULE_jsonObject);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 30; Match(T__4);
			State = 39;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==STRING || _la==IDENT) {
				{
				State = 31; _localctx._jsonProp = jsonProp();
				_localctx._props.Add(_localctx._jsonProp);
				State = 36;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__2) {
					{
					{
					State = 32; Match(T__2);
					State = 33; _localctx._jsonProp = jsonProp();
					_localctx._props.Add(_localctx._jsonProp);
					}
					}
					State = 38;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
			}

			State = 41; Match(T__5);
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

	public partial class JsonPropContext : ParserRuleContext {
		public IToken propName;
		public JsonValueContext propValue;
		public JsonValueContext jsonValue() {
			return GetRuleContext<JsonValueContext>(0);
		}
		public ITerminalNode STRING() { return GetToken(JsonParser.STRING, 0); }
		public ITerminalNode IDENT() { return GetToken(JsonParser.IDENT, 0); }
		public JsonPropContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_jsonProp; } }
		public override void EnterRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.EnterJsonProp(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IJsonListener typedListener = listener as IJsonListener;
			if (typedListener != null) typedListener.ExitJsonProp(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IJsonVisitor<TResult> typedVisitor = visitor as IJsonVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitJsonProp(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public JsonPropContext jsonProp() {
		JsonPropContext _localctx = new JsonPropContext(Context, State);
		EnterRule(_localctx, 6, RULE_jsonProp);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 45;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case STRING:
				{
				State = 43; _localctx.propName = Match(STRING);
				}
				break;
			case IDENT:
				{
				State = 44; _localctx.propName = Match(IDENT);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			State = 47; Match(T__6);
			State = 48; _localctx.propValue = jsonValue();
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
		'\x5964', '\x3', '\xF', '\x35', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x5', '\x2', '\x12', '\n', '\x2', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\a', '\x3', '\x18', '\n', '\x3', 
		'\f', '\x3', '\xE', '\x3', '\x1B', '\v', '\x3', '\x5', '\x3', '\x1D', 
		'\n', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\a', '\x4', '%', '\n', '\x4', '\f', '\x4', '\xE', 
		'\x4', '(', '\v', '\x4', '\x5', '\x4', '*', '\n', '\x4', '\x3', '\x4', 
		'\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x5', '\x5', '\x30', '\n', 
		'\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x2', 
		'\x2', '\x6', '\x2', '\x4', '\x6', '\b', '\x2', '\x2', '\x2', ';', '\x2', 
		'\x11', '\x3', '\x2', '\x2', '\x2', '\x4', '\x13', '\x3', '\x2', '\x2', 
		'\x2', '\x6', ' ', '\x3', '\x2', '\x2', '\x2', '\b', '/', '\x3', '\x2', 
		'\x2', '\x2', '\n', '\x12', '\a', '\r', '\x2', '\x2', '\v', '\x12', '\a', 
		'\n', '\x2', '\x2', '\f', '\x12', '\a', '\v', '\x2', '\x2', '\r', '\x12', 
		'\a', '\f', '\x2', '\x2', '\xE', '\x12', '\a', '\x3', '\x2', '\x2', '\xF', 
		'\x12', '\x5', '\x6', '\x4', '\x2', '\x10', '\x12', '\x5', '\x4', '\x3', 
		'\x2', '\x11', '\n', '\x3', '\x2', '\x2', '\x2', '\x11', '\v', '\x3', 
		'\x2', '\x2', '\x2', '\x11', '\f', '\x3', '\x2', '\x2', '\x2', '\x11', 
		'\r', '\x3', '\x2', '\x2', '\x2', '\x11', '\xE', '\x3', '\x2', '\x2', 
		'\x2', '\x11', '\xF', '\x3', '\x2', '\x2', '\x2', '\x11', '\x10', '\x3', 
		'\x2', '\x2', '\x2', '\x12', '\x3', '\x3', '\x2', '\x2', '\x2', '\x13', 
		'\x1C', '\a', '\x4', '\x2', '\x2', '\x14', '\x19', '\x5', '\x2', '\x2', 
		'\x2', '\x15', '\x16', '\a', '\x5', '\x2', '\x2', '\x16', '\x18', '\x5', 
		'\x2', '\x2', '\x2', '\x17', '\x15', '\x3', '\x2', '\x2', '\x2', '\x18', 
		'\x1B', '\x3', '\x2', '\x2', '\x2', '\x19', '\x17', '\x3', '\x2', '\x2', 
		'\x2', '\x19', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x1A', '\x1D', '\x3', 
		'\x2', '\x2', '\x2', '\x1B', '\x19', '\x3', '\x2', '\x2', '\x2', '\x1C', 
		'\x14', '\x3', '\x2', '\x2', '\x2', '\x1C', '\x1D', '\x3', '\x2', '\x2', 
		'\x2', '\x1D', '\x1E', '\x3', '\x2', '\x2', '\x2', '\x1E', '\x1F', '\a', 
		'\x6', '\x2', '\x2', '\x1F', '\x5', '\x3', '\x2', '\x2', '\x2', ' ', ')', 
		'\a', '\a', '\x2', '\x2', '!', '&', '\x5', '\b', '\x5', '\x2', '\"', '#', 
		'\a', '\x5', '\x2', '\x2', '#', '%', '\x5', '\b', '\x5', '\x2', '$', '\"', 
		'\x3', '\x2', '\x2', '\x2', '%', '(', '\x3', '\x2', '\x2', '\x2', '&', 
		'$', '\x3', '\x2', '\x2', '\x2', '&', '\'', '\x3', '\x2', '\x2', '\x2', 
		'\'', '*', '\x3', '\x2', '\x2', '\x2', '(', '&', '\x3', '\x2', '\x2', 
		'\x2', ')', '!', '\x3', '\x2', '\x2', '\x2', ')', '*', '\x3', '\x2', '\x2', 
		'\x2', '*', '+', '\x3', '\x2', '\x2', '\x2', '+', ',', '\a', '\b', '\x2', 
		'\x2', ',', '\a', '\x3', '\x2', '\x2', '\x2', '-', '\x30', '\a', '\r', 
		'\x2', '\x2', '.', '\x30', '\a', '\xE', '\x2', '\x2', '/', '-', '\x3', 
		'\x2', '\x2', '\x2', '/', '.', '\x3', '\x2', '\x2', '\x2', '\x30', '\x31', 
		'\x3', '\x2', '\x2', '\x2', '\x31', '\x32', '\a', '\t', '\x2', '\x2', 
		'\x32', '\x33', '\x5', '\x2', '\x2', '\x2', '\x33', '\t', '\x3', '\x2', 
		'\x2', '\x2', '\b', '\x11', '\x19', '\x1C', '&', ')', '/',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
