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

    [SerializeField] List<AreaScript> sector0Areas, sector1Areas, sector2Areas; 

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
                        SpawnLevelPart(true, 0, groundSize, sector0Areas, x, y, 0, rng);
                    }
                    else if (y < 3 && x < 3)
                    {
                        SpawnLevelPart(false, 0, groundSize, sector0Areas, x, y, sector0Cost, rng);
                    }
                    else if (y > 2 && y < 6 || x > 2 && x < 6)
                    {
                        if (x < 6 && y < 6) 
                        {
                            SpawnLevelPart(false, 1, groundSize, sector1Areas, x, y, sector1Cost, rng);
                        }
                    }
                    if (y > 5 || x > 5)
                    {
                        SpawnLevelPart(false, 2, groundSize, sector2Areas, x, y, sector2Cost, rng);
                    }
                }
            }
        }
        
    }

    /*
    public void UpdatePrices(int sector, List<AreaScript> areaList, AreaScript area, float originalCost)
    {
        areaList.Remove(area);
        for (int i = 0; i < areaList.Count; i++)
        {
            //int multiplier = sectorAreaAmounts[i] - areaList.Count;

            areaList[i].price = originalCost * (sectorsOwned[sector] / 2 * 1000);
            
        }
    }
    */

    void SpawnLevelPart(bool firstPart, int sector, float groundSize, List<AreaScript> areaList, float x, float y, float cost, System.Random rng)
    {
        if (firstPart)
        {
            GameObject center = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
            AreaScript newAreaScript = center.GetComponent<AreaScript>();
            newAreaScript.SetAreaStats(sector, 0, buildScript);
            newAreaScript.bought = true;
            StorageScript.Instance.currentSector = 0;
        }
        else
        {
            GameObject levelPart = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
            AreaScript newAreaScript = levelPart.GetComponent<AreaScript>();
            areaList.Add(newAreaScript);
            newAreaScript.SetAreaStats(sector, cost, buildScript);
        }
    }
}
