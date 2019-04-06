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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="FlatbufferParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface IFlatbufferVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.schema"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSchema([NotNull] FlatbufferParser.SchemaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.include"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInclude([NotNull] FlatbufferParser.IncludeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.namespace"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNamespace([NotNull] FlatbufferParser.NamespaceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAttribute([NotNull] FlatbufferParser.AttributeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.rootType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRootType([NotNull] FlatbufferParser.RootTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.fileExtension"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFileExtension([NotNull] FlatbufferParser.FileExtensionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.fileIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitString([NotNull] FlatbufferParser.StringContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.table"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTable([NotNull] FlatbufferParser.TableContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.tableField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTableField([NotNull] FlatbufferParser.TableFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.struct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStruct([NotNull] FlatbufferParser.StructContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.structField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStructField([NotNull] FlatbufferParser.StructFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.rpc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRpc([NotNull] FlatbufferParser.RpcContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.rpcField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRpcField([NotNull] FlatbufferParser.RpcFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.enum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnum([NotNull] FlatbufferParser.EnumContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.enumField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnumField([NotNull] FlatbufferParser.EnumFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.union"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnion([NotNull] FlatbufferParser.UnionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.unionField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUnionField([NotNull] FlatbufferParser.UnionFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMetadata([NotNull] FlatbufferParser.MetadataContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.metadataField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMetadataField([NotNull] FlatbufferParser.MetadataFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.attr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAttr([NotNull] FlatbufferParser.AttrContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.attrField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAttrField([NotNull] FlatbufferParser.AttrFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.attrFieldValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAttrFieldValue([NotNull] FlatbufferParser.AttrFieldValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.singleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSingleType([NotNull] FlatbufferParser.SingleTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.listType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitListType([NotNull] FlatbufferParser.ListTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.objectValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitObjectValue([NotNull] FlatbufferParser.ObjectValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.objectValueField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitObjectValueField([NotNull] FlatbufferParser.ObjectValueFieldContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.arrayValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrayValue([NotNull] FlatbufferParser.ArrayValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitValue([NotNull] FlatbufferParser.ValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.singleValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSingleValue([NotNull] FlatbufferParser.SingleValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="FlatbufferParser.scalarValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitScalarValue([NotNull] FlatbufferParser.ScalarValueContext context);
}
