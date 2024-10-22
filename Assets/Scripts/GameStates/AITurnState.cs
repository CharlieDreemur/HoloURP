using UnityEngine;
public class AITurnState : ITurnState
{
    private CardGameManager cardGameManager;
    private TurnManager turnManager;

    public AITurnState(CardGameManager cardGameManager, TurnManager turnManager)
    {
        this.cardGameManager = cardGameManager;
        this.turnManager = turnManager;
    }

    public void Enter()
    {
        Debug.Log("AI's Turn Started");
        // AI turn logic
    }

    public void Execute()
    {
        // Execute AI actions (simplified as an example)
        ICommand aiActionCommand = new DrawCardCommand();
        turnManager.ExecuteCommand(aiActionCommand);

        // After AI finishes, switch back to player’s turn
        cardGameManager.SetState(new PlayerTurnState(cardGameManager, turnManager));
    }

    public void Exit()
    {
        Debug.Log("AI's Turn Ended");
    }
}