using UnityEngine;

public class PlayCardCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        context.player.PlayCard(context.handZoneVisual.CurrentCardIndex);
    }

}
