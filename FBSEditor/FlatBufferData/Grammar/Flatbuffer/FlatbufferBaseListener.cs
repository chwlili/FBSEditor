//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Flatbuffer.g4 by ANTLR 4.7.1

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
/// This class provides an empty implementation of <see cref="IFlatbufferListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class FlatbufferBaseListener : IFlatbufferListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.schema"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSchema([NotNull] FlatbufferParser.SchemaContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.schema"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSchema([NotNull] FlatbufferParser.SchemaContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.include"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInclude([NotNull] FlatbufferParser.IncludeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.include"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInclude([NotNull] FlatbufferParser.IncludeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.namespace"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNamespace([NotNull] FlatbufferParser.NamespaceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.namespace"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNamespace([NotNull] FlatbufferParser.NamespaceContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.attribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAttribute([NotNull] FlatbufferParser.AttributeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.attribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAttribute([NotNull] FlatbufferParser.AttributeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.rootType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRootType([NotNull] FlatbufferParser.RootTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.rootType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRootType([NotNull] FlatbufferParser.RootTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.fileExtension"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFileExtension([NotNull] FlatbufferParser.FileExtensionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.fileExtension"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFileExtension([NotNull] FlatbufferParser.FileExtensionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.fileIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.fileIdentifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.string"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterString([NotNull] FlatbufferParser.StringContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.string"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitString([NotNull] FlatbufferParser.StringContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.table"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTable([NotNull] FlatbufferParser.TableContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.table"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTable([NotNull] FlatbufferParser.TableContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.tableField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTableField([NotNull] FlatbufferParser.TableFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.tableField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTableField([NotNull] FlatbufferParser.TableFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.struct"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStruct([NotNull] FlatbufferParser.StructContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.struct"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStruct([NotNull] FlatbufferParser.StructContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.structField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStructField([NotNull] FlatbufferParser.StructFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.structField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStructField([NotNull] FlatbufferParser.StructFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.rpc"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRpc([NotNull] FlatbufferParser.RpcContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.rpc"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRpc([NotNull] FlatbufferParser.RpcContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.rpcField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRpcField([NotNull] FlatbufferParser.RpcFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.rpcField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRpcField([NotNull] FlatbufferParser.RpcFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.enum"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEnum([NotNull] FlatbufferParser.EnumContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.enum"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEnum([NotNull] FlatbufferParser.EnumContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.enumField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEnumField([NotNull] FlatbufferParser.EnumFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.enumField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEnumField([NotNull] FlatbufferParser.EnumFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.union"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnion([NotNull] FlatbufferParser.UnionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.union"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnion([NotNull] FlatbufferParser.UnionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.unionField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnionField([NotNull] FlatbufferParser.UnionFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.unionField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnionField([NotNull] FlatbufferParser.UnionFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.metadata"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMetadata([NotNull] FlatbufferParser.MetadataContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.metadata"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMetadata([NotNull] FlatbufferParser.MetadataContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.metadataField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMetadataField([NotNull] FlatbufferParser.MetadataFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.metadataField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMetadataField([NotNull] FlatbufferParser.MetadataFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.attr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAttr([NotNull] FlatbufferParser.AttrContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.attr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAttr([NotNull] FlatbufferParser.AttrContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.attrField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAttrField([NotNull] FlatbufferParser.AttrFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.attrField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAttrField([NotNull] FlatbufferParser.AttrFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.singleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSingleType([NotNull] FlatbufferParser.SingleTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.singleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSingleType([NotNull] FlatbufferParser.SingleTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.listType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterListType([NotNull] FlatbufferParser.ListTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.listType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitListType([NotNull] FlatbufferParser.ListTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.objectValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterObjectValue([NotNull] FlatbufferParser.ObjectValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.objectValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitObjectValue([NotNull] FlatbufferParser.ObjectValueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.objectValueField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterObjectValueField([NotNull] FlatbufferParser.ObjectValueFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.objectValueField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitObjectValueField([NotNull] FlatbufferParser.ObjectValueFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.arrayValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArrayValue([NotNull] FlatbufferParser.ArrayValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.arrayValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArrayValue([NotNull] FlatbufferParser.ArrayValueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.value"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterValue([NotNull] FlatbufferParser.ValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.value"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitValue([NotNull] FlatbufferParser.ValueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.singleValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSingleValue([NotNull] FlatbufferParser.SingleValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.singleValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSingleValue([NotNull] FlatbufferParser.SingleValueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.scalarValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterScalarValue([NotNull] FlatbufferParser.ScalarValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.scalarValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitScalarValue([NotNull] FlatbufferParser.ScalarValueContext context) { }

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
