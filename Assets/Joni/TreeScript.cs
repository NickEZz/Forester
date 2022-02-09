using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] float loopTime = 1;
    [SerializeField] float timer;

    [SerializeField] int treesInRadius;

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

                if (rng >= 1)
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


        Vector3 rayPos = new Vector3(transform.position.x - x, 10f, transform.position.z - z);

        RaycastHit treePos;
        Physics.Raycast(rayPos, Vector3.down, out treePos);

        Collider[] overlap = Physics.OverlapSphere(treePos.point, 0.5f, treeMask);
        if (overlap.Length == 0)
        {
            Instantiate(treePrefab, treePos.point, Quaternion.identity);
        }
    }

    public void CutTree()
    {
        Destroy(gameObject);
    }


}
