using System;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    void Execute(string[] parameters, Action<string> outputCallback);
}
