using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class AITurnState : IGameState
{
    private CardGameManager cardGameManager;
    private GameContext gameContext;

    public AITurnState(CardGameManager cardGameManager)
    {
        this.cardGameManager = cardGameManager;
        gameContext = cardGameManager.playerContext;
        gameContext.cardGameManager = cardGameManager;

    }

    public void Enter()
    {
        Debug.Log("AI's Turn Started");
        gameContext.cardDeck.DrawCards(gameContext.aiPlayer);
        cardGameManager.StartCoroutine(WaitForSeconds(() => gameContext.aiPlayer.PlayRandomCard(), 1.5f));
        cardGameManager.StartCoroutine(WaitForSeconds(() => cardGameManager.AdvanceTurn(), 1.5f));
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        Debug.Log("AI's Turn Ended");
    }

    IEnumerator WaitForSeconds(UnityAction callback, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        callback.Invoke();
    }
}