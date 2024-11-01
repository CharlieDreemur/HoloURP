using UnityEngine;

public class NavigateRightCommand : ICommand<PlayerContext>
{
 public void Execute(PlayerContext context)
    {
        //Debug.Log("NavigateRightCommand");
        CardPlayer player = context.playerBase as CardPlayer;
        if (!context.isDrawOpponent)
        {
            player.handZoneVisual.NavigateRight();
        }
        else{
            player.drawOpponentCard.NavigateRight();
        }
    }

}
