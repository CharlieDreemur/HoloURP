using Unity.VisualScripting;
using UnityEngine;

public class EndTurnCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        Debug.Log("End Turn");
        context.playZone.AddCardsIntoDeck();
        context.handZoneVisual.HideHand();
        context.cardGameManager.AdvanceTurn();
    }

}
