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
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
[System.CLSCompliant(false)]
public partial class SQLLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		K_AS=10, K_ASC=11, K_BY=12, K_DELETE=13, K_DESC=14, K_EXPLAIN=15, K_FROM=16, 
		K_INSERT=17, K_INTO=18, K_ORDER=19, K_SELECT=20, K_VALUES=21, K_WHERE=22, 
		IDENTIFIER=23, NUMBER_LITERAL=24, STRING_LITERAL=25, WS=26;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"K_AS", "K_ASC", "K_BY", "K_DELETE", "K_DESC", "K_EXPLAIN", "K_FROM", 
		"K_INSERT", "K_INTO", "K_ORDER", "K_SELECT", "K_VALUES", "K_WHERE", "IDENTIFIER", 
		"NUMBER_LITERAL", "STRING_LITERAL", "WS", "A", "B", "C", "D", "E", "F", 
		"G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", 
		"U", "V", "W", "X", "Y", "Z"
	};


	public SQLLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public SQLLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "';'", "','", "'('", "')'", "'.'", "'*'", "'='", "'=='", "'!='"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, "K_AS", "K_ASC", 
		"K_BY", "K_DELETE", "K_DESC", "K_EXPLAIN", "K_FROM", "K_INSERT", "K_INTO", 
		"K_ORDER", "K_SELECT", "K_VALUES", "K_WHERE", "IDENTIFIER", "NUMBER_LITERAL", 
		"STRING_LITERAL", "WS"
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

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static SQLLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x1C', '\x115', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', '\x10', '\x4', 
		'\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', '\x13', '\t', 
		'\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', '\x15', '\x4', 
		'\x16', '\t', '\x16', '\x4', '\x17', '\t', '\x17', '\x4', '\x18', '\t', 
		'\x18', '\x4', '\x19', '\t', '\x19', '\x4', '\x1A', '\t', '\x1A', '\x4', 
		'\x1B', '\t', '\x1B', '\x4', '\x1C', '\t', '\x1C', '\x4', '\x1D', '\t', 
		'\x1D', '\x4', '\x1E', '\t', '\x1E', '\x4', '\x1F', '\t', '\x1F', '\x4', 
		' ', '\t', ' ', '\x4', '!', '\t', '!', '\x4', '\"', '\t', '\"', '\x4', 
		'#', '\t', '#', '\x4', '$', '\t', '$', '\x4', '%', '\t', '%', '\x4', '&', 
		'\t', '&', '\x4', '\'', '\t', '\'', '\x4', '(', '\t', '(', '\x4', ')', 
		'\t', ')', '\x4', '*', '\t', '*', '\x4', '+', '\t', '+', '\x4', ',', '\t', 
		',', '\x4', '-', '\t', '-', '\x4', '.', '\t', '.', '\x4', '/', '\t', '/', 
		'\x4', '\x30', '\t', '\x30', '\x4', '\x31', '\t', '\x31', '\x4', '\x32', 
		'\t', '\x32', '\x4', '\x33', '\t', '\x33', '\x4', '\x34', '\t', '\x34', 
		'\x4', '\x35', '\t', '\x35', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', '\x5', 
		'\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', '\b', '\x3', 
		'\b', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', 
		'\x3', '\n', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\f', '\x3', 
		'\f', '\x3', '\f', '\x3', '\f', '\x3', '\r', '\x3', '\r', '\x3', '\r', 
		'\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\x3', '\xE', 
		'\x3', '\xE', '\x3', '\xE', '\x3', '\xF', '\x3', '\xF', '\x3', '\xF', 
		'\x3', '\xF', '\x3', '\xF', '\x3', '\x10', '\x3', '\x10', '\x3', '\x10', 
		'\x3', '\x10', '\x3', '\x10', '\x3', '\x10', '\x3', '\x10', '\x3', '\x10', 
		'\x3', '\x11', '\x3', '\x11', '\x3', '\x11', '\x3', '\x11', '\x3', '\x11', 
		'\x3', '\x12', '\x3', '\x12', '\x3', '\x12', '\x3', '\x12', '\x3', '\x12', 
		'\x3', '\x12', '\x3', '\x12', '\x3', '\x13', '\x3', '\x13', '\x3', '\x13', 
		'\x3', '\x13', '\x3', '\x13', '\x3', '\x14', '\x3', '\x14', '\x3', '\x14', 
		'\x3', '\x14', '\x3', '\x14', '\x3', '\x14', '\x3', '\x15', '\x3', '\x15', 
		'\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', 
		'\x3', '\x16', '\x3', '\x16', '\x3', '\x16', '\x3', '\x16', '\x3', '\x16', 
		'\x3', '\x16', '\x3', '\x16', '\x3', '\x17', '\x3', '\x17', '\x3', '\x17', 
		'\x3', '\x17', '\x3', '\x17', '\x3', '\x17', '\x3', '\x18', '\x3', '\x18', 
		'\a', '\x18', '\xCB', '\n', '\x18', '\f', '\x18', '\xE', '\x18', '\xCE', 
		'\v', '\x18', '\x3', '\x19', '\x6', '\x19', '\xD1', '\n', '\x19', '\r', 
		'\x19', '\xE', '\x19', '\xD2', '\x3', '\x1A', '\x3', '\x1A', '\a', '\x1A', 
		'\xD7', '\n', '\x1A', '\f', '\x1A', '\xE', '\x1A', '\xDA', '\v', '\x1A', 
		'\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1B', '\x3', '\x1B', '\x3', '\x1B', 
		'\x3', '\x1B', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1D', '\x3', '\x1D', 
		'\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1F', '\x3', '\x1F', '\x3', ' ', 
		'\x3', ' ', '\x3', '!', '\x3', '!', '\x3', '\"', '\x3', '\"', '\x3', '#', 
		'\x3', '#', '\x3', '$', '\x3', '$', '\x3', '%', '\x3', '%', '\x3', '&', 
		'\x3', '&', '\x3', '\'', '\x3', '\'', '\x3', '(', '\x3', '(', '\x3', ')', 
		'\x3', ')', '\x3', '*', '\x3', '*', '\x3', '+', '\x3', '+', '\x3', ',', 
		'\x3', ',', '\x3', '-', '\x3', '-', '\x3', '.', '\x3', '.', '\x3', '/', 
		'\x3', '/', '\x3', '\x30', '\x3', '\x30', '\x3', '\x31', '\x3', '\x31', 
		'\x3', '\x32', '\x3', '\x32', '\x3', '\x33', '\x3', '\x33', '\x3', '\x34', 
		'\x3', '\x34', '\x3', '\x35', '\x3', '\x35', '\x2', '\x2', '\x36', '\x3', 
		'\x3', '\x5', '\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', 
		'\xF', '\t', '\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', 
		'\xE', '\x1B', '\xF', '\x1D', '\x10', '\x1F', '\x11', '!', '\x12', '#', 
		'\x13', '%', '\x14', '\'', '\x15', ')', '\x16', '+', '\x17', '-', '\x18', 
		'/', '\x19', '\x31', '\x1A', '\x33', '\x1B', '\x35', '\x1C', '\x37', '\x2', 
		'\x39', '\x2', ';', '\x2', '=', '\x2', '?', '\x2', '\x41', '\x2', '\x43', 
		'\x2', '\x45', '\x2', 'G', '\x2', 'I', '\x2', 'K', '\x2', 'M', '\x2', 
		'O', '\x2', 'Q', '\x2', 'S', '\x2', 'U', '\x2', 'W', '\x2', 'Y', '\x2', 
		'[', '\x2', ']', '\x2', '_', '\x2', '\x61', '\x2', '\x63', '\x2', '\x65', 
		'\x2', 'g', '\x2', 'i', '\x2', '\x3', '\x2', '!', '\x5', '\x2', '\x43', 
		'\\', '\x61', '\x61', '\x63', '|', '\x6', '\x2', '\x32', ';', '\x43', 
		'\\', '\x61', '\x61', '\x63', '|', '\x3', '\x2', '\x32', ';', '\x3', '\x2', 
		')', ')', '\x5', '\x2', '\v', '\f', '\xF', '\xF', '\"', '\"', '\x4', '\x2', 
		'\x43', '\x43', '\x63', '\x63', '\x4', '\x2', '\x44', '\x44', '\x64', 
		'\x64', '\x4', '\x2', '\x45', '\x45', '\x65', '\x65', '\x4', '\x2', '\x46', 
		'\x46', '\x66', '\x66', '\x4', '\x2', 'G', 'G', 'g', 'g', '\x4', '\x2', 
		'H', 'H', 'h', 'h', '\x4', '\x2', 'I', 'I', 'i', 'i', '\x4', '\x2', 'J', 
		'J', 'j', 'j', '\x4', '\x2', 'K', 'K', 'k', 'k', '\x4', '\x2', 'L', 'L', 
		'l', 'l', '\x4', '\x2', 'M', 'M', 'm', 'm', '\x4', '\x2', 'N', 'N', 'n', 
		'n', '\x4', '\x2', 'O', 'O', 'o', 'o', '\x4', '\x2', 'P', 'P', 'p', 'p', 
		'\x4', '\x2', 'Q', 'Q', 'q', 'q', '\x4', '\x2', 'R', 'R', 'r', 'r', '\x4', 
		'\x2', 'S', 'S', 's', 's', '\x4', '\x2', 'T', 'T', 't', 't', '\x4', '\x2', 
		'U', 'U', 'u', 'u', '\x4', '\x2', 'V', 'V', 'v', 'v', '\x4', '\x2', 'W', 
		'W', 'w', 'w', '\x4', '\x2', 'X', 'X', 'x', 'x', '\x4', '\x2', 'Y', 'Y', 
		'y', 'y', '\x4', '\x2', 'Z', 'Z', 'z', 'z', '\x4', '\x2', '[', '[', '{', 
		'{', '\x4', '\x2', '\\', '\\', '|', '|', '\x2', '\xFD', '\x2', '\x3', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\a', '\x3', '\x2', '\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\v', '\x3', '\x2', '\x2', '\x2', '\x2', '\r', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\xF', '\x3', '\x2', '\x2', '\x2', '\x2', '\x11', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x13', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x15', '\x3', '\x2', '\x2', '\x2', '\x2', '\x17', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x19', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1B', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x1D', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x1F', '\x3', '\x2', '\x2', '\x2', '\x2', '!', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '#', '\x3', '\x2', '\x2', '\x2', '\x2', '%', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\'', '\x3', '\x2', '\x2', '\x2', '\x2', ')', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '+', '\x3', '\x2', '\x2', '\x2', '\x2', '-', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '/', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x31', '\x3', '\x2', '\x2', '\x2', '\x2', '\x33', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x35', '\x3', '\x2', '\x2', '\x2', '\x3', 'k', '\x3', '\x2', 
		'\x2', '\x2', '\x5', 'm', '\x3', '\x2', '\x2', '\x2', '\a', 'o', '\x3', 
		'\x2', '\x2', '\x2', '\t', 'q', '\x3', '\x2', '\x2', '\x2', '\v', 's', 
		'\x3', '\x2', '\x2', '\x2', '\r', 'u', '\x3', '\x2', '\x2', '\x2', '\xF', 
		'w', '\x3', '\x2', '\x2', '\x2', '\x11', 'y', '\x3', '\x2', '\x2', '\x2', 
		'\x13', '|', '\x3', '\x2', '\x2', '\x2', '\x15', '\x7F', '\x3', '\x2', 
		'\x2', '\x2', '\x17', '\x82', '\x3', '\x2', '\x2', '\x2', '\x19', '\x86', 
		'\x3', '\x2', '\x2', '\x2', '\x1B', '\x89', '\x3', '\x2', '\x2', '\x2', 
		'\x1D', '\x90', '\x3', '\x2', '\x2', '\x2', '\x1F', '\x95', '\x3', '\x2', 
		'\x2', '\x2', '!', '\x9D', '\x3', '\x2', '\x2', '\x2', '#', '\xA2', '\x3', 
		'\x2', '\x2', '\x2', '%', '\xA9', '\x3', '\x2', '\x2', '\x2', '\'', '\xAE', 
		'\x3', '\x2', '\x2', '\x2', ')', '\xB4', '\x3', '\x2', '\x2', '\x2', '+', 
		'\xBB', '\x3', '\x2', '\x2', '\x2', '-', '\xC2', '\x3', '\x2', '\x2', 
		'\x2', '/', '\xC8', '\x3', '\x2', '\x2', '\x2', '\x31', '\xD0', '\x3', 
		'\x2', '\x2', '\x2', '\x33', '\xD4', '\x3', '\x2', '\x2', '\x2', '\x35', 
		'\xDD', '\x3', '\x2', '\x2', '\x2', '\x37', '\xE1', '\x3', '\x2', '\x2', 
		'\x2', '\x39', '\xE3', '\x3', '\x2', '\x2', '\x2', ';', '\xE5', '\x3', 
		'\x2', '\x2', '\x2', '=', '\xE7', '\x3', '\x2', '\x2', '\x2', '?', '\xE9', 
		'\x3', '\x2', '\x2', '\x2', '\x41', '\xEB', '\x3', '\x2', '\x2', '\x2', 
		'\x43', '\xED', '\x3', '\x2', '\x2', '\x2', '\x45', '\xEF', '\x3', '\x2', 
		'\x2', '\x2', 'G', '\xF1', '\x3', '\x2', '\x2', '\x2', 'I', '\xF3', '\x3', 
		'\x2', '\x2', '\x2', 'K', '\xF5', '\x3', '\x2', '\x2', '\x2', 'M', '\xF7', 
		'\x3', '\x2', '\x2', '\x2', 'O', '\xF9', '\x3', '\x2', '\x2', '\x2', 'Q', 
		'\xFB', '\x3', '\x2', '\x2', '\x2', 'S', '\xFD', '\x3', '\x2', '\x2', 
		'\x2', 'U', '\xFF', '\x3', '\x2', '\x2', '\x2', 'W', '\x101', '\x3', '\x2', 
		'\x2', '\x2', 'Y', '\x103', '\x3', '\x2', '\x2', '\x2', '[', '\x105', 
		'\x3', '\x2', '\x2', '\x2', ']', '\x107', '\x3', '\x2', '\x2', '\x2', 
		'_', '\x109', '\x3', '\x2', '\x2', '\x2', '\x61', '\x10B', '\x3', '\x2', 
		'\x2', '\x2', '\x63', '\x10D', '\x3', '\x2', '\x2', '\x2', '\x65', '\x10F', 
		'\x3', '\x2', '\x2', '\x2', 'g', '\x111', '\x3', '\x2', '\x2', '\x2', 
		'i', '\x113', '\x3', '\x2', '\x2', '\x2', 'k', 'l', '\a', '=', '\x2', 
		'\x2', 'l', '\x4', '\x3', '\x2', '\x2', '\x2', 'm', 'n', '\a', '.', '\x2', 
		'\x2', 'n', '\x6', '\x3', '\x2', '\x2', '\x2', 'o', 'p', '\a', '*', '\x2', 
		'\x2', 'p', '\b', '\x3', '\x2', '\x2', '\x2', 'q', 'r', '\a', '+', '\x2', 
		'\x2', 'r', '\n', '\x3', '\x2', '\x2', '\x2', 's', 't', '\a', '\x30', 
		'\x2', '\x2', 't', '\f', '\x3', '\x2', '\x2', '\x2', 'u', 'v', '\a', ',', 
		'\x2', '\x2', 'v', '\xE', '\x3', '\x2', '\x2', '\x2', 'w', 'x', '\a', 
		'?', '\x2', '\x2', 'x', '\x10', '\x3', '\x2', '\x2', '\x2', 'y', 'z', 
		'\a', '?', '\x2', '\x2', 'z', '{', '\a', '?', '\x2', '\x2', '{', '\x12', 
		'\x3', '\x2', '\x2', '\x2', '|', '}', '\a', '#', '\x2', '\x2', '}', '~', 
		'\a', '?', '\x2', '\x2', '~', '\x14', '\x3', '\x2', '\x2', '\x2', '\x7F', 
		'\x80', '\x5', '\x37', '\x1C', '\x2', '\x80', '\x81', '\x5', '[', '.', 
		'\x2', '\x81', '\x16', '\x3', '\x2', '\x2', '\x2', '\x82', '\x83', '\x5', 
		'\x37', '\x1C', '\x2', '\x83', '\x84', '\x5', '[', '.', '\x2', '\x84', 
		'\x85', '\x5', ';', '\x1E', '\x2', '\x85', '\x18', '\x3', '\x2', '\x2', 
		'\x2', '\x86', '\x87', '\x5', '\x39', '\x1D', '\x2', '\x87', '\x88', '\x5', 
		'g', '\x34', '\x2', '\x88', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x89', 
		'\x8A', '\x5', '=', '\x1F', '\x2', '\x8A', '\x8B', '\x5', '?', ' ', '\x2', 
		'\x8B', '\x8C', '\x5', 'M', '\'', '\x2', '\x8C', '\x8D', '\x5', '?', ' ', 
		'\x2', '\x8D', '\x8E', '\x5', ']', '/', '\x2', '\x8E', '\x8F', '\x5', 
		'?', ' ', '\x2', '\x8F', '\x1C', '\x3', '\x2', '\x2', '\x2', '\x90', '\x91', 
		'\x5', '=', '\x1F', '\x2', '\x91', '\x92', '\x5', '?', ' ', '\x2', '\x92', 
		'\x93', '\x5', '[', '.', '\x2', '\x93', '\x94', '\x5', ';', '\x1E', '\x2', 
		'\x94', '\x1E', '\x3', '\x2', '\x2', '\x2', '\x95', '\x96', '\x5', '?', 
		' ', '\x2', '\x96', '\x97', '\x5', '\x65', '\x33', '\x2', '\x97', '\x98', 
		'\x5', 'U', '+', '\x2', '\x98', '\x99', '\x5', 'M', '\'', '\x2', '\x99', 
		'\x9A', '\x5', '\x37', '\x1C', '\x2', '\x9A', '\x9B', '\x5', 'G', '$', 
		'\x2', '\x9B', '\x9C', '\x5', 'Q', ')', '\x2', '\x9C', ' ', '\x3', '\x2', 
		'\x2', '\x2', '\x9D', '\x9E', '\x5', '\x41', '!', '\x2', '\x9E', '\x9F', 
		'\x5', 'Y', '-', '\x2', '\x9F', '\xA0', '\x5', 'S', '*', '\x2', '\xA0', 
		'\xA1', '\x5', 'O', '(', '\x2', '\xA1', '\"', '\x3', '\x2', '\x2', '\x2', 
		'\xA2', '\xA3', '\x5', 'G', '$', '\x2', '\xA3', '\xA4', '\x5', 'Q', ')', 
		'\x2', '\xA4', '\xA5', '\x5', '[', '.', '\x2', '\xA5', '\xA6', '\x5', 
		'?', ' ', '\x2', '\xA6', '\xA7', '\x5', 'Y', '-', '\x2', '\xA7', '\xA8', 
		'\x5', ']', '/', '\x2', '\xA8', '$', '\x3', '\x2', '\x2', '\x2', '\xA9', 
		'\xAA', '\x5', 'G', '$', '\x2', '\xAA', '\xAB', '\x5', 'Q', ')', '\x2', 
		'\xAB', '\xAC', '\x5', ']', '/', '\x2', '\xAC', '\xAD', '\x5', 'S', '*', 
		'\x2', '\xAD', '&', '\x3', '\x2', '\x2', '\x2', '\xAE', '\xAF', '\x5', 
		'S', '*', '\x2', '\xAF', '\xB0', '\x5', 'Y', '-', '\x2', '\xB0', '\xB1', 
		'\x5', '=', '\x1F', '\x2', '\xB1', '\xB2', '\x5', '?', ' ', '\x2', '\xB2', 
		'\xB3', '\x5', 'Y', '-', '\x2', '\xB3', '(', '\x3', '\x2', '\x2', '\x2', 
		'\xB4', '\xB5', '\x5', '[', '.', '\x2', '\xB5', '\xB6', '\x5', '?', ' ', 
		'\x2', '\xB6', '\xB7', '\x5', 'M', '\'', '\x2', '\xB7', '\xB8', '\x5', 
		'?', ' ', '\x2', '\xB8', '\xB9', '\x5', ';', '\x1E', '\x2', '\xB9', '\xBA', 
		'\x5', ']', '/', '\x2', '\xBA', '*', '\x3', '\x2', '\x2', '\x2', '\xBB', 
		'\xBC', '\x5', '\x61', '\x31', '\x2', '\xBC', '\xBD', '\x5', '\x37', '\x1C', 
		'\x2', '\xBD', '\xBE', '\x5', 'M', '\'', '\x2', '\xBE', '\xBF', '\x5', 
		'_', '\x30', '\x2', '\xBF', '\xC0', '\x5', '?', ' ', '\x2', '\xC0', '\xC1', 
		'\x5', '[', '.', '\x2', '\xC1', ',', '\x3', '\x2', '\x2', '\x2', '\xC2', 
		'\xC3', '\x5', '\x63', '\x32', '\x2', '\xC3', '\xC4', '\x5', '\x45', '#', 
		'\x2', '\xC4', '\xC5', '\x5', '?', ' ', '\x2', '\xC5', '\xC6', '\x5', 
		'Y', '-', '\x2', '\xC6', '\xC7', '\x5', '?', ' ', '\x2', '\xC7', '.', 
		'\x3', '\x2', '\x2', '\x2', '\xC8', '\xCC', '\t', '\x2', '\x2', '\x2', 
		'\xC9', '\xCB', '\t', '\x3', '\x2', '\x2', '\xCA', '\xC9', '\x3', '\x2', 
		'\x2', '\x2', '\xCB', '\xCE', '\x3', '\x2', '\x2', '\x2', '\xCC', '\xCA', 
		'\x3', '\x2', '\x2', '\x2', '\xCC', '\xCD', '\x3', '\x2', '\x2', '\x2', 
		'\xCD', '\x30', '\x3', '\x2', '\x2', '\x2', '\xCE', '\xCC', '\x3', '\x2', 
		'\x2', '\x2', '\xCF', '\xD1', '\t', '\x4', '\x2', '\x2', '\xD0', '\xCF', 
		'\x3', '\x2', '\x2', '\x2', '\xD1', '\xD2', '\x3', '\x2', '\x2', '\x2', 
		'\xD2', '\xD0', '\x3', '\x2', '\x2', '\x2', '\xD2', '\xD3', '\x3', '\x2', 
		'\x2', '\x2', '\xD3', '\x32', '\x3', '\x2', '\x2', '\x2', '\xD4', '\xD8', 
		'\a', ')', '\x2', '\x2', '\xD5', '\xD7', '\n', '\x5', '\x2', '\x2', '\xD6', 
		'\xD5', '\x3', '\x2', '\x2', '\x2', '\xD7', '\xDA', '\x3', '\x2', '\x2', 
		'\x2', '\xD8', '\xD6', '\x3', '\x2', '\x2', '\x2', '\xD8', '\xD9', '\x3', 
		'\x2', '\x2', '\x2', '\xD9', '\xDB', '\x3', '\x2', '\x2', '\x2', '\xDA', 
		'\xD8', '\x3', '\x2', '\x2', '\x2', '\xDB', '\xDC', '\a', ')', '\x2', 
		'\x2', '\xDC', '\x34', '\x3', '\x2', '\x2', '\x2', '\xDD', '\xDE', '\t', 
		'\x6', '\x2', '\x2', '\xDE', '\xDF', '\x3', '\x2', '\x2', '\x2', '\xDF', 
		'\xE0', '\b', '\x1B', '\x2', '\x2', '\xE0', '\x36', '\x3', '\x2', '\x2', 
		'\x2', '\xE1', '\xE2', '\t', '\a', '\x2', '\x2', '\xE2', '\x38', '\x3', 
		'\x2', '\x2', '\x2', '\xE3', '\xE4', '\t', '\b', '\x2', '\x2', '\xE4', 
		':', '\x3', '\x2', '\x2', '\x2', '\xE5', '\xE6', '\t', '\t', '\x2', '\x2', 
		'\xE6', '<', '\x3', '\x2', '\x2', '\x2', '\xE7', '\xE8', '\t', '\n', '\x2', 
		'\x2', '\xE8', '>', '\x3', '\x2', '\x2', '\x2', '\xE9', '\xEA', '\t', 
		'\v', '\x2', '\x2', '\xEA', '@', '\x3', '\x2', '\x2', '\x2', '\xEB', '\xEC', 
		'\t', '\f', '\x2', '\x2', '\xEC', '\x42', '\x3', '\x2', '\x2', '\x2', 
		'\xED', '\xEE', '\t', '\r', '\x2', '\x2', '\xEE', '\x44', '\x3', '\x2', 
		'\x2', '\x2', '\xEF', '\xF0', '\t', '\xE', '\x2', '\x2', '\xF0', '\x46', 
		'\x3', '\x2', '\x2', '\x2', '\xF1', '\xF2', '\t', '\xF', '\x2', '\x2', 
		'\xF2', 'H', '\x3', '\x2', '\x2', '\x2', '\xF3', '\xF4', '\t', '\x10', 
		'\x2', '\x2', '\xF4', 'J', '\x3', '\x2', '\x2', '\x2', '\xF5', '\xF6', 
		'\t', '\x11', '\x2', '\x2', '\xF6', 'L', '\x3', '\x2', '\x2', '\x2', '\xF7', 
		'\xF8', '\t', '\x12', '\x2', '\x2', '\xF8', 'N', '\x3', '\x2', '\x2', 
		'\x2', '\xF9', '\xFA', '\t', '\x13', '\x2', '\x2', '\xFA', 'P', '\x3', 
		'\x2', '\x2', '\x2', '\xFB', '\xFC', '\t', '\x14', '\x2', '\x2', '\xFC', 
		'R', '\x3', '\x2', '\x2', '\x2', '\xFD', '\xFE', '\t', '\x15', '\x2', 
		'\x2', '\xFE', 'T', '\x3', '\x2', '\x2', '\x2', '\xFF', '\x100', '\t', 
		'\x16', '\x2', '\x2', '\x100', 'V', '\x3', '\x2', '\x2', '\x2', '\x101', 
		'\x102', '\t', '\x17', '\x2', '\x2', '\x102', 'X', '\x3', '\x2', '\x2', 
		'\x2', '\x103', '\x104', '\t', '\x18', '\x2', '\x2', '\x104', 'Z', '\x3', 
		'\x2', '\x2', '\x2', '\x105', '\x106', '\t', '\x19', '\x2', '\x2', '\x106', 
		'\\', '\x3', '\x2', '\x2', '\x2', '\x107', '\x108', '\t', '\x1A', '\x2', 
		'\x2', '\x108', '^', '\x3', '\x2', '\x2', '\x2', '\x109', '\x10A', '\t', 
		'\x1B', '\x2', '\x2', '\x10A', '`', '\x3', '\x2', '\x2', '\x2', '\x10B', 
		'\x10C', '\t', '\x1C', '\x2', '\x2', '\x10C', '\x62', '\x3', '\x2', '\x2', 
		'\x2', '\x10D', '\x10E', '\t', '\x1D', '\x2', '\x2', '\x10E', '\x64', 
		'\x3', '\x2', '\x2', '\x2', '\x10F', '\x110', '\t', '\x1E', '\x2', '\x2', 
		'\x110', '\x66', '\x3', '\x2', '\x2', '\x2', '\x111', '\x112', '\t', '\x1F', 
		'\x2', '\x2', '\x112', 'h', '\x3', '\x2', '\x2', '\x2', '\x113', '\x114', 
		'\t', ' ', '\x2', '\x2', '\x114', 'j', '\x3', '\x2', '\x2', '\x2', '\x6', 
		'\x2', '\xCC', '\xD2', '\xD8', '\x3', '\x2', '\x3', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace SimpleDatabase.Parsing.Antlr
