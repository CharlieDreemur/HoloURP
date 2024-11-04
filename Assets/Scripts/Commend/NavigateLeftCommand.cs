using UnityEngine;

public class NavigateLeftCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        Debug.Log("NavigateLeftCommand");
        AudioManager.Instance.Play("cardswipe");
        CardPlayer player = context.playerBase as CardPlayer;
        if (!context.isDrawOpponent)
        {
            player.handZoneVisual.NavigateLeft();
        }
        else{
            player.drawOpponentCard.NavigateLeft();
        }
    }

}
