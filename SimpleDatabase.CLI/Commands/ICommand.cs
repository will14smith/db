namespace SimpleDatabase.CLI.Commands;

public interface ICommand
{
    CommandResponse Handle(string[] args);
}