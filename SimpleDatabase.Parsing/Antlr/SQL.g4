grammar SQL;

program: statement (';' statement)* ';'?;

// statements
statement: statement_dml | statement_ddl;

// dml statements
statement_dml: (K_EXPLAIN)? 
	( statement_select
	| statement_insert
	| statement_delete
	);


statement_select: K_SELECT Columns+=result_column (',' Columns+=result_column)* K_FROM Table=table_name (K_WHERE Where=expression)? (K_ORDER K_BY Ordering+=ordering_term (',' Ordering+=ordering_term)*)?;
statement_insert: K_INSERT K_INTO Table=table_name ('(' Columns+=column_name (',' Columns+=column_name)* ')')? K_VALUES Values+=statement_insert_value (',' Values+=statement_insert_value)*;
statement_insert_value: '(' Values+=expression (',' Values+=expression)* ')';
statement_delete: K_DELETE K_FROM Table=table_name (K_WHERE Where=expression)?;

// ddl statements
statement_ddl: 
    ( statement_ddl_create_table
    );
statement_ddl_create_table: K_CREATE K_TABLE (K_IF K_NOT K_EXISTS)? Table=table_name '(' column_definition_list ')';

// columns
result_column
	: (table_name '.')? '*' # result_column_star
	| expression (K_AS? column_alias)? # result_column_expr
	;

column_alias: IDENTIFIER;
column_name: IDENTIFIER;

ordering_term: expression (K_ASC | K_DESC)?;

column_definition_list: Columns+=column_definition (',' Columns+=column_definition)*;
column_definition: column_name column_type;
column_type
    : IDENTIFIER # column_type_no_args
    | IDENTIFIER '(' NUMBER_LITERAL ')' # column_type_one_arg
    ;

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
K_ASC: A S C;
K_BY: B Y;
K_CREATE: C R E A T E;
K_DELETE: D E L E T E;
K_DESC: D E S C;
K_EXISTS: E X I S T S;
K_EXPLAIN: E X P L A I N;
K_FROM: F R O M;
K_IF: I F;
K_INSERT: I N S E R T;
K_INTO: I N T O;
K_NOT: N O T;
K_ORDER: O R D E R;
K_SELECT: S E L E C T;
K_TABLE: T A B L E;
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