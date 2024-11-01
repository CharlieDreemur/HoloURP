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
        UIManager.Instance.holdCircle.gameObject.SetActive(true);
        //wait for 1 s
        _cardGameManager.StartCoroutine(_cardGameManager.WaitForSeconds(() =>
        {
            _controls.Player.Enable();
            player.handZoneVisual.ShowHand();
        }, 1f));
        HintLose();
    }

    public void DrawOpponent(PlayerBase opponent)
    {
        AudioManager.Instance.Play("roundlose");
        UIManager.Instance.ShowMessage("You Lose, draw one card from your opponent");
        _controls.Player.NavigateLeft.Enable();
        _controls.Player.NavigateRight.Enable();
        _controls.Player.PlayCard.Enable();
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
            UIManager.Instance.ShowMessage("You don't have any card that is equal or larger than the last card");
            _cardGameManager.StartCoroutine(_cardGameManager.WaitForSeconds(() => UIManager.Instance.ShowMessage("Hold F to end your turn"), 2f));
            return true;
        }
    }
    public void InitKeyCommandMap()
    {
        //_controls.Player.DrawCard.performed += ctx => _drawCardCommand.Execute(playerInfo);
        _controls.Player.PlayCard.performed += ctx => _playCardCommand.Execute(playerInfo);
        _controls.Player.EndTurn.performed += ctx => _endTurnCommand.Execute(playerInfo);
        _controls.Player.HideHandZone.performed += ctx => _hideShowCardCommand.Execute(playerInfo);
        _controls.Player.NavigateLeft.performed += ctx => _navigateLeftCommand.Execute(playerInfo);
        _controls.Player.NavigateRight.performed += ctx => _navigateRightCommand.Execute(playerInfo);
    }

    public void Exit()
    {
        Debug.Log("Player's Turn Ended");
        UIManager.Instance.EndTurn();

        player.handZoneVisual.HideHand();
        _controls.Player.Disable();
    }
}
