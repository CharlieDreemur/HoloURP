using System.Collections.Generic;
using UnityEngine;
// ICommand represents a command that can be executed and undone by player
public interface ICommand
{
    void Execute();
    void Undo();
}

/// <summary>
/// TurnManager is responsible for managing the single turn of the player and the AI.
/// </summary>
public class TurnManager : MonoBehaviour
{
    public Stack<ICommand> commandHistory = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        commandHistory.Push(command);
    }

    public void UndoLastCommand()
    {
        if (commandHistory.Count > 0)
        {
            ICommand lastCommand = commandHistory.Pop();
            lastCommand.Undo();
        }
    }
}
