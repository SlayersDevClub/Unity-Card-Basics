using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card Game/Card Data")]

public class CardData : ScriptableObject
{
    public string cardName, Description;
    public int FaceValue;
    public Texture cardImage;
}
