using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] int seed;  

    [SerializeField] bool mapExists;

    [SerializeField] int width;
    [SerializeField] int height;

    [SerializeField] float sector0Cost, sector1Cost, sector2Cost;

    [SerializeField] GameObject[] groundPrefabs;

    public int[] sectorsOwned = new int[3];
    public int[] sectorAreaAmounts = new int[3];

    [SerializeField] BuildScript buildScript;

    // Start is called before the first frame update
    void Start()
    {
        if (!mapExists)
        {
            System.Random rng = new System.Random(seed);

            // Laskee maa-palan koon, jotta maa-palat yhdistyy täydellisesti
            float groundSize = groundPrefabs[0].transform.localScale.x / 50;

            // Laskee offsetin, jotta maa-alueen keskiosa on 0x, 0z positionissa
            float offset = Mathf.Floor(width / 2) * 10;

            // Scripti generoi maa-palat rivi kerrallaan, vasemmalta oikealla jonka jälkeen se generoi seuraavan rivin edellisen yläpuolelle
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++) // en keksiny parempaa tapaa jakaa alueet eri ryhmiin
                {
                    if (y == 0 && x == 0)
                    {
                        GameObject center = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
                        AreaScript newAreaScript = center.GetComponent<AreaScript>();
                        newAreaScript.SetAreaStats(0, 0, buildScript);
                        newAreaScript.bought = true;
                        StorageScript.Instance.currentSector = 0;
                    }
                    else if (y < 3 && x < 3)
                    {
                        GameObject sector0 = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
                        AreaScript newAreaScript = sector0.GetComponent<AreaScript>();
                        newAreaScript.SetAreaStats(0, sector0Cost, buildScript);


                        //sector0.GetComponent<AreaScript>().sector = 0;
                        //sector0.GetComponent<AreaScript>().price = 100f;
                    }
                    else if (y > 2 && y < 6 || x > 2 && x < 6)
                    {
                        
                        if (x < 6 && y < 6) 
                        {
                            GameObject sector1 = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
                            AreaScript newAreaScript = sector1.GetComponent<AreaScript>();
                            newAreaScript.SetAreaStats(1, sector1Cost, buildScript);

                            //sector1.GetComponent<AreaScript>().sector = 1;
                            //sector1.GetComponent<AreaScript>().price = 200f;
                        }
                    }
                    if (y > 5 || x > 5)
                    {
                        GameObject sector2 = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
                        AreaScript newAreaScript = sector2.GetComponent<AreaScript>();
                        newAreaScript.SetAreaStats(2, sector2Cost, buildScript);

                        //sector2.GetComponent<AreaScript>().sector = 2;
                        //sector2.GetComponent<AreaScript>().price = 300f;
                    }

                    

                    /*
                    if (y == Mathf.Floor(height / 2) && x == Mathf.Floor(width / 2))
                    {
                        // Keskimmäinen maa-pala on valmiiksi ostettu
                        GameObject center = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize - offset, 0, y * groundSize - offset), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
                        center.GetComponent<AreaScript>().bought = true;
                    }
                    else
                    {
                        // Luo maapalan valitsemalla random maa-palan kaikista saatavilla olevista maa-paloista ja laittamalla sen maa-palan oikeaan paikkaan
                        Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize - offset, 0, y * groundSize - offset), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
                    }
                    */
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
