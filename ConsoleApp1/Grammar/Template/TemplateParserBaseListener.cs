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
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ITemplateParserListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class TemplateParserBaseListener : ITemplateParserListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.document"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDocument([NotNull] TemplateParser.DocumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.document"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDocument([NotNull] TemplateParser.DocumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.if"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIf([NotNull] TemplateParser.IfContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.if"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIf([NotNull] TemplateParser.IfContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpr([NotNull] TemplateParser.ExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.expr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpr([NotNull] TemplateParser.ExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.exprCall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprCall([NotNull] TemplateParser.ExprCallContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.exprCall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprCall([NotNull] TemplateParser.ExprCallContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.exprProp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprProp([NotNull] TemplateParser.ExprPropContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.exprProp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprProp([NotNull] TemplateParser.ExprPropContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="TemplateParser.exprValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprValue([NotNull] TemplateParser.ExprValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TemplateParser.exprValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprValue([NotNull] TemplateParser.ExprValueContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
