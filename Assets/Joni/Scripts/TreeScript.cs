using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public int treeType;

    public float hp;

    public float averageTreeHeight;

    public float treeHeight;

    public bool adultTree = false;
    [SerializeField] GameObject grass;

    [SerializeField] bool beingSawed = false;

    public float growthEveryFrame;

    AreaScript areaOfTree;

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

    TutorialScript tutorialScript;

    // Start is called before the first frame update
    void Start()
    {
        StorageScript.Instance.treesInGame.Add(gameObject);

        audioManager = FindObjectOfType<AudioManager>();

        animator = GetComponent<Animator>();

        tutorialScript = FindObjectOfType<TutorialScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (areaOfTree == null)
        {
            RaycastHit treePos;
            if (Physics.Raycast(new Vector3(transform.position.x, 10f, transform.position.z), Vector3.down, out treePos, 20f, layerMasks[2]))
            {
                areaOfTree = treePos.collider.GetComponent<AreaScript>();
                areaOfTree.treesInArea.Add(gameObject);
                treeSpawnChance += (int)areaOfTree.workingPower * 4;
            }
        }

        if (adultTree) // Sitten ku puu on kasvanut kokonaan, se alkaa spawnaamaan muita puita
        {
            transform.localScale = new Vector3(treeHeight, treeHeight, treeHeight);
            grass.SetActive(true);

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
            if (transform.localScale.y >= treeHeight) // Kun puu on kasvanut
            {
                adultTree = true;
            }

            if (areaOfTree.builtBuildingsInArea > 0) // Puu kasvaa nopeammin jos alueella on rakennuksia
            {
                transform.localScale += new Vector3(1, 1, 1) * growthEveryFrame * (areaOfTree.workingPower + 1) * Time.deltaTime; // include workingpower in this too
            }
            else
            {
                transform.localScale += new Vector3(1, 1, 1) * growthEveryFrame * Time.deltaTime;

            }
        }

        areaOfTree.treeTypesInArea[treeType] = true;
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
                GameObject newTree = Instantiate(StorageScript.Instance.treeTypes[treeType], treePos.point, Quaternion.identity);
                TreeScript newTreeScript = newTree.GetComponent<TreeScript>();
                newTreeScript.treeHeight = Random.Range(newTreeScript.averageTreeHeight - 0.02f, newTreeScript.averageTreeHeight + 0.02f); // Arpoo random numeron puun korkeudelle
                newTree.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0);
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
            hp -= damage;
            Instantiate(axeModel, transform.position + new Vector3(0, 0.45f, -0.8f), Quaternion.Euler(0f, 180f, 90f));

            if (player)
            {
                audioManager.PlaySound("choptree", transform.position);
            }

            if (hp <= 0)
            {
                if (adultTree)
                {
                    areaOfTree.treesInArea.Remove(gameObject);
                   
                    StorageScript.Instance.wood[treeType] += treeHeight * 10;

                    if (tutorialScript.tutorial)
                    {
                        tutorialScript.treesCutDown++;
                    }
                }

                StorageScript.Instance.treesInGame.Remove(gameObject);

                if (player)
                {
                    audioManager.PlaySound("treefall", transform.position);
                }
                StartAnimation();
            }
        }
    }

    public void StartSawing(float damage)
    {
        if (!beingSawed)
        {
            beingSawed = true;
            StartCoroutine(SawTree(damage));
        }
    }

    IEnumerator SawTree(float damage)
    {
        GameObject saw = Instantiate(sawModel, transform.position + new Vector3(0.55f, 0.5f, 0f), Quaternion.Euler(90, 50, 50));
        audioManager.PlaySound("sawtree", transform.position);

        while (true)
        {
            yield return new WaitForSeconds(1f);
            hp -= damage;

            if (hp <= 0)
            {
                if (adultTree)
                {
                    areaOfTree.treesInArea.Remove(gameObject);
                    
                    StorageScript.Instance.wood[treeType] += treeHeight * 10;

                    if (tutorialScript.tutorial)
                    {
                        tutorialScript.treesCutDown++;
                    }
                }

                StorageScript.Instance.treesInGame.Remove(gameObject);

                toolScript.sawing = false;

                Destroy(saw);
                audioManager.StopSound("sawtree");
                StartAnimation();
                yield break;
            }
        }
    }

    public void StartChainsaw()
    {
        if (!beingSawed)
        {
            if (adultTree)
            {
                audioManager.PlaySound("chainsawstart", Vector3.zero);
            }
        }
    }

    public void ChainsawTree(float damage)
    {
        if (!beingSawed)
        {
            if (adultTree)
            {
                if (!audioManager.IsPlaying("chainsawstart") && !audioManager.IsPlaying("chainsawloop"))
                {
                    audioManager.PlaySound("chainsawloop", Vector3.zero);
                }

                hp -= damage;
                //Instantiate(axeModel, transform.position + new Vector3(0, 0.45f, -0.8f), Quaternion.Euler(0f, 180f, 90f));
                
                if (hp <= 0)
                {
                    audioManager.PlaySound("treefall", transform.position);
                    StopChainsaw();
                    StartAnimation();
                }
            }
        }
    }

    public void StopChainsaw()
    {
        audioManager.StopSound("chainsawstart");
        audioManager.StopSound("chainsawloop");
        audioManager.PlaySound("chainsawstop", Vector3.zero);
    }

    public void StartAnimation()
    {
        if (!animationPlaying)
        {
            animator.SetTrigger("Cut");
            animationPlaying = true;
        }
    }

    void DestroyTree()
    {
        areaOfTree.treeTypesInArea[treeType] = false;
        Destroy(gameObject);
    }
}

[System.Serializable]
public class TreeSaveData
{
    public int treeType;
    public float hp;
    public bool adultTree;
    public float treeHeight;
    public CustomVector position;
    public float scale;
    public float yRotation;

    public TreeSaveData(int _treeType, float _hp, bool _adultTree, float _treeHeight, CustomVector _position, float _scale, float _yRotation) // Vector3 _position, Vector3 _scale, Quaternion _rotation
    {
        treeType = _treeType;
        hp = _hp;
        adultTree = _adultTree;
        treeHeight = _treeHeight;
        position = _position;
        scale = _scale;
        yRotation = _yRotation;
    }
}

[System.Serializable]
public class Sapling
{
    public string name;
    public float price;
    public Texture2D sprite;
}