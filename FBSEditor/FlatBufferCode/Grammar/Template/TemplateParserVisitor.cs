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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="TemplateParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface ITemplateParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.document"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDocument([NotNull] TemplateParser.DocumentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.code"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCode([NotNull] TemplateParser.CodeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.text"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitText([NotNull] TemplateParser.TextContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.break"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBreak([NotNull] TemplateParser.BreakContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.continue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitContinue([NotNull] TemplateParser.ContinueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.return"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReturn([NotNull] TemplateParser.ReturnContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.var"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVar([NotNull] TemplateParser.VarContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.if"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIf([NotNull] TemplateParser.IfContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.switch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSwitch([NotNull] TemplateParser.SwitchContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.while"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhile([NotNull] TemplateParser.WhileContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.dowhile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDowhile([NotNull] TemplateParser.DowhileContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.for"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFor([NotNull] TemplateParser.ForContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.foreach"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForeach([NotNull] TemplateParser.ForeachContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpr([NotNull] TemplateParser.ExprContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.exprCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExprCall([NotNull] TemplateParser.ExprCallContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.exprProp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExprProp([NotNull] TemplateParser.ExprPropContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="TemplateParser.exprValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExprValue([NotNull] TemplateParser.ExprValueContext context);
}