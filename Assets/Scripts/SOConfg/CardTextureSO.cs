using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSpriteSO", menuName = "Scriptable Objects/CardSpriteSO")]
public class CardSpriteSO : ScriptableObject
{
    public List<Sprite> numberCardSprite;
    public Sprite bombCardSprite;
}
