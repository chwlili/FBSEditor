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

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="CsvParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface ICsvListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="CsvParser.csvTab"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCsvTab([NotNull] CsvParser.CsvTabContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CsvParser.csvTab"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCsvTab([NotNull] CsvParser.CsvTabContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CsvParser.csvRow"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCsvRow([NotNull] CsvParser.CsvRowContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CsvParser.csvRow"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCsvRow([NotNull] CsvParser.CsvRowContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CsvParser.csvCol"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCsvCol([NotNull] CsvParser.CsvColContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CsvParser.csvCol"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCsvCol([NotNull] CsvParser.CsvColContext context);
}
