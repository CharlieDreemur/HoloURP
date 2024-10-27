using UnityEngine;
public class AITurnState : IGameState
{
    private CardGameManager cardGameManager;

    public AITurnState(CardGameManager cardGameManager)
    {
        this.cardGameManager = cardGameManager;
    }

    public void Enter()
    {
        Debug.Log("AI's Turn Started");
        // AI turn logic
    }

    public void Execute()
    {
        // Execute AI actions (simplified as an example)
        //ICommand aiActionCommand = new DrawCardCommand();

        // After AI finishes, switch back to player’s turn
        cardGameManager.SetState(new PlayerTurnState(cardGameManager));
    }

    public void Exit()
    {
        Debug.Log("AI's Turn Ended");
    }
}