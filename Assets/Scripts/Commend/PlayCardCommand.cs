using UnityEngine;

public class PlayCardCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        CardPlayer player = context.playerBase as CardPlayer;
        bool result = player.PlayCard();
        if(result==true) context.cardGameManager.AdvanceRound();   
        else{
            UIManager.Instance.ShowMessage("You can't play a smaller card");
        }
    }

}
