using System.Collections.Generic;

namespace SimpleDatabase.CLI.Commands;

public class CommandHandler
{
    private readonly Dictionary<string, ICommand> _commands = new();

    public void Register(string key, ICommand command) => _commands[key] = command;
    
    public CommandResponse Handle(string input)
    {
        var args = ParseArgs(input);

        return _commands.TryGetValue(args[0], out var command) 
            ? command.Handle(args) 
            : new CommandResponse.Unrecognised(input);
    }

    private static string[] ParseArgs(string input)
    {
        if (input.StartsWith("."))
        {
            input = input[1..];
        }

        // TODO handle quoting
        return input.Split(" ");
    }
}