using UnityEngine;

public class NavigateRightCommand : ICommand<PlayerContext>
{
 public void Execute(PlayerContext context)
    {
        context.handZoneVisual.NavigateRight();
    }

}
