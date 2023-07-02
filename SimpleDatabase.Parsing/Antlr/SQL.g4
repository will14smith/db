grammar SQL;

program: statement (';' statement)* ';'? EOF;

// statements
statement: statement_dml | statement_ddl;

// dml statements
statement_dml: (K_EXPLAIN K_EXECUTE?)? 
	( statement_dml_select
	| statement_dml_insert
	| statement_dml_delete
	);

statement_dml_select: K_SELECT Columns+=result_column (',' Columns+=result_column)* K_FROM table_from (K_WHERE Where=expression)? (K_ORDER K_BY Ordering+=ordering_term (',' Ordering+=ordering_term)*)?;
statement_dml_insert: K_INSERT K_INTO Table=table_name ('(' Columns+=column_name (',' Columns+=column_name)* ')')? K_VALUES Values+=statement_dml_insert_value (',' Values+=statement_dml_insert_value)*;
statement_dml_insert_value: '(' Values+=expression (',' Values+=expression)* ')';
statement_dml_delete: K_DELETE K_FROM Table=table_name (K_WHERE Where=expression)?;

table_from: table_alias table_join*;
table_alias: table=table_name K_AS? alias=table_name?; 
table_join: K_JOIN table_alias (K_ON expression)?;

// ddl statements
statement_ddl: 
    ( statement_ddl_create_table
    | statement_ddl_create_index
    );
    
statement_ddl_create_table: K_CREATE K_TABLE (K_IF K_NOT K_EXISTS)? Table=table_name '(' Columns+=column_definition (',' Columns+=column_definition)* ')';
statement_ddl_create_index: K_CREATE K_INDEX (K_IF K_NOT K_EXISTS)? Index=index_name K_ON Table=table_name '(' Columns+=index_column (',' Columns+=index_column)* ')' statement_ddl_create_index_including?;
statement_ddl_create_index_including: K_INCLUDING '(' Columns+=column_name (',' Columns+=column_name)* ')';
 
// columns
result_column
	: (table_name '.')? '*' # result_column_star
	| expression (K_AS? column_alias)? # result_column_expr
	;

column_alias: IDENTIFIER;
column_name: IDENTIFIER;

ordering_term: expression (K_ASC | K_DESC)?;

column_definition: column_name column_type;
column_type
    : IDENTIFIER # column_type_no_args
    | IDENTIFIER '(' NUMBER_LITERAL ')' # column_type_one_arg
    ;

// tables
table_name: IDENTIFIER;
index_name: IDENTIFIER;
index_column: column_name (K_ASC | K_DESC)?;

// expressions
expression
	: literal_value # expression_literal
	| (table_name '.')? column_name # expression_column
	| expression Operator=('=' | '==' | '!=') expression # expression_equality
	| expression Operator=(K_AND | K_OR) expression # expression_boolean
	;

literal_value
	: NUMBER_LITERAL
	| STRING_LITERAL
	;

// keywords
K_AND: A N D;
K_AS: A S;
K_ASC: A S C;
K_BY: B Y;
K_CREATE: C R E A T E;
K_DELETE: D E L E T E;
K_DESC: D E S C;
K_EXECUTE: E X E C U T E;
K_EXISTS: E X I S T S;
K_EXPLAIN: E X P L A I N;
K_FROM: F R O M;
K_IF: I F;
K_INCLUDING: I N C L U D I N G;
K_INDEX: I N D E X;
K_INSERT: I N S E R T;
K_INTO: I N T O;
K_JOIN: J O I N;
K_NOT: N O T;
K_ON: O N;
K_OR: O R;
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