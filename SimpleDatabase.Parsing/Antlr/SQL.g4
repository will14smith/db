grammar SQL;

program: statement (';' statement)* ';'?;

// statements
statement: (K_EXPLAIN)? 
	( statement_select
	| statement_insert
	);

statement_select: K_SELECT Columns+=result_column (',' Columns+=result_column)* K_FROM Table=table_name (K_WHERE Where=expression)?;
statement_insert: K_INSERT K_INTO Table=table_name (Columns+=column_name (',' Columns+=column_name)*)? K_VALUES Values+=statement_insert_value (',' Values+=statement_insert_value)*;
statement_insert_value: '(' Values+=expression (',' Values+=expression)* ')';

// columns
result_column
	: (table_name '.')? '*' # result_column_star
	| expression (K_AS? column_alias)? # result_column_expr
	;

column_alias: IDENTIFIER;
column_name: IDENTIFIER;

// tables
table_name: IDENTIFIER;

// expressions
expression
	: literal_value # expression_literal
	| (table_name '.')? column_name # expression_column
	| expression Operator=('=' | '==' | '!=') expression # expression_equality
	;


literal_value
	: NUMBER_LITERAL
	| STRING_LITERAL
	;

// keywords
K_AS: A S;
K_EXPLAIN: E X P L A I N;
K_FROM: F R O M;
K_INSERT: I N S E R T;
K_INTO: I N T O;
K_SELECT: S E L E C T;
K_VALUES: V A L U E S;
K_WHERE: W H E R E;

// tokens
IDENTIFIER: [a-zA-Z_] [a-zA-Z_0-9]*;
NUMBER_LITERAL: [0-9]+;
STRING_LITERAL: '\'' (~'\'')* '\''; // TODO handle escapes

WS: [ \t\r\n] -> channel(HIDDEN);


// fragments
fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];