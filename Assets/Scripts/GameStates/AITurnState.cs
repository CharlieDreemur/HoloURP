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

    public void DrawOpponent(PlayerBase opponent)
    {
        AudioManager.Instance.Play("roundwin");
        UIManager.Instance.ShowMessage("You Win! You opponent should draw a card from you!");
        AIPlayer aiPlayer = (AIPlayer)context.playerBase;
        CardPlayer cardPlayer = (CardPlayer)opponent;
        cardPlayer.handZoneVisual.ShowHand();
        UnityAction action = () =>
        {
            CardBase card = aiPlayer.DrawOpponent(opponent);
            if(card is NumberCard)
            {
                cardGameManager.StartCoroutine(cardGameManager.WaitForSeconds(() => cardGameManager.AdvanceTurn(), 2f));
            }
            else
            {
                aiPlayer.Hurt();
                Debug.Log("AI's Turn DrawOpponent:You draw a bomb card");
                cardGameManager.StartCoroutine(cardGameManager.WaitForSeconds(() => cardGameManager.Reset(), 2f));
            }
        };
        cardGameManager.StartCoroutine(cardGameManager.WaitForSeconds(action, 1.5f));
    }

    public void Exit()
    {
        Debug.Log("AI's Turn Ended");
    }

}