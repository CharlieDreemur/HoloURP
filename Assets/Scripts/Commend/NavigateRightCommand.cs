using UnityEngine;

public class NavigateRightCommand : ICommand<PlayerContext>
{
 public void Execute(PlayerContext context)
    {
        Debug.Log("NavigateRightCommand");
        AudioManager.Instance.Play("cardswipe");
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
