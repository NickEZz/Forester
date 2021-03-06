using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    protected string savePath;

    public bool saving;
    bool loading;
    bool reset = false;

    [SerializeField] GameObject savingIcon;
    [SerializeField] TextMeshProUGUI savingIconText;

    [SerializeField] float autosaveInterval;

    [SerializeField] ToolScript toolScript;
    [SerializeField] MapManager mapManager;
    [SerializeField] TutorialScript tutorialScript;

    Fade fade;

    float totalWoodEarned;
    StartupAlert alert;

    //public SaveData saveData;

    private void Awake()
    {
        fade = GetComponent<Fade>();
        alert = GetComponent<StartupAlert>();

        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Application.persistentDataPath + "/save.dat"; // Pit?? muuttaa jos aikoo tehd? webgl buildin

        try
        {
            LoadGameData(); // Kun peli k?ynnistyy, lataa tallennetut tiedot
        }
        catch (System.Exception)
        {
            print("Game data corrupted.");
            File.Delete(savePath);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        

        InvokeRepeating("SaveGameData", autosaveInterval, autosaveInterval); // Autosave
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveGameData(); // Ctrl + s tallentaa pelin
            }
        }

        if (reset && !fade.fading)
        {
            File.Delete(savePath);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SaveGameData() 
    {
        saving = true;
        savingIconText.text = "Saving...";
        savingIcon.SetActive(true);

        SaveData saveData = new SaveData(); // Tekee uuden savedata variablen, jonne se tallentaa mm. rahan, puun ja taimien m??r?t

        saveData.wood = StorageScript.Instance.wood;
        saveData.money = StorageScript.Instance.money;
        saveData.currentSector = StorageScript.Instance.currentSector;
        saveData.saplings = StorageScript.Instance.saplings;

        saveData.currentAxeUpgrade = StorageScript.Instance.currentAxeUpgrade; // Tallentaa my?s ty?kalujen p?ivitykset
        saveData.currentSawUpgrade = StorageScript.Instance.currentSawUpgrade;
        saveData.currentChainsawUpgrade = StorageScript.Instance.currentChainsawUpgrade;

        saveData.areas = StorageScript.Instance.areas; // Tallentaa kaikki alueet, ett? peli muistaa mink? alueen pelaaja on ostanut.
        saveData.sectorsOwned = mapManager.sectorsOwned;

        StorageScript.Instance.trees.Clear(); // Tyhjent?? trees listan edellisist? puista

        for (int i = 0; i < StorageScript.Instance.treesInGame.Count; i++) // Ottaa kaikki spawnatut puut ja tallentaa niiden tietoja toiseen listaan
        {
            TreeScript treeScript = StorageScript.Instance.treesInGame[i].GetComponent<TreeScript>();
            TreeSaveData tree = new TreeSaveData(treeScript.treeType,
                treeScript.hp, 
                treeScript.adultTree, 
                treeScript.treeHeight,
                new CustomVector(treeScript.transform.position.x, treeScript.transform.position.y, treeScript.transform.position.z), 
                treeScript.transform.localScale.y, 
                treeScript.transform.eulerAngles.y);

            StorageScript.Instance.trees.Add(tree);
        }

        saveData.trees = StorageScript.Instance.trees; // Kun puiden tiedot on tallennettu, tiedot siirret??n savedataan

        StorageScript.Instance.buildings.Clear();

        for (int i = 0; i < StorageScript.Instance.buildingsInGame.Count; i++)
        {
            HouseScript houseScript = StorageScript.Instance.buildingsInGame[i].GetComponent<HouseScript>();
            BuildingSaveData house = new BuildingSaveData(houseScript.buildingLevel, 
                houseScript.buildTime,
                houseScript.chosenColor,
                new CustomVector(houseScript.transform.position.x, houseScript.transform.position.y, houseScript.transform.position.z),
                houseScript.transform.eulerAngles.y);

            StorageScript.Instance.buildings.Add(house);
        }

        saveData.buildings = StorageScript.Instance.buildings;

        DateTime dateTime = DateTime.Now;
        saveData.quitTimeString = dateTime.ToString();

        BinaryFormatter binaryFormatter = new BinaryFormatter(); // Vie savedatan tiedot save.dat tiedostoon
        FileStream file = File.Create(savePath);
        binaryFormatter.Serialize(file, saveData);
        file.Close();

        saving = false;
        StartCoroutine(SaveIcon());
        Debug.Log("Saved game");
    }

    public void LoadGameData()
    {
        loading = true;

        if (File.Exists(savePath)) // Jos pelaajalla on save.dat tiedosto eli pelaaja on pelannut peli? ennenkin
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter(); // Ottaa savedatan tiedostot save.dat tiedostosta
            FileStream file = File.Open(savePath, FileMode.Open);
            SaveData saveData = (SaveData)binaryFormatter.Deserialize(file);
            file.Close();

            StorageScript.Instance.money = saveData.money; // Jonka j?lkeen kopioi kaikki savedatan tiedot peliin, mm. rahan, puun ja taimien m??r?
            StorageScript.Instance.wood = saveData.wood;
            StorageScript.Instance.currentSector = saveData.currentSector;
            StorageScript.Instance.saplings = saveData.saplings;

            StorageScript.Instance.currentAxeUpgrade = saveData.currentAxeUpgrade; // Ja ty?kalujen p?ivitykset
            StorageScript.Instance.currentSawUpgrade = saveData.currentSawUpgrade;
            StorageScript.Instance.currentChainsawUpgrade = saveData.currentChainsawUpgrade;

            StorageScript.Instance.areas = saveData.areas; // My?s tarkistaa alueiden tiedot ett? onko pelaaja ostanut alueita
            mapManager.sectorsOwned = saveData.sectorsOwned;
            
            mapManager.CreateMap(true);

            for (int i = 0; i < saveData.trees.Count; i++) // Ottaa edellisen pelikerran spawnatut puut ja spawnaa ne uudestaan ja antaa niille oikeat tiedot esim. hp, puun korkeus
            {
                GameObject newTree = Instantiate(StorageScript.Instance.treeTypes[saveData.trees[i].treeType], new Vector3(saveData.trees[i].position.x, saveData.trees[i].position.y, saveData.trees[i].position.z), Quaternion.Euler(0, saveData.trees[i].yRotation, 0));
                TreeScript newTreeScript = newTree.GetComponent<TreeScript>();
                newTreeScript.hp = saveData.trees[i].hp;
                newTreeScript.adultTree = saveData.trees[i].adultTree;
                newTreeScript.treeHeight = saveData.trees[i].treeHeight;
                newTree.transform.localScale = new Vector3(saveData.trees[i].scale, saveData.trees[i].scale, saveData.trees[i].scale);
            }

            for (int i = 0; i < saveData.buildings.Count; i++)
            {
                GameObject newBuilding = Instantiate(StorageScript.Instance.buildingTypes[saveData.buildings[i].buildingLevel].buildingPrefab, new Vector3(saveData.buildings[i].position.x, saveData.buildings[i].position.y, saveData.buildings[i].position.z), Quaternion.Euler(0, saveData.buildings[i].yRotation, 0));
                HouseScript newBuildingScript = newBuilding.GetComponent<HouseScript>();
                newBuildingScript.buildingLevel = saveData.buildings[i].buildingLevel;
                newBuildingScript.buildTime = saveData.buildings[i].timer;
                newBuildingScript.chosenColor = saveData.buildings[i].mat;
            }

            TimeSpan timeSpan = DateTime.Now - DateTime.Parse(saveData.quitTimeString);
            print("Offline for " + timeSpan.TotalSeconds + " seconds.");

            for (int i = 0; i < saveData.areas.Count; i++) // K?y kaikki alueet l?pi
            {
                if (saveData.areas[i].bought) // Tarkistaa onko ne ostettu
                {
                    if (saveData.areas[i].workingPower > 0) // Jos alue on ostettu ja alueella on my?s taloja
                    {
                        for (int j = 0; j < saveData.areas[i].treeTypesInArea.Length; j++) 
                        {
                            if (saveData.areas[i].treeTypesInArea[j] == true) // Tarkistaa mink?laisia puita alueella oli
                            {
                                float amount = 0;

                                switch (j) // Antaa pelaajalle puuta sen perusteella ett? mit? puita alueella oli ja my?s rajoittaa m??r?t ett? pelaaja ei saa liian isoja m??ri?
                                {
                                    case 0:
                                        amount = Mathf.Clamp(StorageScript.Instance.offlineEarningMultiplier * saveData.areas[i].workingPower * (float)timeSpan.TotalSeconds, 0, 180);
                                        print("Earned " + amount + " wood of type: " + j);
                                        break;
                                    case 1:
                                        amount = Mathf.Clamp(StorageScript.Instance.offlineEarningMultiplier * 0.66f * saveData.areas[i].workingPower * (float)timeSpan.TotalSeconds, 0, 120);
                                        print("Earned " + amount + " wood of type: " + j);
                                        break;
                                    case 2:
                                        amount = Mathf.Clamp(StorageScript.Instance.offlineEarningMultiplier * 0.33f * saveData.areas[i].workingPower * (float)timeSpan.TotalSeconds, 0, 60);
                                        print("Earned " + amount + " wood of type: " + j);
                                        break;
                                }

                                StorageScript.Instance.wood[j] += amount;
                                totalWoodEarned += amount;
                            }   
                        }
                    }
                }
            }

            if (totalWoodEarned > 0)
            {
                alert.ShowAlert(totalWoodEarned);
            }
            
            Debug.Log("Loaded game");
        }
        else // Jos pelaajalla ei ole save.dat tiedostoa, eli peli alkaa alusta/pelaaja pelaa ensimm?ist? kertaa
        {
            mapManager.CreateMap(false);
            tutorialScript.StartTutorial();
        }

        loading = false;
        fade.FadeIn();
    }

    public void ResetSaveData()
    {
        if (!reset)
        {
            fade.FadeOut();
            reset = true;
        }
    }

    IEnumerator SaveIcon()
    {
        savingIconText.text = "Saved";

        yield return new WaitForSeconds(1f);

        savingIcon.SetActive(false);
    }

    IEnumerator WaitForMapLoaded(List<TreeSaveData> trees)
    {
        while (true)
        {
            if (StorageScript.Instance.areas.Count == mapManager.width * mapManager.height)
            {
                for (int i = 0; i < trees.Count; i++) // Ottaa edellisen pelikerran spawnatut puut ja spawnaa ne uudestaan ja antaa niille oikeat tiedot esim. hp, puun korkeus
                {
                    GameObject newTree = Instantiate(StorageScript.Instance.treeTypes[trees[i].treeType], new Vector3(trees[i].position.x, trees[i].position.y, trees[i].position.z), Quaternion.Euler(0, trees[i].yRotation, 0));
                    TreeScript newTreeScript = newTree.GetComponent<TreeScript>();
                    newTreeScript.hp = trees[i].hp;
                    newTreeScript.adultTree = trees[i].adultTree;
                    newTreeScript.treeHeight = trees[i].treeHeight;
                    newTree.transform.localScale = new Vector3(trees[i].scale, trees[i].scale, trees[i].scale);
                }

                yield break;
            }
        }
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
    public int currentChainsawUpgrade;

    public List<AreaSaveData> areas;
    public List<TreeSaveData> trees;
    public List<BuildingSaveData> buildings;

    public int[] sectorsOwned;

    public string quitTimeString;
}
