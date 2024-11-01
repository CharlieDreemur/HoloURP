using UnityEngine;

public class NavigateRightCommand : ICommand<PlayerContext>
{
 public void Execute(PlayerContext context)
    {
        CardPlayer player = context.playerBase as CardPlayer;
        if (!context.isPunished)
        {
            player.handZoneVisual.NavigateRight();
        }
        else{
            
        }
    }

}
