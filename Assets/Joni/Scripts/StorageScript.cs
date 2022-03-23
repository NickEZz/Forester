using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageScript : MonoBehaviour
{
    public static StorageScript Instance { get; private set; }

    public int currentSector;

    public float money;
    public float[] wood; // 0 = spruce, 1 = pine, 2 = birch

    public float totalWood;

    SaveData saveData;

    public GameObject[] trees;

    public int[] saplings;

    public List<GameObject> buildingsInGame;
    public List<GameObject> treesInGame;

    public List<Area> areas;

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

        saveData = new SaveData();
    }

    private void Update()
    {
        totalWood = wood[0] + wood[1] + wood[2];
    }
}
