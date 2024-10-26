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
        card.transform.SetParent(transform);
        cardModels.Add(card);
    }



    // Remove all cards from the table and return them
    public void ClearTable()
    {
        for(int i = 0; i < cardModels.Count; i++)
        {
            Destroy(cardModels[i]);
        }
    }

    
}
