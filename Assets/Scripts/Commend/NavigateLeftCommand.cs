using UnityEngine;

public class NavigateLeftCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        context.handZoneVisual.NavigateLeft();
    }

}
