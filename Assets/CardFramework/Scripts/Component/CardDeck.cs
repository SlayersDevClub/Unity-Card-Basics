using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour 
{
	[SerializeField]
	private GameObject _cardPrefab;

	public Dictionary<string, string> tarotCardsNames = new Dictionary<string, string>();

	public Dictionary<string, string> tarotCardsDescriptions = new Dictionary<string, string>();
	
	public readonly List<Card> CardList =  new List<Card>();

	public void InstantiateDeck()
	{

		// Get all cards from the ScriptableObject
		List<CardData> allCards = BundleSingleton.Instance.GetAllCards();

		// Convert the list to an array if you need to shuffle it
		CardData[] cardArray = allCards.ToArray();

		// Shuffle the array
		ShuffleArray(cardArray);

		for (int i = 0; i < cardArray.Length; ++i)
		{
			// Get the current card from the shuffled array
			CardData cardData = cardArray[i];

			// Instantiate the card prefab
			GameObject cardInstance = (GameObject)Instantiate(_cardPrefab);
			Card card = cardInstance.GetComponent<Card>();

			// Set card properties based on the ScriptableObject data
			card.gameObject.name = cardData.cardName;
			//card.TexturePath = ""; // You can remove this if you're not using a texture path anymore
			//card.SourceAssetBundlePath = ""; // You can remove this since we're no longer using AssetBundles
			card.transform.position = new Vector3(0, 1, 0);
			card.FaceValue = cardData.FaceValue;
			card.Description = StringToDescriptionValue(cardData.cardName);

			// Assuming the card prefab has a method to set its visual representation:
			card.SetCardImage(cardData.cardImage);

			// Add the instantiated card to the card list
			CardList.Add(card);
		}
	}


	public static void ShuffleArray<T>(T[] arr) {
		for (int i = arr.Length - 1; i > 0; i--) {
			int r = Random.Range(0, i);
			T tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}
	}

	private string StringToNameValue(string input)
	{ 	
		string result = "";
		if (!tarotCardsNames.TryGetValue (input, out result))
			result = input;

		return result;
	}

	private string StringToDescriptionValue(string input)
	{
		string result = "";
		if (!tarotCardsDescriptions.TryGetValue (input, out result))
			result = input;
		return result;
	}

	private int StringToFaceValue(string input)
	{
		for (int i = 2; i < 11; ++i)
		{
			if (input.Contains(i.ToString()))
			{
				return i;
			}
		}
		if (input.Contains("jack") ||
			input.Contains("queen") ||
			input.Contains("king"))
		{
			return 10;
		}
		if (input.Contains("ace"))
		{
			return 11;
		}
		return 0;
	}
}
