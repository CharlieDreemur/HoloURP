using System.Collections.Generic;
using UnityEngine;
public interface IGameState
{
    void Enter();
    void Execute();
    void Exit();
}

[System.Serializable]
public enum PlayerType{
    Player,
    AI
}
[System.Serializable]
public struct PlayerInfo
{
    public PlayerType playerType;
    public string playerName;
}
public interface IContext { }
/// <summary>
/// CardGameManager is the main game manager that controls the state transitions of each turn, for example, the player's turn, the enemy's turn, etc.
/// </summary>
public class CardGameManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerInfo> players = new List<PlayerInfo>();
    private Queue<PlayerInfo> turnQueue = new Queue<PlayerInfo>();
    [SerializeField]
    private int turnCount = 0;
    [SerializeField]
    private PlayerInfo currentPlayerInfo;
    [SerializeField]
    public GameContext playerContext;

    void Awake()
    {
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

    public void AdvanceTurn()
    {
        // Rotate to the next player in the queue
        PlayerInfo currentPlayer = turnQueue.Dequeue();
        currentPlayerInfo = currentPlayer;
        turnQueue.Enqueue(currentPlayer);
        // Create a state for the current player
        IGameState playerTurnState = StateFactory.CreateState(this, currentPlayer);
        SetState(playerTurnState);
        turnCount++;
    }
}

public static class StateFactory
{
    public static IGameState CreateState(CardGameManager gameManager, PlayerInfo player)
    {
        if (player.playerType == PlayerType.Player)
        {
            return new PlayerTurnState(gameManager);
        }
        else
        {
            return new AITurnState(gameManager);
        }
    }
}