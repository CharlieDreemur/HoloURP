using UnityEngine;
// ICommand represents a command that can be executed and undone by player
public interface ICommand<T> where T : IContext
{
    void Execute(T context);

}

public class DrawCardCommand : ICommand<GameContext>
{
    public void Execute(GameContext context)
    {
        context.cardDeck.DrawCards(context.player);
    }

}
