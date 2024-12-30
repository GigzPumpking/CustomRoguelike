using System;
using System.Collections.Generic;
public class ClearHistoryCommand : ICommand
{
    private readonly Action clearHistoryCallback;

    public ClearHistoryCommand(Action clearHistoryCallback)
    {
        this.clearHistoryCallback = clearHistoryCallback;
    }

    public string Name => "clear";
    public string Description => "Clears the command history.";
    public string Usage => "clear";

    public void Execute(string[] parameters, Action<string> outputCallback)
    {
        // Ignore parameters and clear the history
        clearHistoryCallback?.Invoke();
    }
}

public class HelpCommand : ICommand
{
    private readonly Dictionary<string, ICommand> commands;

    public HelpCommand(Dictionary<string, ICommand> commands)
    {
        this.commands = commands;
    }

    public string Name => "help";
    public string Description => "Displays a list of available commands.";
    public string Usage => "help";

    public void Execute(string[] parameters, Action<string> outputCallback)
    {
        outputCallback("Available commands:");
        foreach (var command in commands.Values)
        {
            outputCallback($"- {command.Name}: {command.Description}");
        }
    }
}
