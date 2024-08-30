using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BundleSingleton : Singleton<BundleSingleton>
{
	public CardCollection cardCollection;

	private void Awake()
	{
		// Load the CardCollection ScriptableObject from Resources or assign it manually in the Inspector
		cardCollection = Resources.Load<CardCollection>("CardCollection");

		if (cardCollection == null)
		{
			Debug.LogError("CardCollection not found in Resources!");
		}
	}

	public Card GetCard(string cardName)
	{
		if (cardCollection != null)
		{
			foreach (var card in cardCollection.cards)
			{
				if (card.cardName == cardName)
				{
					//return card;
				}
			}
		}
		return null;
	}

	public List<CardData> GetAllCards()
	{
		return cardCollection != null ? new List<CardData>(cardCollection.cards) : new List<CardData>();
	}
}

