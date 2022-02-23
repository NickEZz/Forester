using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolScript : MonoBehaviour
{
    [SerializeField] int tool;
    [SerializeField] int axeDamage;
    [SerializeField] int sawDamage;

    public bool sawing = false;

    Mesh[] treeMeshes;
    GameObject[] trees;
    [SerializeField] GameObject previewObject;
    [SerializeField] Renderer previewObjectRenderer;

    BuildScript buildScript;

    [SerializeField] LayerMask[] layerMasks;
    [SerializeField] GameObject mainCamera;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = mainCamera.GetComponent<Camera>();
        treeMeshes = StorageScript.Instance.treeMeshes;
        trees = StorageScript.Instance.trees;
        buildScript = GetComponent<BuildScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!buildScript.buildMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                tool--;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                tool++;
            }

            Vector2 input = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(input);

            RaycastHit mouse;

            if (tool <= 1)
            {
                previewObject.GetComponent<MeshFilter>().mesh = null;
            }

            switch (tool)
            {
                case 0: // Kirves

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Physics.Raycast(ray, out mouse, 20f, layerMasks[0]))
                        {
                            mouse.collider.GetComponent<TreeScript>().ChopTree(axeDamage);
                        }
                    }

                    break;

                case 1: // Saha

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Physics.Raycast(ray, out mouse, 20f, layerMasks[0]))
                        {
                            if (!sawing)
                            {
                                sawing = true;
                                mouse.collider.GetComponent<TreeScript>().StartSawing(sawDamage);
                                mouse.collider.GetComponent<TreeScript>().toolScript = this;
                            }
                        }
                    }

                    break;

                case 2: // Spruce
                    SpawnTree(0, ray);
                    break;

                case 3:
                    break;
                case 4:
                    break;
                default:
                    break;
            }
        }
    }

    void SpawnTree(int tree, Ray ray)
    {
        RaycastHit mouse;

        if (Physics.Raycast(ray, out mouse, 20f, layerMasks[1]))
        {
            previewObject.GetComponent<MeshFilter>().mesh = treeMeshes[0]; // Muuttaa previewobjectin meshin     Pitäs keksiä joku tapa miten peli ei kutsuis getcomponent metodia joka frame, huono performancelle
            previewObject.transform.position = mouse.point; // Liikuttaa previewobjectia sinne missä hiiri on

            Collider[] overlap = Physics.OverlapSphere(mouse.point, 0.5f, layerMasks[0]);
            if (overlap.Length > 0)
            {
                previewObjectRenderer.materials[1].SetColor("_Color", Color.red); // new Color(255f, 0f, 0f)
            }
            else if (overlap.Length == 0)
            {
                previewObjectRenderer.materials[1].SetColor("_Color", Color.white); // new Color(255f, 255f, 255f)

                if (Input.GetMouseButtonDown(0))
                {
                    if (StorageScript.Instance.spruceSaplings > 0)
                    {
                        StorageScript.Instance.spruceSaplings--;
                        Instantiate(trees[tree], mouse.point, Quaternion.identity);
                    }
                }
            }
        }
    }
}
