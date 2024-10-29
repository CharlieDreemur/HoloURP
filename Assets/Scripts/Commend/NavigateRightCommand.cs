using UnityEngine;

public class NavigateRightCommand : ICommand<GameContext>
{
 public void Execute(GameContext context)
    {
        context.handZoneVisual.NavigateRight();
    }

}
