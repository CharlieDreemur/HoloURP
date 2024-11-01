using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class AITurnState : IGameState
{
    private CardGameManager cardGameManager;
    private PlayerContext context;

    public AITurnState(CardGameManager cardGameManager, PlayerContext context)
    {
        this.cardGameManager = cardGameManager;
        this.context = context;
    }

    public void Enter()
    {
        Debug.Log("AI's Turn Started");
        AIPlayer aiPlayer = (AIPlayer)context.playerBase;
        cardGameManager.StartCoroutine(cardGameManager.WaitForSeconds(() => aiPlayer.PlayRandomCard(context), 1.5f));
    }

     public void PunishOpponent(PlayerBase opponent)
    {

    }
    public void Execute()
    {
    }

    public void Exit()
    {
        Debug.Log("AI's Turn Ended");
    }

}