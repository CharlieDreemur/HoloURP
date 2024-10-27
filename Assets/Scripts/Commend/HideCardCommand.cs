using UnityEngine;

public class HideShowCardCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        context.handZoneVisual.HideShowHand();
    }

}
