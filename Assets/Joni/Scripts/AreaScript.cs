using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScript : MonoBehaviour
{
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

    [SerializeField] bool working;

    [SerializeField] float timer;

    [SerializeField] Material grassMat;
    [SerializeField] Material highlightedMat;

    Renderer currentMaterial;

    [SerializeField] Vector3 size;

    [SerializeField] LayerMask[] gridPartMasks;

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
            if (!setup) // Jos aluetta ei ole vielä setupattu
            {
                GetComponent<MeshCollider>().enabled = true; // antaa alueelle colliderin
                gameObject.layer = 8; // vaihtaa layerin "Buyable Area" to "Ground"
                currentMaterial.material = grassMat; // Vaihtaa maan värin
                buildScript.gridList.Add(grid); // ja lisää alueen gridin buildscript grid listaan

                setup = true; // Laittaa setup boolin true että peli ei toista näitä montaa kertaa
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
        }

        if (working) // Jos alueella on rakennuksia ja puita on enemmän kuin 5
        {
            timer -= Time.deltaTime; // Laskee yhden sekunnin

            if (timer <= 0)
            {
                foreach (var tree in treesInArea) // Jonka jälkeen käy läpi puut listasta
                {
                    TreeScript treeScript = tree.GetComponent<TreeScript>();
                    if (treeScript.adultTree) // Ottaa ekan puun joka on jo kasvanut
                    {
                        treeScript.ChopTree(workingPower / 2f, false); // Ja vähentää siltä puulta hp:ta
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

        if (gameObject.layer == 10) // Jos aluetta ei ole vielä ostettu
        {
            // Etsii gridin osia, jotka eivät ole pois käytöstä, ja poistaa ne käytöstä
            Collider[] gridParts = Physics.OverlapBox(transform.position, size, Quaternion.identity, gridPartMasks[0]);
            for (int i = 0; i < gridParts.Length; i++)
            {
                gridParts[i].gameObject.GetComponent<MeshFilter>().mesh = null;
                gridParts[i].gameObject.layer = 12;
            }
        }
        else if (gameObject.layer == 8) // Jos alue on ostettu
        {
            // Etsii gridin osia, jotka ovat pois käytöstä, ja laittaa ne takaisin päälle
            Collider[] gridParts = Physics.OverlapBox(transform.position, size, Quaternion.identity, gridPartMasks[1]);
            for (int i = 0; i < gridParts.Length; i++)
            {
                gridParts[i].gameObject.GetComponent<MeshFilter>().mesh = planeMesh;
                gridParts[i].gameObject.layer = 11;
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

    public void SetAreaStats(int _sector, float _price, BuildScript _buildScript)
    {
        sector = _sector;
        price = _price;
        buildScript = _buildScript;
    }
}
