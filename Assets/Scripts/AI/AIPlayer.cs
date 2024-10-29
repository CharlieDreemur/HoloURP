using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPlayer : PlayerBase
{
    public void PlayRandomCard()
    {
        if (HandCards.Count > 0)
        {
            int index = Random.Range(0, HandCards.Count);
            PlayCard(index);
        }
    }
}
