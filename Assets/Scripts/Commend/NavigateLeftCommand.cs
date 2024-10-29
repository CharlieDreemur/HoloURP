using UnityEngine;

public class NavigateLeftCommand : ICommand<GameContext>
{
    public void Execute(GameContext context)
    {
        context.handZoneVisual.NavigateLeft();
    }

}
