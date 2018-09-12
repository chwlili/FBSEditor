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
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="FlatbufferParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface IFlatbufferListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.schema"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSchema([NotNull] FlatbufferParser.SchemaContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.schema"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSchema([NotNull] FlatbufferParser.SchemaContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.include"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInclude([NotNull] FlatbufferParser.IncludeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.include"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInclude([NotNull] FlatbufferParser.IncludeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.namespace"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamespace([NotNull] FlatbufferParser.NamespaceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.namespace"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamespace([NotNull] FlatbufferParser.NamespaceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttribute([NotNull] FlatbufferParser.AttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttribute([NotNull] FlatbufferParser.AttributeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.rootType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRootType([NotNull] FlatbufferParser.RootTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.rootType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRootType([NotNull] FlatbufferParser.RootTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.fileExtension"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFileExtension([NotNull] FlatbufferParser.FileExtensionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.fileExtension"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFileExtension([NotNull] FlatbufferParser.FileExtensionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.fileIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.fileIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFileIdentifier([NotNull] FlatbufferParser.FileIdentifierContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterString([NotNull] FlatbufferParser.StringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitString([NotNull] FlatbufferParser.StringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.metas"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMetas([NotNull] FlatbufferParser.MetasContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.metas"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMetas([NotNull] FlatbufferParser.MetasContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.bindMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBindMeta([NotNull] FlatbufferParser.BindMetaContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.bindMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBindMeta([NotNull] FlatbufferParser.BindMetaContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.indexMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIndexMeta([NotNull] FlatbufferParser.IndexMetaContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.indexMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIndexMeta([NotNull] FlatbufferParser.IndexMetaContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.nullableMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNullableMeta([NotNull] FlatbufferParser.NullableMetaContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.nullableMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNullableMeta([NotNull] FlatbufferParser.NullableMetaContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.referenceMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReferenceMeta([NotNull] FlatbufferParser.ReferenceMetaContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.referenceMeta"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReferenceMeta([NotNull] FlatbufferParser.ReferenceMetaContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.table"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTable([NotNull] FlatbufferParser.TableContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.table"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTable([NotNull] FlatbufferParser.TableContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.tableField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTableField([NotNull] FlatbufferParser.TableFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.tableField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTableField([NotNull] FlatbufferParser.TableFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.struct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStruct([NotNull] FlatbufferParser.StructContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.struct"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStruct([NotNull] FlatbufferParser.StructContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.structField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStructField([NotNull] FlatbufferParser.StructFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.structField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStructField([NotNull] FlatbufferParser.StructFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.rpc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRpc([NotNull] FlatbufferParser.RpcContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.rpc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRpc([NotNull] FlatbufferParser.RpcContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.rpcField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRpcField([NotNull] FlatbufferParser.RpcFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.rpcField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRpcField([NotNull] FlatbufferParser.RpcFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.enum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEnum([NotNull] FlatbufferParser.EnumContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.enum"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEnum([NotNull] FlatbufferParser.EnumContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.enumField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEnumField([NotNull] FlatbufferParser.EnumFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.enumField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEnumField([NotNull] FlatbufferParser.EnumFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.union"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnion([NotNull] FlatbufferParser.UnionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.union"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnion([NotNull] FlatbufferParser.UnionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.unionField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnionField([NotNull] FlatbufferParser.UnionFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.unionField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnionField([NotNull] FlatbufferParser.UnionFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMetadata([NotNull] FlatbufferParser.MetadataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMetadata([NotNull] FlatbufferParser.MetadataContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.metadataField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMetadataField([NotNull] FlatbufferParser.MetadataFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.metadataField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMetadataField([NotNull] FlatbufferParser.MetadataFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.attr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttr([NotNull] FlatbufferParser.AttrContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.attr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttr([NotNull] FlatbufferParser.AttrContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.attrField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttrField([NotNull] FlatbufferParser.AttrFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.attrField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttrField([NotNull] FlatbufferParser.AttrFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.singleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSingleType([NotNull] FlatbufferParser.SingleTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.singleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSingleType([NotNull] FlatbufferParser.SingleTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.listType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterListType([NotNull] FlatbufferParser.ListTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.listType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitListType([NotNull] FlatbufferParser.ListTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.objectValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterObjectValue([NotNull] FlatbufferParser.ObjectValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.objectValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitObjectValue([NotNull] FlatbufferParser.ObjectValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.objectValueField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterObjectValueField([NotNull] FlatbufferParser.ObjectValueFieldContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.objectValueField"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitObjectValueField([NotNull] FlatbufferParser.ObjectValueFieldContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.arrayValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrayValue([NotNull] FlatbufferParser.ArrayValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.arrayValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrayValue([NotNull] FlatbufferParser.ArrayValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValue([NotNull] FlatbufferParser.ValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValue([NotNull] FlatbufferParser.ValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.singleValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSingleValue([NotNull] FlatbufferParser.SingleValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.singleValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSingleValue([NotNull] FlatbufferParser.SingleValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="FlatbufferParser.scalarValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterScalarValue([NotNull] FlatbufferParser.ScalarValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="FlatbufferParser.scalarValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitScalarValue([NotNull] FlatbufferParser.ScalarValueContext context);
}
