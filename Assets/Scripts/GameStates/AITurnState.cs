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
        Debug.Log("AI's Turn PunishOpponent");
        AIPlayer aiPlayer = (AIPlayer)context.playerBase;
        CardPlayer cardPlayer = (CardPlayer)opponent;
        cardPlayer.handZoneVisual.ShowHand();
        cardGameManager.StartCoroutine(cardGameManager.WaitForSeconds(() => aiPlayer.PunishOpponent(opponent), 1.5f));
        cardGameManager.StartCoroutine(cardGameManager.WaitForSeconds(() => cardGameManager.AdvanceTurn(), 2f));
    }

    public void Exit()
    {
        Debug.Log("AI's Turn Ended");
    }

}