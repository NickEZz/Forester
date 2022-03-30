using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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

    Fade fade;

    //public SaveData saveData;

    private void Awake()
    {
        fade = GetComponent<Fade>();

        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Application.persistentDataPath + "/save.dat"; // Pitää muuttaa jos aikoo tehdä webgl buildin

        LoadGameData(); // Kun peli käynnistyy, lataa tallennetut tiedot

        InvokeRepeating("SaveGameData", autosaveInterval, autosaveInterval); // Autosave
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

        SaveData saveData = new SaveData(); // Tekee uuden savedata variablen, jonne se tallentaa mm. rahan, puun ja taimien määrät

        saveData.wood = StorageScript.Instance.wood;
        saveData.money = StorageScript.Instance.money;
        saveData.currentSector = StorageScript.Instance.currentSector;
        saveData.saplings = StorageScript.Instance.saplings;

        saveData.currentAxeUpgrade = StorageScript.Instance.currentAxeUpgrade; // Tallentaa myös työkalujen päivitykset
        saveData.currentSawUpgrade = StorageScript.Instance.currentSawUpgrade;

        saveData.areas = StorageScript.Instance.areas; // Tallentaa kaikki alueet, että peli muistaa minkä alueen pelaaja on ostanut.

        StorageScript.Instance.trees.Clear(); // Tyhjentää trees listan edellisistä puista

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

        saveData.trees = StorageScript.Instance.trees; // Kun puiden tiedot on tallennettu, tiedot siirretään savedataan

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

        if (File.Exists(savePath)) // Jos pelaajalla on save.dat tiedosto eli pelaaja on pelannut peliä ennenkin
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter(); // Ottaa savedatan tiedostot save.dat tiedostosta
            FileStream file = File.Open(savePath, FileMode.Open);
            SaveData saveData = (SaveData)binaryFormatter.Deserialize(file);
            file.Close();

            StorageScript.Instance.money = saveData.money; // Jonka jälkeen kopioi kaikki savedatan tiedot peliin, mm. rahan, puun ja taimien määrä
            StorageScript.Instance.wood = saveData.wood;
            StorageScript.Instance.currentSector = saveData.currentSector;
            StorageScript.Instance.saplings = saveData.saplings;

            StorageScript.Instance.currentAxeUpgrade = saveData.currentAxeUpgrade; // Ja työkalujen päivitykset
            StorageScript.Instance.currentSawUpgrade = saveData.currentSawUpgrade;

            StorageScript.Instance.areas = saveData.areas; // Myös tarkistaa alueiden tiedot että onko pelaaja ostanut alueita

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

            Debug.Log("Loaded game");
        }
        else // Jos pelaajalla ei ole save.dat tiedostoa, eli peli alkaa alusta/pelaaja pelaa ensimmäistä kertaa
        {
            mapManager.CreateMap(false); 
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

    public List<AreaSaveData> areas;
    public List<TreeSaveData> trees;
    public List<BuildingSaveData> buildings;
}
