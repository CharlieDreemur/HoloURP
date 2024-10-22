using UnityEngine;
public interface ITurnState
{
    void Enter();
    void Execute();
    void Exit();
}

/// <summary>
/// CardGameManager is the main game manager that controls the state transitions of each turn, for example, the player's turn, the enemy's turn, etc.
/// </summary>
public class CardGameManager : MonoBehaviour
{
    private ITurnState currentState;
    public void SetState(ITurnState newState)
    {
        // Exit current state and enter new state
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

     private void Update()
    {
        // Continuously execute the current state logic
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
}
