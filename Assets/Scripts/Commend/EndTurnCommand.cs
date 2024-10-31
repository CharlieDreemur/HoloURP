using Unity.VisualScripting;
using UnityEngine;

public class EndTurnCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        Debug.Log("End Turn");
        CardPlayer player = context.playerBase as CardPlayer;
        player.handZoneVisual.HideHand();
        context.cardGameManager.EndTurn();
    }

}
