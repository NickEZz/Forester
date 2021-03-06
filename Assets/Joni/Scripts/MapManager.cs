using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] int seed;

    public int width;
    public int height;

    [SerializeField] float sector0Cost, sector1Cost, sector2Cost;

    [SerializeField] List<AreaScript> sector0Areas, sector1Areas, sector2Areas; 

    [SerializeField] GameObject[] groundPrefabs;

    [SerializeField] GameObject[] groundObjectPrefabs;
    [SerializeField] float objectSpawnChance;

    public int[] sectorsOwned = new int[3];
    public int[] sectorAreaAmounts = new int[3];

    [SerializeField] BuildScript buildScript;

    int i;

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(0, 1001);
    }

    public void CreateMap(bool mapExists)
    {
        System.Random rng = new System.Random(seed);

        // Laskee maa-palan koon, jotta maa-palat yhdistyy täydellisesti
        float groundSize = groundPrefabs[0].transform.localScale.x / 50;

        // Laskee offsetin, jotta maa-alueen keskiosa on 0x, 0z positionissa
        float offset = Mathf.Floor(width / 2) * 10;

        // Scripti generoi maa-palat rivi kerrallaan, vasemmalta oikealle jonka jälkeen se generoi seuraavan rivin edellisen yläpuolelle
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++) // en keksiny parempaa tapaa jakaa alueet eri ryhmiin
            {
                if (y == 0 && x == 0)
                {
                    SpawnLevelPart(true, 0, groundSize, sector0Areas, x, y, 0, rng, mapExists);
                }
                else if (y == 1 || x == 1)
                {
                    if (y == 1 && x < 2)
                    {
                        SpawnLevelPart(false, 1, groundSize, sector1Areas, x, y, sector1Cost, rng, mapExists);
                    }
                    else if (x == 1 && y < 2)
                    {
                        SpawnLevelPart(false, 1, groundSize, sector1Areas, x, y, sector1Cost, rng, mapExists);
                    }
                }
                if (y > 1 || x > 1)
                {
                    SpawnLevelPart(false, 2, groundSize, sector2Areas, x, y, sector2Cost, rng, mapExists);
                }


                /*
                else if (y < 3 && x < 3)
                {
                    SpawnLevelPart(false, 0, groundSize, sector0Areas, x, y, sector0Cost, rng, mapExists);
                }
                else if (y > 2 && y < 6 || x > 2 && x < 6)
                {
                    if (x < 6 && y < 6)
                    {
                        SpawnLevelPart(false, 1, groundSize, sector1Areas, x, y, sector1Cost, rng, mapExists);
                    }
                }
                if (y > 5 || x > 5)
                {
                    SpawnLevelPart(false, 2, groundSize, sector2Areas, x, y, sector2Cost, rng, mapExists);
                }
                */
            }
        }

        SpawnGroundObjects();
    }

    void SpawnLevelPart(bool firstPart, int sector, float groundSize, List<AreaScript> areaList, float x, float y, float cost, System.Random rng, bool mapExists)
    {
        if (firstPart)
        {
            GameObject center = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
            AreaScript newAreaScript = center.GetComponent<AreaScript>();
            newAreaScript.SetAreaStats(sector, 0, i, buildScript);
            newAreaScript.enabled = true;
            newAreaScript.bought = true;
            if (!mapExists)
            {
                StorageScript.Instance.currentSector = 0;

                AreaSaveData area = new AreaSaveData(newAreaScript.bought, newAreaScript.totalBuildingsInArea, newAreaScript.builtBuildingsInArea, /*newAreaScript.treesInArea,*/ newAreaScript.workingPower);

                StorageScript.Instance.areas.Add(area);
            }
        }
        else
        {
            GameObject levelPart = Instantiate(groundPrefabs[rng.Next(0, groundPrefabs.Length)], new Vector3(x * groundSize, 0, y * groundSize), Quaternion.Euler(-90f, 0f, 0f), gameObject.transform);
            AreaScript newAreaScript = levelPart.GetComponent<AreaScript>();
            areaList.Add(newAreaScript);
            newAreaScript.SetAreaStats(sector, cost, i, buildScript);
            if (mapExists)
            {
                newAreaScript.enabled = StorageScript.Instance.areas[i].bought;
                newAreaScript.bought = StorageScript.Instance.areas[i].bought;
                newAreaScript.totalBuildingsInArea = StorageScript.Instance.areas[i].totalBuildingsInArea;
                newAreaScript.builtBuildingsInArea = StorageScript.Instance.areas[i].builtBuildingsInArea;
                //newAreaScript.treesInArea = StorageScript.Instance.areas[i].treesInArea;
                newAreaScript.workingPower = StorageScript.Instance.areas[i].workingPower;
            }
            else
            {
                AreaSaveData area = new AreaSaveData(newAreaScript.bought, newAreaScript.totalBuildingsInArea, newAreaScript.builtBuildingsInArea, /*newAreaScript.treesInArea,*/ newAreaScript.workingPower);

                StorageScript.Instance.areas.Add(area);
            }
        }
        i++;
    }

    void SpawnGroundObjects()
    {
        System.Random rng = new System.Random(seed);

        float w = width * 10;
        float h = height * 10;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (rng.Next(0, 101) <= objectSpawnChance)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(x, 10, y), Vector3.down, out hit, 20f))
                    {
                        if (hit.collider.tag != "Structure")
                        {
                            Instantiate(groundObjectPrefabs[rng.Next(0, groundObjectPrefabs.Length)], hit.point, Quaternion.Euler(-90f, 0f, 0f));
                        }
                    }
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
