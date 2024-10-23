
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    public int Health = 3;
    [SerializeField]
    public List<ICard> HandCards = new List<ICard>();
    
}
