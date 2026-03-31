using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/player.save";
    private static string encryptionKey = "NotSupposedToReadThis100217";

    public static void Save(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        string encrypted = EncryptDecrypt(json);
        File.WriteAllText(savePath, encrypted);

        Debug.Log("Saved successfully!");
    }

    public static SaveData Load()
    {
        if(!File.Exists(savePath)) return null;

        string encrypted = File.ReadAllText(savePath);
        string json = EncryptDecrypt(encrypted);

        return JsonUtility.FromJson<SaveData>(json);
    }

    public static string EncryptDecrypt(string data)
    {
        char[] key = encryptionKey.ToCharArray();
        char[] input = data.ToCharArray();

        for (int i = 0; i < input.Length; i++)
        {
            input[i] ^= key[i % key.Length];
        }

        return new string(input);
    }
}
