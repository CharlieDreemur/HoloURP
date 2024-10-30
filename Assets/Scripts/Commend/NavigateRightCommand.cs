using UnityEngine;

public class NavigateRightCommand : ICommand<PlayerContext>
{
 public void Execute(PlayerContext context)
    {
        CardPlayer player = context.playerBase as CardPlayer;
        player.handZoneVisual.NavigateRight();
    }

}
