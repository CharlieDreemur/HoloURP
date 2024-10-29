using UnityEngine;

public class PlayCardCommand : ICommand<GameContext>
{
    public void Execute(GameContext context)
    {
        context.player.PlayCard(context.handZoneVisual.CurrentCardIndex);
    }

}
