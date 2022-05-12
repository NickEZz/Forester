using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageScript : MonoBehaviour
{
    public static StorageScript Instance { get; private set; }

    public int currentSector;

    public float money;
    public float[] wood; // 0 = spruce, 1 = pine, 2 = birch
    public int[] saplings;

    public float totalWood;

    public float totalWorkingPower;
    public float offlineEarningMultiplier;

    public int currentAxeUpgrade;
    public int currentSawUpgrade;
    public int currentChainsawUpgrade;

    public GameObject[] treeTypes;
    public Sapling[] saplingTypes;
    public Building[] buildingTypes;
    public Tool[] axes;
    public Tool[] saws;
    public Tool chainsaw;

    public List<GameObject> buildingsInGame;
    public List<GameObject> treesInGame;

    public List<TreeSaveData> trees;

    public List<AreaSaveData> areas;

    public List<BuildingSaveData> buildings;

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

        Application.targetFrameRate = 144;
        QualitySettings.vSyncCount = 0;
    }

    private void Update()
    {
        totalWood = wood[0] + wood[1] + wood[2];
    }
}
