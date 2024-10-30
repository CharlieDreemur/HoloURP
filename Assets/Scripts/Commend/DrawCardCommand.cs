using UnityEngine;
// ICommand represents a command that can be executed and undone by player
public interface ICommand<T> where T : IContext
{
    void Execute(T context);

}

public class DrawCardCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        CardPlayer player = context.playerBase as CardPlayer;
        player.DrawCards();
    }

}
