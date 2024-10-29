using Unity.VisualScripting;
using UnityEngine;

public class EndTurnCommand : ICommand<GameContext>
{
    public void Execute(GameContext context)
    {
        Debug.Log("End Turn");
        context.playZone.AddCardsIntoDeck();
        context.handZoneVisual.HideHand();
        context.cardGameManager.AdvanceTurn();
    }

}
