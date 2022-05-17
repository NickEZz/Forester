using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScript : MonoBehaviour
{
    public int areaId;
    
    public float price;
    public bool bought = false;
    public int sector;

    [SerializeField] GameObject grid;
    public BuildScript buildScript;

    public int totalBuildingsInArea;
    public int builtBuildingsInArea;
    public List<GameObject> treesInArea;

    bool setup = false;

    public float workingPower;

    public bool[] treeTypesInArea;

    [SerializeField] bool working;

    [SerializeField] float timer;

    [SerializeField] Material grassMat;
    [SerializeField] Material highlightedMat;

    Renderer currentMaterial;

    [SerializeField] Vector3 size;

    [SerializeField] LayerMask treeLayerMask;

    [SerializeField] Mesh planeMesh;

    MapManager mapManager;

    public bool focused;

    // Start is called before the first frame update
    void Start()
    {
        //mapManager = FindObjectOfType<MapManager>();
        currentMaterial = GetComponent<Renderer>();
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bought) // Kun pelaaja ostaa alueen, antaa alueelle colliderin
        {
            if (!setup) // Jos aluetta ei ole viel‰ setupattu
            {
                GetComponent<MeshCollider>().enabled = true; // antaa alueelle colliderin
                gameObject.layer = 8; // vaihtaa layerin "Buyable Area" to "Ground"
                currentMaterial.material = grassMat; // Vaihtaa maan v‰rin
                buildScript.gridList.Add(grid); // ja lis‰‰ alueen gridin buildscript grid listaan

                setup = true; // Laittaa setup boolin true ett‰ peli ei toista n‰it‰ montaa kertaa
            }

            if (builtBuildingsInArea > 0)
            {
                if (treesInArea.Count > 5)
                {
                    working = true;
                }
                else
                {
                    working = false;
                }
            }

            if (focused)
            {
                currentMaterial.material = highlightedMat;
            }
            else
            {
                currentMaterial.material = grassMat;
            }

            StorageScript.Instance.areas[areaId].treeTypesInArea = treeTypesInArea;

            if (working) // Jos alueella on rakennuksia ja puita on enemm‰n kuin 5
            {
                timer -= Time.deltaTime; // Laskee yhden sekunnin

                if (timer <= 0)
                {
                    foreach (var tree in treesInArea) // Jonka j‰lkeen k‰y l‰pi puut listasta
                    {
                        TreeScript treeScript = tree.GetComponent<TreeScript>();
                        if (treeScript.adultTree) // Ottaa ekan puun joka on jo kasvanut
                        {
                            treeScript.ChopTree(workingPower / 2f, false); // Ja v‰hent‰‰ silt‰ puulta hp:ta
                            if (treeScript.hp <= 0)
                            {
                                treesInArea.Remove(tree); // Jos puulla on 0 hp, poistaa sen listasta
                            }

                            timer = 1;

                            break; // Lopettaa foreach loopin koska se teki puuhun vahinkoa
                        }
                    }

                }
            }
        }
    }

    public bool BuyArea()
    {
        if (!bought)
        {
            if (StorageScript.Instance.money >= price)
            {
                bought = true;
                StorageScript.Instance.money -= price;

                //mapManager.UpdatePrices(StorageScript.Instance.currentSector, mapManager., mapManager.)

                // save areas

                this.enabled = true;
                StorageScript.Instance.areas[areaId].bought = true;
                SaveManager.Instance.SaveGameData();

                if (sector > StorageScript.Instance.currentSector)
                {
                    StorageScript.Instance.currentSector = sector;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public void SetAreaStats(int _sector, float _price, int _areaId, BuildScript _buildScript)
    {
        sector = _sector;
        price = _price;
        buildScript = _buildScript;
        areaId = _areaId;
    }
}

[System.Serializable]
public class AreaSaveData
{
    public bool bought;
    public int totalBuildingsInArea;
    public int builtBuildingsInArea;
    public float workingPower;
    public bool[] treeTypesInArea;

    public AreaSaveData(bool _bought, int _totalBuildingsInArea, int _builtBuildingsInArea, float _workingPower)
    {
        bought = _bought;
        totalBuildingsInArea = _totalBuildingsInArea;
        builtBuildingsInArea= _builtBuildingsInArea;
        workingPower = _workingPower;
    }
}
