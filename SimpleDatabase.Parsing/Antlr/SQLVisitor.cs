//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\will\Source\Repos\SimpleDatabase\SimpleDatabase.Parsing\Antlr\SQL.g4 by ANTLR 4.7

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace SimpleDatabase.Parsing.Antlr {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="SQLParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public interface ISQLVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProgram([NotNull] SQLParser.ProgramContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] SQLParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.statement_select"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_select([NotNull] SQLParser.Statement_selectContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.statement_insert"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_insert([NotNull] SQLParser.Statement_insertContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.statement_insert_value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement_insert_value([NotNull] SQLParser.Statement_insert_valueContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>result_column_star</c>
	/// labeled alternative in <see cref="SQLParser.result_column"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitResult_column_star([NotNull] SQLParser.Result_column_starContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>result_column_expr</c>
	/// labeled alternative in <see cref="SQLParser.result_column"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitResult_column_expr([NotNull] SQLParser.Result_column_exprContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.column_alias"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitColumn_alias([NotNull] SQLParser.Column_aliasContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.column_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitColumn_name([NotNull] SQLParser.Column_nameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.table_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTable_name([NotNull] SQLParser.Table_nameContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expression_column</c>
	/// labeled alternative in <see cref="SQLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_column([NotNull] SQLParser.Expression_columnContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expression_equality</c>
	/// labeled alternative in <see cref="SQLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_equality([NotNull] SQLParser.Expression_equalityContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>expression_literal</c>
	/// labeled alternative in <see cref="SQLParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression_literal([NotNull] SQLParser.Expression_literalContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="SQLParser.literal_value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteral_value([NotNull] SQLParser.Literal_valueContext context);
}
} // namespace SimpleDatabase.Parsing.Antlr
