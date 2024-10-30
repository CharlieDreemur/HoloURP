using UnityEngine;

public class NavigateLeftCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        CardPlayer player = context.playerBase as CardPlayer;
        player.handZoneVisual.NavigateLeft();
    }

}
