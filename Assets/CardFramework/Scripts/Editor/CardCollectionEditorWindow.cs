using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CardCollectionEditorWindow : EditorWindow
{
    private CardCollection cardCollection;
    private Vector2 scrollPos;

    [MenuItem("Window/Card Collection Editor")]
    public static void ShowWindow()
    {
        GetWindow<CardCollectionEditorWindow>("Card Collection Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Card Collection Editor", EditorStyles.boldLabel);

        cardCollection = (CardCollection)EditorGUILayout.ObjectField("Card Collection", cardCollection, typeof(CardCollection), false);

        if (cardCollection != null)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var card in cardCollection.cards)
            {
                DrawCardData(card);
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add New Card"))
            {
                CreateNewCard();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a Card Collection to edit.", MessageType.Info);
        }
    }

    private void DrawCardData(CardData card)
    {
        GUILayout.BeginVertical("box");
        card.cardName = EditorGUILayout.TextField("Card Name", card.cardName);
        card.Description = EditorGUILayout.TextField("Description", card.Description);
        card.FaceValue = EditorGUILayout.IntField("Face Value", card.FaceValue);
        card.cardImage = (Texture)EditorGUILayout.ObjectField("Card Image", card.cardImage, typeof(Texture), false);

        if (GUILayout.Button("Remove Card"))
        {
            RemoveCard(card);
        }
        GUILayout.EndVertical();
    }

    private void CreateNewCard()
    {
        CardData newCard = ScriptableObject.CreateInstance<CardData>();
        int ran = Random.Range(0,10000);
        newCard.cardName = "New Card" + ran.ToString();
        AssetDatabase.CreateAsset(newCard, $"Assets/NewCard" + ran.ToString() + ".asset");
        AssetDatabase.SaveAssets();

        // Add the new card to the collection
        var cardList = new List<CardData>(cardCollection.cards);
        cardList.Add(newCard);
        cardCollection.cards = cardList.ToArray();

        EditorUtility.SetDirty(cardCollection);
        AssetDatabase.SaveAssets();
    }

    private void RemoveCard(CardData card)
    {
        var cardList = new List<CardData>(cardCollection.cards);
        cardList.Remove(card);
        cardCollection.cards = cardList.ToArray();

        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(card));
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(cardCollection);
    }
}
