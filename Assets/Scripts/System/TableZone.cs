using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// TableZone represents the area on the table where cards are placed after being played.
/// </summary>
public class TableZone : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cardsOnTable = new List<GameObject>();

    // Add a card to the table
    public void AddCardToTable(GameObject card)
    {
        cardsOnTable.Add(card);
        Debug.Log("Card added to the table.");
    }

    // Remove all cards from the table and return them
    public List<GameObject> ClearTable()
    {
        List<GameObject> clearedCards = new List<GameObject>(cardsOnTable);
        cardsOnTable.Clear();
        return clearedCards;
    }
}
