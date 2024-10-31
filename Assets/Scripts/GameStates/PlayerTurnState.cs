using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTurnState : IGameState
{
    private CardGameManager _cardGameManager;
    private PlayerContext playerInfo;
    private CardPlayer player => (CardPlayer)playerInfo.playerBase;
    private InputControls _controls;
    private readonly DrawCardCommand _drawCardCommand = new DrawCardCommand();
    private readonly PlayCardCommand _playCardCommand = new PlayCardCommand();
    private readonly EndTurnCommand _endTurnCommand = new EndTurnCommand();
    private readonly HideShowCardCommand _hideShowCardCommand = new HideShowCardCommand();
    private readonly NavigateLeftCommand _navigateLeftCommand = new NavigateLeftCommand();
    private readonly NavigateRightCommand _navigateRightCommand = new NavigateRightCommand();

    public PlayerTurnState(CardGameManager cardGameManager, PlayerContext playerInfo)
    {
        this._cardGameManager = cardGameManager;
        this.playerInfo = playerInfo;
        _controls = new InputControls();
        InitKeyCommandMap();

    }

    public void Enter()
    {
        UIManager.Instance.ShowTurnText();
        //wait for 1 s
        _cardGameManager.StartCoroutine(_cardGameManager.WaitForSeconds(() =>
        {
            _controls.Enable();
            player.handZoneVisual.ShowHand();
        }, 1f));
        _cardGameManager.StartCoroutine(_cardGameManager.WaitForSeconds(() => HintLose(), 6f));
    }

    public void PunishOpponent(PlayerBase opponent)
    {

    }
    private bool HintLose()
    {
        if (player.playZone.LastCardInfo == null)
        {
            return false;
        }
        if (player.HasLargerCard(player.playZone.LastCardInfo.card))
        {
            return false;
        }
        else
        {
            UIManager.Instance.ShowMessage("Hold F to end your turn");
            return true;
        }
    }
    public void InitKeyCommandMap()
    {
        _controls.Player.DrawCard.performed += ctx => _drawCardCommand.Execute(playerInfo);
        _controls.Player.PlayCard.performed += ctx => _playCardCommand.Execute(playerInfo);
        _controls.Player.EndTurn.performed += ctx => _endTurnCommand.Execute(playerInfo);
        _controls.Player.HideHandZone.performed += ctx => _hideShowCardCommand.Execute(playerInfo);
        _controls.Player.NavigateLeft.performed += ctx => _navigateLeftCommand.Execute(playerInfo);
        _controls.Player.NavigateRight.performed += ctx => _navigateRightCommand.Execute(playerInfo);
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
