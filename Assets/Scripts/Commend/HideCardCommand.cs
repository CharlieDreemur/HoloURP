using UnityEngine;

public class HideShowCardCommand : ICommand<GameContext>
{
    public void Execute(GameContext context)
    {
        context.handZoneVisual.HideShowHand();
    }

}
