using System;

public class SpawnEnemyCommand : ICommand
{
    public string Name => "spawn_enemy";
    public string Description => "Spawns enemies. Usage: spawn_enemy <name> [<count>]";

    public void Execute(string[] parameters, Action<string> outputCallback)
    {
        if (parameters.Length == 0)
        {
            outputCallback("Error: Missing required parameter. Usage: spawn_enemy <name> [<count>].");
            return;
        }

        string enemyName = parameters[0];
        int enemyCount = 1;

        if (parameters.Length > 1)
        {
            if (!int.TryParse(parameters[1], out enemyCount) || enemyCount <= 0)
            {
                outputCallback($"Error: Invalid number of enemies '{parameters[1]}'. Please specify a positive integer.");
                return;
            }
        }

        // Add actual logic for spawning enemies here
        bool success = EnemyPool.Instance.SpawnEnemy(enemyName, enemyCount);

        if (success)
        {
            outputCallback($"Spawned {enemyCount} enemies of type '{enemyName}'.");
        }
        else
        {
            outputCallback($"Error: Failed to spawn enemies of type '{enemyName}'.");
        }
    }
}
