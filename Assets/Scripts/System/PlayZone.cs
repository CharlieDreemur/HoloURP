using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// PlayZone represents the area on the table where cards are placed after being played.
/// </summary>
public class PlayZone : MonoBehaviour
{
    [SerializeField]
    private HandZone _playerStats;
    [SerializeField]
    private List<GameObject> cardModels = new List<GameObject>();
    void Awake(){
        _playerStats.PlayCardEvent.AddListener(AddCardToTable);
    }

    // Add a card to the table
    public void AddCardToTable(List<CardBase> cards)
    {
        foreach (CardBase card in cards)
        {
            
        }
    }


    // Remove all cards from the table and return them
    public List<GameObject> ClearTable()
    {
        List<GameObject> clearedCards = new List<GameObject>(cardModels);
        cardModels.Clear();
        return clearedCards;
    }
}
