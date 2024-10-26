using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// PlayZone represents the area on the table where cards are placed after being played.
/// </summary>
public class PlayZoneVisual : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cardModels = new List<GameObject>();

    // Add a card to the table
    public void AddCardToTable(GameObject card)
    {
        cardModels.Add(card);
        Debug.Log("Card added to the table.");
    }

    // Remove all cards from the table and return them
    public List<GameObject> ClearTable()
    {
        List<GameObject> clearedCards = new List<GameObject>(cardModels);
        cardModels.Clear();
        return clearedCards;
    }
}
