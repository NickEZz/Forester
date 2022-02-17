using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolScript : MonoBehaviour
{
    [SerializeField] LayerMask treeMask;
    [SerializeField] GameObject mainCamera;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = mainCamera.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 input = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(input);

            RaycastHit mouse; 
            if (Physics.Raycast(ray, out mouse, 20f, treeMask))
            {
                mouse.collider.GetComponent<TreeScript>().CutTree();
            }
        }
    }
}
