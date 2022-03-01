using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] int treeType;

    [SerializeField] int hp;

    [SerializeField] float averageTreeHeight;

    [SerializeField] float treeHeight;

    public bool adultTree = false;

    [SerializeField] bool beingSawed = false;

    [SerializeField] float growthEveryFrame;

    AreaScript areaOfTree;
    GameObject newTree;

    Animator animator;

    [SerializeField] int treeSpawnChance;

    [SerializeField] float loopTime = 1;
    [SerializeField] float timer;

    [SerializeField] int treesInRadius;

    [SerializeField] LayerMask[] layerMasks;
    public ToolScript toolScript;

    // Start is called before the first frame update
    void Start()
    {
        treeHeight = Random.Range(averageTreeHeight - 0.02f, averageTreeHeight + 0.02f); // Arpoo random numeron puun korkeudelle
        animator = GetComponent<Animator>();

        RaycastHit treePos;
        Physics.Raycast(new Vector3(transform.position.x, 10f, transform.position.z), Vector3.down, out treePos, 20f, layerMasks[2]);
       
        areaOfTree = treePos.collider.GetComponent<AreaScript>();
        areaOfTree.treesInArea.Add(gameObject);

        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (adultTree) // Sitten ku puu on kasvanut kokonaan, se alkaa spawnaamaan muita puita
        {
            // Ajastin, kun ajastin on 0, aloittaa ajastimen alusta
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = loopTime;

                // Valitsee random numeron 1-100 eli 1-100%
                float rng = Random.Range(1, 101);
                if (rng <= treeSpawnChance)
                {
                    SpawnTree();
                }

                // Tarkistaa kuinka monta puuta t‰m‰n puun l‰hell‰ on
                Collider[] sphere = Physics.OverlapSphere(transform.position, 3f, layerMasks[0]);
                treesInRadius = sphere.Length;
            }

            /*// Jos puita on enemm‰n kuin 5 yhdell‰ alueella, scripti lopettaa puiden generoimisen
            if (treesInRadius <= 5)
            {
                
            }*/
        }
        else // Puu kasvaa ensin
        {
            if (areaOfTree.builtBuildingsInArea > 0) // Puu kasvaa nopeammin jos alueella on rakennuksia
            {
                transform.localScale += new Vector3(1, 1, 1) * growthEveryFrame * (areaOfTree.builtBuildingsInArea + 1) * Time.deltaTime;
            }
            else
            {
                transform.localScale += new Vector3(1, 1, 1) * growthEveryFrame * Time.deltaTime;

            }
            
            if (transform.localScale.y >= treeHeight) // Kun puu on kasvanut
            {
                adultTree = true;
            }
        }
        


    }

    void SpawnTree()
    {
        // Valitsee random sijainnin puun l‰helt‰ jonne spawnata uusi puu
        float x = Random.Range(-1f, 2f);
        float z = Random.Range(-1f, 2f);

        
        Vector3 rayPos = new Vector3(transform.position.x - x, 10f, transform.position.z - z);

        // Ottaa sijainnin ylh‰‰lt‰p‰in raycastilla, ett‰ puu spawnaa maan p‰‰lle, eik‰ ole ilmassa tai maan sis‰ss‰.
        RaycastHit treePos;
        Physics.Raycast(rayPos, Vector3.down, out treePos);

        // Tarkistaa ensin onko halutulla alueella puuta valmiiksi, ett‰ uusi puu ei spawnaa olemassa olevan puun sis‰lle. Jos alueella on jo puu, toistaa t‰m‰n metodin uudelleen
        Collider[] overlap = Physics.OverlapSphere(treePos.point, 0.7f, layerMasks[1]);
        if (overlap.Length == 0)
        {
            GameObject newTree = Instantiate(StorageScript.Instance.trees[treeType], treePos.point, Quaternion.identity);
        }
        else
        {
            // spawnaa puu jonnekki muualle
        }
    }

    public void ChopTree(int damage)
    {
        if (!beingSawed)
        {
            if (adultTree)
            {
                hp -= damage;
                if (hp <= 0)
                {
                    //animator.SetTrigger("Cut");
                    StartAnimation();
                }
            }
        }
    }

    public void StartSawing(int damage)
    {
        if (!beingSawed)
        {
            if (adultTree)
            {
                beingSawed = true;
                StartCoroutine(SawTree(damage));
            }   
        }
    }

    IEnumerator SawTree(int damage)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            hp -= damage;
            if (hp <= 0)
            {
                toolScript.sawing = false;
                //animator.SetTrigger("Cut");
                StartAnimation();
                yield break;
            }
        }
    }

    public void StartAnimation()
    {
        animator.SetTrigger("Cut");
        StorageScript.Instance.wood += treeHeight * 10;
    }

    void DestroyTree()
    {
        Destroy(gameObject);
    }
}