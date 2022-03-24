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
    [SerializeField] MapManager mapManager;

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
        */
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGameData(); 
        }
       
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

        StorageScript.Instance.trees.Clear();

        for (int i = 0; i < StorageScript.Instance.treesInGame.Count; i++)
        {
            TreeScript treeScript = StorageScript.Instance.treesInGame[i].GetComponent<TreeScript>();
            Tree tree = new Tree(treeScript.treeType,
                treeScript.hp, 
                treeScript.adultTree, 
                treeScript.treeHeight,
                new CustomVector(treeScript.transform.position.x, treeScript.transform.position.y, treeScript.transform.position.z), 
                treeScript.transform.localScale.y, 
                treeScript.transform.rotation.y);

            StorageScript.Instance.trees.Add(tree);
        }

        saveData.trees = StorageScript.Instance.trees;

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

            StorageScript.Instance.areas = saveData.areas;

            mapManager.CreateMap(true);

            for (int i = 0; i < saveData.trees.Count; i++)
            {
                GameObject newTree = Instantiate(StorageScript.Instance.treeTypes[saveData.trees[i].treeType], new Vector3(saveData.trees[i].position.x, saveData.trees[i].position.y, saveData.trees[i].position.z), Quaternion.Euler(0, saveData.trees[i].yRotation, 0));
                TreeScript newTreeScript = newTree.GetComponent<TreeScript>();
                newTreeScript.hp = saveData.trees[i].hp;
                newTreeScript.adultTree = saveData.trees[i].adultTree;
                newTreeScript.treeHeight = saveData.trees[i].treeHeight;
                newTree.transform.localScale = new Vector3(saveData.trees[i].scale, saveData.trees[i].scale, saveData.trees[i].scale);

                StorageScript.Instance.treesInGame.Add(newTree);
            }

            Debug.Log("Loaded game");
        }
        else
        {
            mapManager.CreateMap(false);
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

    public List<Area> areas;
    public List<Tree> trees;
}
