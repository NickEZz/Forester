using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageScript : MonoBehaviour
{
    public static StorageScript Instance { get; private set; }

    public float money;
    public float wood;

    public float workingPower;

    public GameObject[] trees;
    public Mesh[] treeMeshes;
    public int spruceSaplings;

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
}
