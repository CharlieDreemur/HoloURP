using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
public interface IGameState
{
    void Enter();
    void DrawOpponent(PlayerBase opponent) { }
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
    public bool isDrawOpponent = false;
}

/// <summary>
/// CardGameManager is the main game manager that controls the state transitions of each turn, for example, the player's turn, the enemy's turn, etc.
/// </summary>
public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance;
    public System.Random RNG = new System.Random();
    [SerializeField]
    private int perTurnDrawCount = 2;
    [SerializeField]
    private List<PlayerContext> players = new List<PlayerContext>();
    [SerializeField] private List<PlayerContext> turnQueueList = new List<PlayerContext>();

    private Queue<PlayerContext> turnQueue = new Queue<PlayerContext>();
    [SerializeField]
    private int turnCount = 0;
    [SerializeField]
    private PlayerContext currentPlayerInfo;
    [SerializeField]
    private PlayZone playZone;
    [SerializeField]
    private IGameState currentState;
    public PlayerTurnState playerTurnState;
    public AITurnState aiTurnState;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        for (int i = 0; i < players.Count; i++)
        {
            players[i].cardGameManager = this;
        }
        playerTurnState = new PlayerTurnState(this, players[0]);
        aiTurnState = new AITurnState(this, players[1]);
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
                turnQueueList.Add(player);
            }
            AdvanceRound();
        }
        else
        {
            Debug.LogError("Not enough players defined in CardGameManager.");
        }
    }


    public void SetState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void EndTurn()
    {

        currentState.Exit();
        //Determine the winner
        PunishPlayer(currentPlayerInfo, turnQueue.Peek());
    }
    public void Reset()
    {
        currentPlayerInfo.isDrawOpponent = false;
        playZone.AddCardsIntoDeck();
        playZone.LastCardInfo = null;
        foreach (var player in players)
        {
            player.playerBase.Clear();
        }
        UnityAction callback = () =>
        {

            AdvanceRound();
        };
        StartCoroutine(WaitForSeconds(callback, 1f));
    }
    public void AdvanceTurn()
    {
        currentPlayerInfo.isDrawOpponent = false;
        playZone.AddCardsIntoDeck();
        playZone.LastCardInfo = null;
        UnityAction callback = () =>
        {
            if (turnCount > 0)
            {
                foreach (var player in players)
                {
                    //only draw if card <=3
                    if (player.playerBase.HandCards.Count <= 3)
                    {
                        player.playerBase.DrawCards(perTurnDrawCount);
                    }
                }
            }
            AdvanceRound();
        };
        StartCoroutine(WaitForSeconds(callback, 1f));
        turnCount++;
    }
    public void PunishPlayer(PlayerContext winner, PlayerContext loser)
    {
        winner.isDrawOpponent = true;
        IGameState playerTurnState = StateFactory.CreateState(this, winner);
        playerTurnState.DrawOpponent(loser.playerBase);
    }
    public void AdvanceRound()
    {
        // Rotate to the next player in the queue
        PlayerContext currentPlayer = turnQueue.Dequeue();
        turnQueueList.Remove(currentPlayer);
        currentPlayerInfo = currentPlayer;
        turnQueue.Enqueue(currentPlayer);
        turnQueueList.Add(currentPlayer);
        // Create a state for the current player
        IGameState playerTurnState = StateFactory.CreateState(this, currentPlayer);
        SetState(playerTurnState);
    }
    private IGameState GetTurnState(PlayerContext player)
    {
        return player.playerType == PlayerType.Player ? playerTurnState : aiTurnState;
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
        return player.playerType == PlayerType.Player 
            ? gameManager.playerTurnState 
            : gameManager.aiTurnState;
    }
}