//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from TemplateParser.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="TemplateParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface ITemplateParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.document"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDocument([NotNull] TemplateParser.DocumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.document"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDocument([NotNull] TemplateParser.DocumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.code"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCode([NotNull] TemplateParser.CodeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.code"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCode([NotNull] TemplateParser.CodeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.text"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterText([NotNull] TemplateParser.TextContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.text"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitText([NotNull] TemplateParser.TextContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.var"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVar([NotNull] TemplateParser.VarContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.var"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVar([NotNull] TemplateParser.VarContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.if"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIf([NotNull] TemplateParser.IfContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.if"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIf([NotNull] TemplateParser.IfContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.switch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitch([NotNull] TemplateParser.SwitchContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.switch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitch([NotNull] TemplateParser.SwitchContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.while"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhile([NotNull] TemplateParser.WhileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.while"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhile([NotNull] TemplateParser.WhileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.dowhile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDowhile([NotNull] TemplateParser.DowhileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.dowhile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDowhile([NotNull] TemplateParser.DowhileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.for"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFor([NotNull] TemplateParser.ForContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.for"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFor([NotNull] TemplateParser.ForContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.foreach"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForeach([NotNull] TemplateParser.ForeachContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.foreach"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForeach([NotNull] TemplateParser.ForeachContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr([NotNull] TemplateParser.ExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr([NotNull] TemplateParser.ExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.exprCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExprCall([NotNull] TemplateParser.ExprCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.exprCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExprCall([NotNull] TemplateParser.ExprCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.exprProp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExprProp([NotNull] TemplateParser.ExprPropContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.exprProp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExprProp([NotNull] TemplateParser.ExprPropContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.exprValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExprValue([NotNull] TemplateParser.ExprValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.exprValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExprValue([NotNull] TemplateParser.ExprValueContext context);
}
