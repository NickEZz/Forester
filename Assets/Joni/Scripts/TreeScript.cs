using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] int treeType;

    public float hp;

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

    [SerializeField] GameObject axeModel;
    [SerializeField] GameObject sawModel;

    bool animationPlaying;

    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        treeHeight = Random.Range(averageTreeHeight - 0.02f, averageTreeHeight + 0.02f); // Arpoo random numeron puun korkeudelle
        animator = GetComponent<Animator>();

        RaycastHit treePos;
        Physics.Raycast(new Vector3(transform.position.x, 10f, transform.position.z), Vector3.down, out treePos, 20f, layerMasks[2]);
       
        areaOfTree = treePos.collider.GetComponent<AreaScript>();
        areaOfTree.treesInArea.Add(gameObject);
        treeSpawnChance += (int)areaOfTree.workingPower * 4;

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
        }
        else // Puu kasvaa ensin
        {
            if (areaOfTree.builtBuildingsInArea > 0) // Puu kasvaa nopeammin jos alueella on rakennuksia
            {
                transform.localScale += new Vector3(1, 1, 1) * growthEveryFrame * (areaOfTree.workingPower + 1) * Time.deltaTime; // include workingpower in this too
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
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);

        
        Vector3 rayPos = new Vector3(transform.position.x - x, 10f, transform.position.z - z);

        // Ottaa sijainnin ylh‰‰lt‰p‰in raycastilla, ett‰ puu spawnaa maan p‰‰lle, eik‰ ole ilmassa tai maan sis‰ss‰.
        RaycastHit treePos;
        if (Physics.Raycast(rayPos, Vector3.down, out treePos, 20f, layerMasks[2]))
        {
            // Tarkistaa ensin onko halutulla alueella puuta valmiiksi, ett‰ uusi puu ei spawnaa olemassa olevan puun sis‰lle. Jos alueella on jo puu, toistaa t‰m‰n metodin uudelleen
            Collider[] overlap = Physics.OverlapSphere(treePos.point, 0.7f, layerMasks[1]);
            if (overlap.Length == 0)
            {
                GameObject newTree = Instantiate(StorageScript.Instance.trees[treeType], treePos.point, Quaternion.identity);
                StorageScript.Instance.treesInGame.Add(newTree);
            }
            else
            {
                // spawnaa puu jonnekki muualle??
            }
        }
        
    }

    public void ChopTree(float damage, bool player)
    {
        if (!beingSawed)
        {
            if (adultTree)
            {
                hp -= damage;
                Instantiate(axeModel, transform.position + new Vector3(0, 0.45f, -0.8f), Quaternion.Euler(0f, 180f, 90f));

                if (player)
                {
                    audioManager.PlaySound("choptree", transform.position);
                }

                if (hp <= 0)
                {
                    if (player)
                    {
                        audioManager.PlaySound("treefall", transform.position);
                    }
                    StartAnimation();
                }
            }
        }
    }

    public void StartSawing(float damage)
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

    IEnumerator SawTree(float damage)
    {
        GameObject saw = Instantiate(sawModel, transform.position + new Vector3(0.6f, 0.36f, 0f), Quaternion.Euler(0, 50, 90));
        audioManager.PlaySound("sawtree", transform.position);

        while (true)
        {
            yield return new WaitForSeconds(1f);
            hp -= damage;

            if (hp <= 0)
            {
                toolScript.sawing = false;

                Destroy(saw);
                audioManager.StopSound("sawtree");
                StartAnimation();
                yield break;
            }
        }
    }

    public void StartAnimation()
    {
        if (!animationPlaying)
        {
            animator.SetTrigger("Cut");
            animationPlaying = true;
            areaOfTree.treesInArea.Remove(gameObject);
            StorageScript.Instance.treesInGame.Remove(gameObject);
            StorageScript.Instance.wood[treeType] += treeHeight * 10;
            /*
            switch (treeType)
            {
                case 0:
                    StorageScript.Instance.spruceWood += treeHeight * 10;
                    break;
                case 1:
                    StorageScript.Instance.pineWood += treeHeight * 10;
                    break;
                case 2:
                    StorageScript.Instance.birchWood += treeHeight * 10;
                    break;
            }*/
        }
    }

    void DestroyTree()
    {
        Destroy(gameObject);
    }
}