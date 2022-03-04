using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageScript : MonoBehaviour
{
    public static StorageScript Instance { get; private set; }

    public int currentSector;

    public float money;
    public float spruceWood;
    public float pineWood;
    public float birchWood;
    public float wood;

    public GameObject[] trees;
    public Mesh[] treeMeshes;

    public int spruceSaplings;
    public int pineSaplings;
    public int birchSaplings;

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
        wood = spruceWood + birchWood + pineWood;
    }
}
