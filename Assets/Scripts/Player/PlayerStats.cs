
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    public int Health = 3;
    [SerializeField]
    public List<CardBase> HandCards = new List<CardBase>();
    
}
