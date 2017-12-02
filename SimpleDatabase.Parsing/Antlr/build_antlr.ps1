$script_folder = Split-Path $MyInvocation.MyCommand.Path -Parent

$tool_path = Join-Path $script_folder "../../tools/antlr-4.7-complete.jar"
$grammar_path = Join-Path $script_folder "SQL.g4"
$output_folder = $script_folder

java -jar $tool_path -Dlanguage=CSharp -o $output_folder -package SimpleDatabase.Parsing.Antlr -visitor -no-listener $grammar_path
pause