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

    public GameObject[] trees;

    public int[] saplings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        totalWood = wood[0] + wood[1] + wood[2];
    }
}
