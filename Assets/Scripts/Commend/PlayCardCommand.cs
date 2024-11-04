using UnityEngine;

public class PlayCardCommand : ICommand<PlayerContext>
{
    public void Execute(PlayerContext context)
    {
        Debug.Log("PlayCardCommand");
        CardPlayer player = context.playerBase as CardPlayer;
        if (!context.isDrawOpponent)
        {
            bool result = player.PlayCard();
            if (result == true)
            {
                Debug.Log("PlayCardCommand player.PlayCard() == true");
                context.cardGameManager.AdvanceRound();
            }

        }
        else
        {
            CardBase card = player.drawOpponentCard.DrawCard();
            UIManager.Instance.HideDrawOpponentTutorial();
            CameraController.Instance.RestoreCamera();
            if (card is NumberCard)
            {
                NumberCard numberCard = card as NumberCard;
                context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.AdvanceTurn(), 1.5f));
            }
            else
            {
                CameraController.Instance.ShakeCamera();
                AudioManager.Instance.Play("corrupt");
                UIManager.Instance.ShowMessage("You draw a bomb card!");
                context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() =>player.Hurt(), 1f));
                context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.Reset(), 1.5f));
            }
        }
    }

}
