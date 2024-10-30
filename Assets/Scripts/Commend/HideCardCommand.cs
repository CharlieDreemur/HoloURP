using UnityEngine;

public class HideShowCardCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        CardPlayer player = context.playerBase as CardPlayer;
        player.handZoneVisual.HideShowHand();
    }

}
