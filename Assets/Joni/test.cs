using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                GameObject empty = GameObject.CreatePrimitive(PrimitiveType.Cube);
                empty.transform.position = new Vector3(x * 1.11f, 2, y * 1.11f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
