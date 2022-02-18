using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] int treeType;

    [SerializeField] int hp;

    [SerializeField] float treeHeight;

    [SerializeField] bool adultTree = false;

    [SerializeField] bool beingSawed = false;

    [SerializeField] float growthEveryFrame;


    [SerializeField] int treeSpawnChance;

    [SerializeField] float loopTime = 1;
    [SerializeField] float timer;

    [SerializeField] int treesInRadius;

    [SerializeField] LayerMask treeMask;
    public ToolScript toolScript;

    // Start is called before the first frame update
    void Start()
    {
        treeHeight = Random.Range(0.1f, 0.15f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (adultTree) // Sitten ku puu on kasvanut kokonaan, se alkaa spawnaamaan muita puita
        {
            // Jos puita on enemm�n kuin 5 yhdell� alueella, scripti lopettaa puiden generoimisen
            if (treesInRadius <= 5)
            {
                // Ajastin, kun ajastin on 0, aloittaa ajastimen alusta
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = loopTime;

                    // Valitsee random numeron 1-20, eli t�ll� hetkell� 5% mahdollisuus ett� puu spawnaa
                    float rng = Random.Range(1, 101);
                    if (rng <= treeSpawnChance)
                    {
                        SpawnTree();
                    }

                    // Tarkistaa kuinka monta puuta t�m�n puun l�hell� on
                    Collider[] sphere = Physics.OverlapSphere(transform.position, 3f, treeMask);
                    treesInRadius = sphere.Length;
                }
            }
        }
        else // Puu kasvaa ensin
        {
            transform.localScale += new Vector3(1, 1, 1) * growthEveryFrame * Time.deltaTime;
            if (transform.localScale.y >= treeHeight)
            {
                adultTree = true;
            }
        }
        


    }
    void SpawnTree()
    {
        // Valitsee random sijainnin puun l�helt� jonne spawnata uusi puu
        float x = Random.Range(-2f, 3f);
        float z = Random.Range(-2f, 3f);

        
        Vector3 rayPos = new Vector3(transform.position.x - x, 10f, transform.position.z - z);

        // Ottaa sijainnin ylh��lt�p�in raycastilla, ett� puu spawnaa maan p��lle, eik� ole ilmassa tai maan sis�ss�.
        RaycastHit treePos;
        Physics.Raycast(rayPos, Vector3.down, out treePos);

        // Tarkistaa ensin onko halutulla alueella puuta valmiiksi, ett� uusi puu ei spawnaa olemassa olevan puun sis�lle. Jos alueella on jo puu, toistaa t�m�n metodin uudelleen
        Collider[] overlap = Physics.OverlapSphere(treePos.point, 0.5f, treeMask);
        if (overlap.Length == 0)
        {
            Instantiate(StorageScript.Instance.trees[treeType], treePos.point, Quaternion.identity);
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
                    StorageScript.Instance.wood += treeHeight * 10;
                    Destroy(gameObject);
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
                StorageScript.Instance.wood += treeHeight * 10;
                Destroy(gameObject);
            }
        }
    }
}