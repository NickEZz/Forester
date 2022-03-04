using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScript : MonoBehaviour
{
    public bool bought = false;
    public int sector;

    public int totalBuildingsInArea;
    public int builtBuildingsInArea;
    public List<GameObject> treesInArea;

    public float workingPower;

    [SerializeField] bool working;

    [SerializeField] float loopTime = 10;
    [SerializeField] float timer;

    [SerializeField] Material grassMat;

    Renderer currentMaterial;

    [SerializeField] Vector3 size;

    [SerializeField] LayerMask[] gridPartMasks;

    [SerializeField] Mesh planeMesh;

    // Start is called before the first frame update
    void Start()
    {
        currentMaterial = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bought)
        {
            GetComponent<MeshCollider>().enabled = true;
            gameObject.layer = 8;
            currentMaterial.material = grassMat;

            if (builtBuildingsInArea > 0)
            {
                loopTime = 10 - 1.429f * builtBuildingsInArea; // needs to include workingpower in calculation

                if (treesInArea.Count > 10)
                {
                    working = true;
                }
            }
        }

        if (working)
        {
            if (treesInArea.Count > 2)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    foreach (var tree in treesInArea)
                    {
                        TreeScript treeScript = tree.GetComponent<TreeScript>();
                        if (treeScript.adultTree)
                        {
                            treeScript.StartAnimation();
                            treesInArea.Remove(tree);
                            timer = loopTime;

                            break;
                        }
                    }
                    
                }
            }
            else
            {
                working = false;
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

    public void BuySector()
    {
        if (StorageScript.Instance.money >= 100f)
        {
            bought = true;
            StorageScript.Instance.money -= 100f;
            if (sector > StorageScript.Instance.currentSector)
            {
                StorageScript.Instance.currentSector = sector;
            }
        }
    }
}
