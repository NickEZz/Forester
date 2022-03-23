using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    protected string savePath;

    [SerializeField] float autosaveInterval;

    [SerializeField] ToolScript toolScript;

    //public SaveData saveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Application.persistentDataPath + "/save.dat";

        LoadGameData();

        InvokeRepeating("SaveGameData", autosaveInterval, autosaveInterval);
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveGameData();
        } 
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGameData(); 
        }
        */

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveGameData();
            }
        }
    }

    public void SaveGameData() 
    {
        SaveData saveData = new SaveData();

        saveData.wood = StorageScript.Instance.wood;
        saveData.money = StorageScript.Instance.money;
        saveData.currentSector = StorageScript.Instance.currentSector;
        saveData.saplings = StorageScript.Instance.saplings;

        saveData.currentAxeUpgrade = toolScript.currentAxeUpgrade;
        saveData.currentSawUpgrade = toolScript.currentSawUpgrade;

        saveData.areas = StorageScript.Instance.areas;
        //saveData.buildingsInGame = StorageScript.Instance.buildingsInGame;
        //saveData.treesInGame = StorageScript.Instance.treesInGame;

        Debug.Log("Saved game");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        binaryFormatter.Serialize(file, saveData);
        file.Close();
    }

    public void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            SaveData saveData = (SaveData)binaryFormatter.Deserialize(file);
            file.Close();

            StorageScript.Instance.money = saveData.money;
            StorageScript.Instance.wood = saveData.wood;
            StorageScript.Instance.currentSector = saveData.currentSector;
            StorageScript.Instance.saplings = saveData.saplings;

            toolScript.currentAxeUpgrade = saveData.currentAxeUpgrade;
            toolScript.currentSawUpgrade = saveData.currentSawUpgrade;

            Debug.Log("Loaded game");
        }
    }

    public void ResetSaveData()
    {
        File.Delete(savePath);
    }

    public void SaveSetting(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
}

[System.Serializable]
public class SaveData 
{
    public int currentSector;
    public float money;
    public float[] wood;
    public int[] saplings;

    public int currentAxeUpgrade;
    public int currentSawUpgrade;

    public bool mapExists;
    public List<Area> areas;

    //public List<GameObject> buildingsInGame;
    //public List<GameObject> treesInGame;

    //bool mapExists;

    //GameObject[] buildings;
    //GameObject[] trees;
}
