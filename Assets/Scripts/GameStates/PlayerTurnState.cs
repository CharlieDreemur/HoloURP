using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public struct GameContext : IContext
{
    [SerializeField]
    public CardGameManager cardGameManager;
    [SerializeField]
    public CardPlayer player;
    [SerializeField]
    public AIPlayer aiPlayer;
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
    private GameContext gameContext;
    private InputControls _controls;
    private readonly DrawCardCommand _drawCardCommand = new DrawCardCommand();
    private readonly PlayCardCommand _playCardCommand = new PlayCardCommand();
    private readonly EndTurnCommand _endTurnCommand = new EndTurnCommand();
    private readonly HideShowCardCommand _hideShowCardCommand = new HideShowCardCommand();
    private readonly NavigateLeftCommand _navigateLeftCommand = new NavigateLeftCommand();
    private readonly NavigateRightCommand _navigateRightCommand = new NavigateRightCommand();

    public PlayerTurnState(CardGameManager cardGameManager)
    {
        this.cardGameManager = cardGameManager;
        gameContext = cardGameManager.playerContext;
        gameContext.cardGameManager = cardGameManager;
        _controls = new InputControls();
        InitKeyCommandMap();

    }

    public void Enter()
    {
        Debug.Log("Player's Turn Started");
        UIManager.Instance.ShowTurnText();
        //wait for 1 s
        cardGameManager.StartCoroutine(DelayInit(1f));
    }

    private IEnumerator DelayInit(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _controls.Enable();
        var drawCardCommand = new DrawCardCommand();
        drawCardCommand.Execute(gameContext);
        gameContext.handZoneVisual.ShowHand();
    }

    public void InitKeyCommandMap()
    {
        _controls.Player.DrawCard.performed += ctx => _drawCardCommand.Execute(gameContext);
        _controls.Player.PlayCard.performed += ctx => _playCardCommand.Execute(gameContext);
        _controls.Player.EndTurn.performed += ctx => _endTurnCommand.Execute(gameContext);
        _controls.Player.HideHandZone.performed += ctx => _hideShowCardCommand.Execute(gameContext);
        _controls.Player.NavigateLeft.performed += ctx => _navigateLeftCommand.Execute(gameContext);
        _controls.Player.NavigateRight.performed += ctx => _navigateRightCommand.Execute(gameContext);
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
