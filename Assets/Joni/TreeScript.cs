using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField]float loopTime = 1;
    [SerializeField]float timer;

    [SerializeField]int treesInRadius;

    [SerializeField] LayerMask treeMask;
    public GameObject treePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (treesInRadius <= 5)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = loopTime;
                float rng = Random.Range(1, 21);
                
                if (rng == 1)
                {
                    SpawnTree();
                }

                Collider[] sphere = Physics.OverlapSphere(transform.position, 3f, treeMask);
                treesInRadius = sphere.Length;
            }
        }


    }
    void SpawnTree()
    {
        float x = Random.Range(-2f, 3f);
        float z = Random.Range(-2f, 3f);
        Vector3 treePos = new Vector3(transform.position.x - x, transform.position.y, transform.position.z - z);

        Instantiate(treePrefab, treePos, Quaternion.identity);
    }

    public void CutTree()
    {
        Destroy(gameObject);
    }
}
