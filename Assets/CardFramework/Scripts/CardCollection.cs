using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "CardCollection", menuName = "Card Game/Card Collection")]
public class CardCollection : ScriptableObject
{
    public CardData[] cards;
}

