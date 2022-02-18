using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolScript : MonoBehaviour
{
    [SerializeField] int tool;

    public bool sawing = false;

    GameObject[] previewObjects;
    GameObject[] trees;
    public GameObject currentPreviewObject;

    [SerializeField] LayerMask[] layerMasks;
    [SerializeField] GameObject mainCamera;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = mainCamera.GetComponent<Camera>();
        previewObjects = StorageScript.Instance.previewObjects;
        trees = StorageScript.Instance.trees;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(input);

        RaycastHit mouse;

        switch (tool)
        {
            case 0: // Kirves

                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(ray, out mouse, 20f, layerMasks[0]))
                    {
                        mouse.collider.GetComponent<TreeScript>().ChopTree(1);
                    }
                }
                
                break;

            case 1: // Saha

                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(ray, out mouse, 20f, layerMasks[1]))
                    {
                        if (!sawing)
                        {
                            sawing = true;
                            mouse.collider.GetComponent<TreeScript>().StartSawing(1);
                            mouse.collider.GetComponent<TreeScript>().toolScript = this;
                        }
                    }
                }

                break;

            case 2: // Spruce

                if (Physics.Raycast(ray, out mouse, 20f, layerMasks[1]))
                {
                    currentPreviewObject.transform.position = mouse.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        Instantiate(trees[0], mouse.point, Quaternion.identity);
                    }
                }

                break;

            case 3:
                break;
            case 4:
                break;
            default:
                break;
        }
    }

    void SpawnTree(int tree)
    {

    }
}
