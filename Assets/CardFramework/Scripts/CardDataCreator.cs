using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class CardDataCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Card Data from Images")]
    public static void CreateCardDataFromImages()
    {
        // Specify the folder containing the images
        string folderPath = "Assets/CardFramework/AssetBundles/Cards";
        string[] imageFiles = Directory.GetFiles(folderPath, "*.png");

        foreach (string filePath in imageFiles)
        {
            // Get the file name without extension
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            // Extract face value from the file name using regex to find the first number
            int faceValue = 0;
            Match match = Regex.Match(fileName, @"\d+");
            if (match.Success)
            {
                faceValue = int.Parse(match.Value);
            }

            // Load the texture from the file
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);

            // Create a new CardData ScriptableObject
            CardData cardData = ScriptableObject.CreateInstance<CardData>();
            cardData.cardName = fileName;
            cardData.FaceValue = faceValue;
            cardData.cardImage = texture;

            // Save the ScriptableObject as an asset
            string assetPath = Path.Combine(folderPath, fileName + ".asset");
            AssetDatabase.CreateAsset(cardData, assetPath);
        }

        // Refresh the AssetDatabase to show the new assets
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
