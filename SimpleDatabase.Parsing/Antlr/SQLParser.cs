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

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public partial class SQLParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		K_AS=1, K_EXPLAIN=2, K_FROM=3, K_SELECT=4, K_WHERE=5, IDENTIFIER=6, NUMBER_LITERAL=7, 
		STRING_LITERAL=8, COMMA=9, DOT=10, EQUAL=11, NOT=12, SEMI=13, STAR=14, 
		WS=15;
	public const int
		RULE_program = 0, RULE_statement = 1, RULE_statement_select = 2, RULE_result_column = 3, 
		RULE_column_alias = 4, RULE_column_name = 5, RULE_table_name = 6, RULE_expression = 7, 
		RULE_literal_value = 8;
	public static readonly string[] ruleNames = {
		"program", "statement", "statement_select", "result_column", "column_alias", 
		"column_name", "table_name", "expression", "literal_value"
	};

	private static readonly string[] _LiteralNames = {
		null, null, null, null, null, null, null, null, null, "','", "'.'", "'='", 
		"'!'", "';'", "'*'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "K_AS", "K_EXPLAIN", "K_FROM", "K_SELECT", "K_WHERE", "IDENTIFIER", 
		"NUMBER_LITERAL", "STRING_LITERAL", "COMMA", "DOT", "EQUAL", "NOT", "SEMI", 
		"STAR", "WS"
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

	public override string GrammarFileName { get { return "SQL.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static SQLParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public SQLParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public SQLParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}
	public partial class ProgramContext : ParserRuleContext {
		public StatementContext[] statement() {
			return GetRuleContexts<StatementContext>();
		}
		public StatementContext statement(int i) {
			return GetRuleContext<StatementContext>(i);
		}
		public ITerminalNode[] SEMI() { return GetTokens(SQLParser.SEMI); }
		public ITerminalNode SEMI(int i) {
			return GetToken(SQLParser.SEMI, i);
		}
		public ProgramContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_program; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitProgram(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ProgramContext program() {
		ProgramContext _localctx = new ProgramContext(Context, State);
		EnterRule(_localctx, 0, RULE_program);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 18; statement();
			State = 23;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,0,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					State = 19; Match(SEMI);
					State = 20; statement();
					}
					} 
				}
				State = 25;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,0,Context);
			}
			State = 27;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==SEMI) {
				{
				State = 26; Match(SEMI);
				}
			}

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

	public partial class StatementContext : ParserRuleContext {
		public Statement_selectContext statement_select() {
			return GetRuleContext<Statement_selectContext>(0);
		}
		public ITerminalNode K_EXPLAIN() { return GetToken(SQLParser.K_EXPLAIN, 0); }
		public StatementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_statement; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitStatement(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public StatementContext statement() {
		StatementContext _localctx = new StatementContext(Context, State);
		EnterRule(_localctx, 2, RULE_statement);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 30;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==K_EXPLAIN) {
				{
				State = 29; Match(K_EXPLAIN);
				}
			}

			{
			State = 32; statement_select();
			}
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

	public partial class Statement_selectContext : ParserRuleContext {
		public Result_columnContext _result_column;
		public IList<Result_columnContext> _Columns = new List<Result_columnContext>();
		public Table_nameContext Table;
		public ExpressionContext Where;
		public ITerminalNode K_SELECT() { return GetToken(SQLParser.K_SELECT, 0); }
		public ITerminalNode K_FROM() { return GetToken(SQLParser.K_FROM, 0); }
		public Result_columnContext[] result_column() {
			return GetRuleContexts<Result_columnContext>();
		}
		public Result_columnContext result_column(int i) {
			return GetRuleContext<Result_columnContext>(i);
		}
		public Table_nameContext table_name() {
			return GetRuleContext<Table_nameContext>(0);
		}
		public ITerminalNode[] COMMA() { return GetTokens(SQLParser.COMMA); }
		public ITerminalNode COMMA(int i) {
			return GetToken(SQLParser.COMMA, i);
		}
		public ITerminalNode K_WHERE() { return GetToken(SQLParser.K_WHERE, 0); }
		public ExpressionContext expression() {
			return GetRuleContext<ExpressionContext>(0);
		}
		public Statement_selectContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_statement_select; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitStatement_select(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Statement_selectContext statement_select() {
		Statement_selectContext _localctx = new Statement_selectContext(Context, State);
		EnterRule(_localctx, 4, RULE_statement_select);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 34; Match(K_SELECT);
			State = 35; _localctx._result_column = result_column();
			_localctx._Columns.Add(_localctx._result_column);
			State = 40;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==COMMA) {
				{
				{
				State = 36; Match(COMMA);
				State = 37; _localctx._result_column = result_column();
				_localctx._Columns.Add(_localctx._result_column);
				}
				}
				State = 42;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 43; Match(K_FROM);
			State = 44; _localctx.Table = table_name();
			State = 47;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==K_WHERE) {
				{
				State = 45; Match(K_WHERE);
				State = 46; _localctx.Where = expression(0);
				}
			}

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

	public partial class Result_columnContext : ParserRuleContext {
		public Result_columnContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_result_column; } }
	 
		public Result_columnContext() { }
		public virtual void CopyFrom(Result_columnContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class Result_column_exprContext : Result_columnContext {
		public ExpressionContext expression() {
			return GetRuleContext<ExpressionContext>(0);
		}
		public Column_aliasContext column_alias() {
			return GetRuleContext<Column_aliasContext>(0);
		}
		public ITerminalNode K_AS() { return GetToken(SQLParser.K_AS, 0); }
		public Result_column_exprContext(Result_columnContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitResult_column_expr(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class Result_column_starContext : Result_columnContext {
		public ITerminalNode STAR() { return GetToken(SQLParser.STAR, 0); }
		public Table_nameContext table_name() {
			return GetRuleContext<Table_nameContext>(0);
		}
		public ITerminalNode DOT() { return GetToken(SQLParser.DOT, 0); }
		public Result_column_starContext(Result_columnContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitResult_column_star(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Result_columnContext result_column() {
		Result_columnContext _localctx = new Result_columnContext(Context, State);
		EnterRule(_localctx, 6, RULE_result_column);
		int _la;
		try {
			State = 62;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,8,Context) ) {
			case 1:
				_localctx = new Result_column_starContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 52;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==IDENTIFIER) {
					{
					State = 49; table_name();
					State = 50; Match(DOT);
					}
				}

				State = 54; Match(STAR);
				}
				break;
			case 2:
				_localctx = new Result_column_exprContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 55; expression(0);
				State = 60;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==K_AS || _la==IDENTIFIER) {
					{
					State = 57;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
					if (_la==K_AS) {
						{
						State = 56; Match(K_AS);
						}
					}

					State = 59; column_alias();
					}
				}

				}
				break;
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

	public partial class Column_aliasContext : ParserRuleContext {
		public ITerminalNode IDENTIFIER() { return GetToken(SQLParser.IDENTIFIER, 0); }
		public Column_aliasContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_column_alias; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitColumn_alias(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Column_aliasContext column_alias() {
		Column_aliasContext _localctx = new Column_aliasContext(Context, State);
		EnterRule(_localctx, 8, RULE_column_alias);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 64; Match(IDENTIFIER);
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

	public partial class Column_nameContext : ParserRuleContext {
		public ITerminalNode IDENTIFIER() { return GetToken(SQLParser.IDENTIFIER, 0); }
		public Column_nameContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_column_name; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitColumn_name(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Column_nameContext column_name() {
		Column_nameContext _localctx = new Column_nameContext(Context, State);
		EnterRule(_localctx, 10, RULE_column_name);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 66; Match(IDENTIFIER);
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

	public partial class Table_nameContext : ParserRuleContext {
		public ITerminalNode IDENTIFIER() { return GetToken(SQLParser.IDENTIFIER, 0); }
		public Table_nameContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_table_name; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitTable_name(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Table_nameContext table_name() {
		Table_nameContext _localctx = new Table_nameContext(Context, State);
		EnterRule(_localctx, 12, RULE_table_name);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 68; Match(IDENTIFIER);
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

	public partial class ExpressionContext : ParserRuleContext {
		public ExpressionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_expression; } }
	 
		public ExpressionContext() { }
		public virtual void CopyFrom(ExpressionContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class Expression_columnContext : ExpressionContext {
		public Column_nameContext column_name() {
			return GetRuleContext<Column_nameContext>(0);
		}
		public Table_nameContext table_name() {
			return GetRuleContext<Table_nameContext>(0);
		}
		public ITerminalNode DOT() { return GetToken(SQLParser.DOT, 0); }
		public Expression_columnContext(ExpressionContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitExpression_column(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class Expression_equalityContext : ExpressionContext {
		public ExpressionContext[] expression() {
			return GetRuleContexts<ExpressionContext>();
		}
		public ExpressionContext expression(int i) {
			return GetRuleContext<ExpressionContext>(i);
		}
		public ITerminalNode[] EQUAL() { return GetTokens(SQLParser.EQUAL); }
		public ITerminalNode EQUAL(int i) {
			return GetToken(SQLParser.EQUAL, i);
		}
		public ITerminalNode NOT() { return GetToken(SQLParser.NOT, 0); }
		public Expression_equalityContext(ExpressionContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitExpression_equality(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class Expression_literalContext : ExpressionContext {
		public Literal_valueContext literal_value() {
			return GetRuleContext<Literal_valueContext>(0);
		}
		public Expression_literalContext(ExpressionContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitExpression_literal(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ExpressionContext expression() {
		return expression(0);
	}

	private ExpressionContext expression(int _p) {
		ParserRuleContext _parentctx = Context;
		int _parentState = State;
		ExpressionContext _localctx = new ExpressionContext(Context, _parentState);
		ExpressionContext _prevctx = _localctx;
		int _startState = 14;
		EnterRecursionRule(_localctx, 14, RULE_expression, _p);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 78;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case NUMBER_LITERAL:
			case STRING_LITERAL:
				{
				_localctx = new Expression_literalContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;

				State = 71; literal_value();
				}
				break;
			case IDENTIFIER:
				{
				_localctx = new Expression_columnContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;
				State = 75;
				ErrorHandler.Sync(this);
				switch ( Interpreter.AdaptivePredict(TokenStream,9,Context) ) {
				case 1:
					{
					State = 72; table_name();
					State = 73; Match(DOT);
					}
					break;
				}
				State = 77; column_name();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			Context.Stop = TokenStream.LT(-1);
			State = 92;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,13,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( ParseListeners!=null )
						TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					{
					_localctx = new Expression_equalityContext(new ExpressionContext(_parentctx, _parentState));
					PushNewRecursionContext(_localctx, _startState, RULE_expression);
					State = 80;
					if (!(Precpred(Context, 1))) throw new FailedPredicateException(this, "Precpred(Context, 1)");
					State = 87;
					ErrorHandler.Sync(this);
					switch (TokenStream.LA(1)) {
					case EQUAL:
						{
						State = 81; Match(EQUAL);
						State = 83;
						ErrorHandler.Sync(this);
						_la = TokenStream.LA(1);
						if (_la==EQUAL) {
							{
							State = 82; Match(EQUAL);
							}
						}

						}
						break;
					case NOT:
						{
						State = 85; Match(NOT);
						State = 86; Match(EQUAL);
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					State = 89; expression(2);
					}
					} 
				}
				State = 94;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,13,Context);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public partial class Literal_valueContext : ParserRuleContext {
		public ITerminalNode NUMBER_LITERAL() { return GetToken(SQLParser.NUMBER_LITERAL, 0); }
		public ITerminalNode STRING_LITERAL() { return GetToken(SQLParser.STRING_LITERAL, 0); }
		public Literal_valueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_literal_value; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISQLVisitor<TResult> typedVisitor = visitor as ISQLVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitLiteral_value(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Literal_valueContext literal_value() {
		Literal_valueContext _localctx = new Literal_valueContext(Context, State);
		EnterRule(_localctx, 16, RULE_literal_value);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 95;
			_la = TokenStream.LA(1);
			if ( !(_la==NUMBER_LITERAL || _la==STRING_LITERAL) ) {
			ErrorHandler.RecoverInline(this);
			}
			else {
				ErrorHandler.ReportMatch(this);
			    Consume();
			}
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

	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 7: return expression_sempred((ExpressionContext)_localctx, predIndex);
		}
		return true;
	}
	private bool expression_sempred(ExpressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return Precpred(Context, 1);
		}
		return true;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\x11', '\x64', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x4', 
		'\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t', '\b', 
		'\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\a', '\x2', '\x18', '\n', '\x2', '\f', '\x2', '\xE', 
		'\x2', '\x1B', '\v', '\x2', '\x3', '\x2', '\x5', '\x2', '\x1E', '\n', 
		'\x2', '\x3', '\x3', '\x5', '\x3', '!', '\n', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\a', '\x4', 
		')', '\n', '\x4', '\f', '\x4', '\xE', '\x4', ',', '\v', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x5', '\x4', '\x32', 
		'\n', '\x4', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x5', '\x5', '\x37', 
		'\n', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x5', '\x5', '<', 
		'\n', '\x5', '\x3', '\x5', '\x5', '\x5', '?', '\n', '\x5', '\x5', '\x5', 
		'\x41', '\n', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', 
		'\x3', '\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', 
		'\t', '\x3', '\t', '\x5', '\t', 'N', '\n', '\t', '\x3', '\t', '\x5', '\t', 
		'Q', '\n', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x5', '\t', 'V', 
		'\n', '\t', '\x3', '\t', '\x3', '\t', '\x5', '\t', 'Z', '\n', '\t', '\x3', 
		'\t', '\a', '\t', ']', '\n', '\t', '\f', '\t', '\xE', '\t', '`', '\v', 
		'\t', '\x3', '\n', '\x3', '\n', '\x3', '\n', '\x2', '\x3', '\x10', '\v', 
		'\x2', '\x4', '\x6', '\b', '\n', '\f', '\xE', '\x10', '\x12', '\x2', '\x3', 
		'\x3', '\x2', '\t', '\n', '\x2', 'h', '\x2', '\x14', '\x3', '\x2', '\x2', 
		'\x2', '\x4', ' ', '\x3', '\x2', '\x2', '\x2', '\x6', '$', '\x3', '\x2', 
		'\x2', '\x2', '\b', '@', '\x3', '\x2', '\x2', '\x2', '\n', '\x42', '\x3', 
		'\x2', '\x2', '\x2', '\f', '\x44', '\x3', '\x2', '\x2', '\x2', '\xE', 
		'\x46', '\x3', '\x2', '\x2', '\x2', '\x10', 'P', '\x3', '\x2', '\x2', 
		'\x2', '\x12', '\x61', '\x3', '\x2', '\x2', '\x2', '\x14', '\x19', '\x5', 
		'\x4', '\x3', '\x2', '\x15', '\x16', '\a', '\xF', '\x2', '\x2', '\x16', 
		'\x18', '\x5', '\x4', '\x3', '\x2', '\x17', '\x15', '\x3', '\x2', '\x2', 
		'\x2', '\x18', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x19', '\x17', '\x3', 
		'\x2', '\x2', '\x2', '\x19', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x1A', 
		'\x1D', '\x3', '\x2', '\x2', '\x2', '\x1B', '\x19', '\x3', '\x2', '\x2', 
		'\x2', '\x1C', '\x1E', '\a', '\xF', '\x2', '\x2', '\x1D', '\x1C', '\x3', 
		'\x2', '\x2', '\x2', '\x1D', '\x1E', '\x3', '\x2', '\x2', '\x2', '\x1E', 
		'\x3', '\x3', '\x2', '\x2', '\x2', '\x1F', '!', '\a', '\x4', '\x2', '\x2', 
		' ', '\x1F', '\x3', '\x2', '\x2', '\x2', ' ', '!', '\x3', '\x2', '\x2', 
		'\x2', '!', '\"', '\x3', '\x2', '\x2', '\x2', '\"', '#', '\x5', '\x6', 
		'\x4', '\x2', '#', '\x5', '\x3', '\x2', '\x2', '\x2', '$', '%', '\a', 
		'\x6', '\x2', '\x2', '%', '*', '\x5', '\b', '\x5', '\x2', '&', '\'', '\a', 
		'\v', '\x2', '\x2', '\'', ')', '\x5', '\b', '\x5', '\x2', '(', '&', '\x3', 
		'\x2', '\x2', '\x2', ')', ',', '\x3', '\x2', '\x2', '\x2', '*', '(', '\x3', 
		'\x2', '\x2', '\x2', '*', '+', '\x3', '\x2', '\x2', '\x2', '+', '-', '\x3', 
		'\x2', '\x2', '\x2', ',', '*', '\x3', '\x2', '\x2', '\x2', '-', '.', '\a', 
		'\x5', '\x2', '\x2', '.', '\x31', '\x5', '\xE', '\b', '\x2', '/', '\x30', 
		'\a', '\a', '\x2', '\x2', '\x30', '\x32', '\x5', '\x10', '\t', '\x2', 
		'\x31', '/', '\x3', '\x2', '\x2', '\x2', '\x31', '\x32', '\x3', '\x2', 
		'\x2', '\x2', '\x32', '\a', '\x3', '\x2', '\x2', '\x2', '\x33', '\x34', 
		'\x5', '\xE', '\b', '\x2', '\x34', '\x35', '\a', '\f', '\x2', '\x2', '\x35', 
		'\x37', '\x3', '\x2', '\x2', '\x2', '\x36', '\x33', '\x3', '\x2', '\x2', 
		'\x2', '\x36', '\x37', '\x3', '\x2', '\x2', '\x2', '\x37', '\x38', '\x3', 
		'\x2', '\x2', '\x2', '\x38', '\x41', '\a', '\x10', '\x2', '\x2', '\x39', 
		'>', '\x5', '\x10', '\t', '\x2', ':', '<', '\a', '\x3', '\x2', '\x2', 
		';', ':', '\x3', '\x2', '\x2', '\x2', ';', '<', '\x3', '\x2', '\x2', '\x2', 
		'<', '=', '\x3', '\x2', '\x2', '\x2', '=', '?', '\x5', '\n', '\x6', '\x2', 
		'>', ';', '\x3', '\x2', '\x2', '\x2', '>', '?', '\x3', '\x2', '\x2', '\x2', 
		'?', '\x41', '\x3', '\x2', '\x2', '\x2', '@', '\x36', '\x3', '\x2', '\x2', 
		'\x2', '@', '\x39', '\x3', '\x2', '\x2', '\x2', '\x41', '\t', '\x3', '\x2', 
		'\x2', '\x2', '\x42', '\x43', '\a', '\b', '\x2', '\x2', '\x43', '\v', 
		'\x3', '\x2', '\x2', '\x2', '\x44', '\x45', '\a', '\b', '\x2', '\x2', 
		'\x45', '\r', '\x3', '\x2', '\x2', '\x2', '\x46', 'G', '\a', '\b', '\x2', 
		'\x2', 'G', '\xF', '\x3', '\x2', '\x2', '\x2', 'H', 'I', '\b', '\t', '\x1', 
		'\x2', 'I', 'Q', '\x5', '\x12', '\n', '\x2', 'J', 'K', '\x5', '\xE', '\b', 
		'\x2', 'K', 'L', '\a', '\f', '\x2', '\x2', 'L', 'N', '\x3', '\x2', '\x2', 
		'\x2', 'M', 'J', '\x3', '\x2', '\x2', '\x2', 'M', 'N', '\x3', '\x2', '\x2', 
		'\x2', 'N', 'O', '\x3', '\x2', '\x2', '\x2', 'O', 'Q', '\x5', '\f', '\a', 
		'\x2', 'P', 'H', '\x3', '\x2', '\x2', '\x2', 'P', 'M', '\x3', '\x2', '\x2', 
		'\x2', 'Q', '^', '\x3', '\x2', '\x2', '\x2', 'R', 'Y', '\f', '\x3', '\x2', 
		'\x2', 'S', 'U', '\a', '\r', '\x2', '\x2', 'T', 'V', '\a', '\r', '\x2', 
		'\x2', 'U', 'T', '\x3', '\x2', '\x2', '\x2', 'U', 'V', '\x3', '\x2', '\x2', 
		'\x2', 'V', 'Z', '\x3', '\x2', '\x2', '\x2', 'W', 'X', '\a', '\xE', '\x2', 
		'\x2', 'X', 'Z', '\a', '\r', '\x2', '\x2', 'Y', 'S', '\x3', '\x2', '\x2', 
		'\x2', 'Y', 'W', '\x3', '\x2', '\x2', '\x2', 'Z', '[', '\x3', '\x2', '\x2', 
		'\x2', '[', ']', '\x5', '\x10', '\t', '\x4', '\\', 'R', '\x3', '\x2', 
		'\x2', '\x2', ']', '`', '\x3', '\x2', '\x2', '\x2', '^', '\\', '\x3', 
		'\x2', '\x2', '\x2', '^', '_', '\x3', '\x2', '\x2', '\x2', '_', '\x11', 
		'\x3', '\x2', '\x2', '\x2', '`', '^', '\x3', '\x2', '\x2', '\x2', '\x61', 
		'\x62', '\t', '\x2', '\x2', '\x2', '\x62', '\x13', '\x3', '\x2', '\x2', 
		'\x2', '\x10', '\x19', '\x1D', ' ', '*', '\x31', '\x36', ';', '>', '@', 
		'M', 'P', 'U', 'Y', '^',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace SimpleDatabase.Parsing.Antlr
