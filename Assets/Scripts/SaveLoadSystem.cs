using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    public static void SaveData<T>(T data, string folderName, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath + "/" + fileName + ".json", json);
    }

    public static void LoadData<T>(T data, string folderName, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, folderName, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, data);
        }
        else
        {
            Debug.LogWarning($"File not found: {filePath}");
        }
    }

    public static void SaveTexture(NoiseData noiseData, Texture2D noiseTexture)
    {
        if (noiseTexture == null)
        {
            Debug.LogError("Texture to save is null.");
            return;
        }

        string directoryPath = Application.persistentDataPath + "/Textures/";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Encode the texture to PNG format
        byte[] textureBytes = noiseTexture.EncodeToPNG();

        // Specify the file path where you want to save the texture
        string filePath = directoryPath + noiseData.name + "Texture.png";

        // Write the texture bytes to a file
        File.WriteAllBytes(filePath, textureBytes);
    }

    public static Texture2D LoadTexture(NoiseData noiseData)
    {
        string filePath = Application.persistentDataPath + "/Textures/" + noiseData.name + "Texture.png";
        // Check if the file exists
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }

        // Read the bytes from the file
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);

        // Create a new Texture2D
        Texture2D loadedTexture = new Texture2D(2, 2); // Placeholder dimensions, will be overridden by LoadImage

        // Load the image data into the Texture2D
        loadedTexture.LoadImage(fileData); // This will auto-resize the texture to match the image dimensions

        return loadedTexture;

    }
}
