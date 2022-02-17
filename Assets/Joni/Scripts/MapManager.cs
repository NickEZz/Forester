using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] int seed;  

    [SerializeField] bool mapExists;

    [SerializeField] int width;
    [SerializeField] int height;

    //[SerializeField] float groundSize;

    [SerializeField] GameObject[] groundPrefabs;


    // Start is called before the first frame update
    void Start()
    {
        if (!mapExists)
        {
            System.Random rng = new System.Random(seed);

            // Laskee maa-palan koon, jotta maa-palat yhdistyy täydellisesti
            float groundSize = groundPrefabs[0].transform.localScale.x / 5;

            // Laskee offsetin, jotta maa-alueen keskiosa on 0x, 0z positionissa
            float offset = Mathf.Floor(width / 2) * 10;

            // Scripti generoi maa-palat rivi kerrallaan, vasemmalta oikealla jonka jälkeen se generoi seuraavan rivin edellisen yläpuolelle
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Luo maapalan valitsemalla random maa-palan kaikista saatavilla olevista maa-paloista ja laittamalla sen maa-palan oikeaan paikkaan
                    Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize - offset, 0, y * groundSize - offset), Quaternion.Euler(-90f, 0f, 0f));
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
