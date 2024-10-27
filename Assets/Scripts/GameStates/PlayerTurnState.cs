using UnityEngine;
[System.Serializable]
public struct PlayerContext : IContext
{
    [SerializeField]
    public PlayerTurnState playerTurnState; // Maybe should not reference too much
    [SerializeField]
    public CardPlayer player;
    [SerializeField]
    public HandZoneVisual handZoneVisual;
    [SerializeField]
    public CardDeck cardDeck;
    [SerializeField]
    public PlayZone playZone;
}
public class PlayerTurnState : IGameState
{
    private CardGameManager cardGameManager;
    private PlayerContext playerContext;
    private InputControls _controls;
    public PlayerTurnState(CardGameManager cardGameManager)
    {
        this.cardGameManager = cardGameManager;
        playerContext = cardGameManager.playerContext;
        playerContext.playerTurnState = this;
        _controls = new InputControls();
        InitKeyCommandMap();

    }

    public void Enter()
    {
        Debug.Log("Player's Turn Started");
        _controls.Enable();
        // Additional logic for entering player's turn
    }

    public void InitKeyCommandMap()
    {
        _controls.Player.DrawCard.performed += ctx =>
        {
            var drawCardCommand = new DrawCardCommand();
            drawCardCommand.Execute(playerContext);
        };
        _controls.Player.PlayCard.performed += ctx =>
        {
            var playCardCommand = new PlayCardCommand();
            playCardCommand.Execute(playerContext);
        };
        _controls.Player.EndTurn.performed += ctx =>
        {
            var endTurnCommand = new EndTurnCommand();
            endTurnCommand.Execute(playerContext);
        };
        _controls.Player.HideHandZone.performed += ctx =>
        {
            var hideCardCommand = new HideShowCardCommand();
            hideCardCommand.Execute(playerContext);
        };
        _controls.Player.NavigateLeft.performed += ctx =>
        {
            var navigateLeftCommand = new NavigateLeftCommand();
            navigateLeftCommand.Execute(playerContext);
        };
        _controls.Player.NavigateRight.performed += ctx =>
        {
            var navigateRightCommand = new NavigateRightCommand();
            navigateRightCommand.Execute(playerContext);
        };
    }

    public void Execute()
    {
        //Debug.Log("Player's Turn Ongoing");
    }
    public void Exit()
    {
        Debug.Log("Player's Turn Ended");
        _controls.Disable();
    }
}
