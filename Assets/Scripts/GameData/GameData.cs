using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}

public class GameData : MonoBehaviour
{
    public static GameData instance;
    public SaveData saveData;
    public int totalLevel;

    public bool firstSplashLaunch;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();
        firstSplashLaunch = true;
    }

    public void Save()
    {
        //Create a binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter();

        //Create a route from the program to the file
        FileStream file = File.Create(Application.persistentDataPath + "/player.dat");

        //Create a copy of save data
        SaveData data = new SaveData();
        data = saveData;

        //Actually save the data in the file
        formatter.Serialize(file, data);

        //Close the data stream
        file.Close();

        // Debug.Log("Saved");
    }

    public void Load()
    {
        totalLevel = GetLevelsNumber();
        // Check if the save game file exists
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            //Create a Binary Formatter
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            // Debug.Log("Loaded");
        }
        else
        {
        saveData = new SaveData();
        saveData.isActive = new bool[totalLevel];
        saveData.stars = new int[totalLevel];
        saveData.highScores = new int[totalLevel];
        saveData.isActive[0] = true;
        }
    }

    public void UpdateHighestScore(int score)
    {
        if (score > saveData.highScores[GameManager.instance.level])
        {
            saveData.highScores[GameManager.instance.level] = score;
            Save();
        }
    }

    public void UpdateLevelStar(int stars)
    {
        if (stars > saveData.stars[GameManager.instance.level])
        {
            saveData.stars[GameManager.instance.level] = stars;
            Save();
        }
    }

    private int GetLevelsNumber() => Resources.LoadAll<World>("World")[0].levels.Length;

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }
}
