using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public interface IGameState
{
    void Enter();
    void Execute();
    void Exit();
}


[System.Serializable]
public enum PlayerType
{
    Player,
    AI
}
public interface IContext
{
}
[System.Serializable]
public class PlayerContext : IContext
{
    public PlayerType playerType;
    public string playerName;
    public PlayerBase playerBase;
    public CardGameManager cardGameManager;
}

/// <summary>
/// CardGameManager is the main game manager that controls the state transitions of each turn, for example, the player's turn, the enemy's turn, etc.
/// </summary>
public class CardGameManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerContext> players = new List<PlayerContext>();
    private Queue<PlayerContext> turnQueue = new Queue<PlayerContext>();
    [SerializeField]
    private int turnCount = 0;
    [SerializeField]
    private PlayerContext currentPlayerInfo;
    [SerializeField]
    private PlayZone playZone;
    void Awake()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].cardGameManager = this;
        }
        SetState(new EmptyState());
    }
    void Start()
    {
        turnCount = 0;

        if (players.Count > 0)
        {
            // Initialize turn queue
            foreach (var player in players)
            {
                turnQueue.Enqueue(player);
            }
            AdvanceTurn();
        }
        else
        {
            Debug.LogError("Not enough players defined in CardGameManager.");
        }
    }

    private IGameState currentState;
    public void SetState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void EndTurn()
    {
        //Determine the winner
        PunishPlayer(turnQueue.Peek(), currentPlayerInfo);
    }

    public void AdvanceTurn()
    {
        playZone.AddCardsIntoDeck();
        playZone.LastCardInfo = null;
        turnCount++;
        UnityAction callback = () =>
        {
            foreach (var player in players)
            {
                player.playerBase.DrawCards(3);
            }
            AdvanceRound();
        };
        StartCoroutine(WaitForSeconds(callback, 1f));

    }
    public void PunishPlayer(PlayerContext winner, PlayerContext loser)
    {
        Debug.Log("Winner: " + winner.playerBase.name + " Loser: " + loser.playerBase.name);
        winner.playerBase.PunishOpponent(loser.playerBase);
    }
    public void AdvanceRound()
    {
        // Rotate to the next player in the queue
        PlayerContext currentPlayer = turnQueue.Dequeue();
        currentPlayerInfo = currentPlayer;
        turnQueue.Enqueue(currentPlayer);
        // Create a state for the current player
        IGameState playerTurnState = StateFactory.CreateState(this, currentPlayer);
        SetState(playerTurnState);
    }

    public IEnumerator WaitForSeconds(UnityAction callback, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        callback.Invoke();
    }
}

public static class StateFactory
{
    public static IGameState CreateState(CardGameManager gameManager, PlayerContext player)
    {
        if (player.playerType == PlayerType.Player)
        {
            return new PlayerTurnState(gameManager, player);
        }
        else
        {
            return new AITurnState(gameManager, player);
        }
    }
}