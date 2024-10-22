using UnityEngine;
public class PlayerTurnState : ITurnState
{
    private CardGameManager cardGameManager;
    private TurnManager turnManager;

    public PlayerTurnState(CardGameManager cardGameManager, TurnManager turnManager)
    {
        this.cardGameManager = cardGameManager;
        this.turnManager = turnManager;
    }

    public void Enter()
    {
        Debug.Log("Player's Turn Started");
        // Additional logic for entering player's turn
    }

    public void Execute()
    {
        // Handle player's inputs and execute commands
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ICommand playCardCommand = new DrawCardCommand();
            turnManager.ExecuteCommand(playCardCommand);
        }

        // End turn condition (e.g., pressing E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            cardGameManager.SetState(new AITurnState(cardGameManager, turnManager));
        }
    }

    public void Exit()
    {
        Debug.Log("Player's Turn Ended");
    }
}
