using System;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    string Usage { get; }
    void Execute(string[] parameters, Action<string> outputCallback);
}
