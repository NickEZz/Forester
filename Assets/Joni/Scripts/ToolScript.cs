using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolScript : MonoBehaviour
{
    public int tool;
    [SerializeField] float axeDamage;
    [SerializeField] float sawDamage;

    public int currentAxeUpgrade;
    public int currentSawUpgrade;

    public Tool[] axes;
    public Tool[] saws;

    public bool sawing = false;

    GameObject[] trees;
    [SerializeField] GameObject previewObject;
    [SerializeField] Renderer previewObjectRenderer;

    [SerializeField] GameObject axeModel;
    [SerializeField] GameObject sawModel;

    BuildScript buildScript;

    [SerializeField] LayerMask[] layerMasks;
    [SerializeField] GameObject mainCamera;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = mainCamera.GetComponent<Camera>();
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
            tool = Mathf.Clamp(tool++, 0, 4);

            Vector2 input = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(input);

            RaycastHit mouse;

            if (tool <= 1)
            {
                previewObject.SetActive(false);
            }

            switch (tool)
            {
                case 0: // Kirves

                    if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Physics.Raycast(ray, out mouse, 40f, layerMasks[0]))
                        {
                            if (mouse.collider.GetComponent<TreeScript>().hp > 0)
                            {
                                mouse.collider.GetComponent<TreeScript>().ChopTree(axeDamage, true);
                            }
                        }
                    }

                    break;

                case 1: // Saha

                    if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Physics.Raycast(ray, out mouse, 40f, layerMasks[0]))
                        {
                            if (mouse.collider.GetComponent<TreeScript>().adultTree && mouse.collider.GetComponent<TreeScript>().hp > 0)
                            {
                                if (!sawing)
                                {
                                    sawing = true;
                                    mouse.collider.GetComponent<TreeScript>().StartSawing(sawDamage);
                                    mouse.collider.GetComponent<TreeScript>().toolScript = this;
                                }
                            }
                        }
                    }

                    break;

                case 2: // Spruce
                    SpawnTree(0, ray);
                    break;

                case 3:
                    SpawnTree(1, ray);
                    break;

                case 4:
                    SpawnTree(2, ray);
                    break;

                default:
                    break;
            }
        }
        else
        {
            previewObject.SetActive(false);
        }
    }

    void SpawnTree(int tree, Ray ray)
    {
        RaycastHit mouse;

        if (Physics.Raycast(ray, out mouse, 40f, layerMasks[1]))
        {
            previewObject.SetActive(true); // Laittaa previewobjectin näkyviin
            previewObject.transform.position = mouse.point; // Liikuttaa previewobjectia sinne missä hiiri on

            if (StorageScript.Instance.saplings[tree] > 0)
            {
                Collider[] overlap = Physics.OverlapSphere(mouse.point, 0.3f, layerMasks[0]);
                if (overlap.Length > 0)
                {
                    previewObjectRenderer.materials[1].SetColor("_Color", Color.red); // new Color(255f, 0f, 0f)
                }
                else if (overlap.Length == 0)
                {
                    previewObjectRenderer.materials[1].SetColor("_Color", Color.white); // new Color(255f, 255f, 255f)

                    if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        FindObjectOfType<AudioManager>().PlaySound("planttree", mouse.point);
                        StorageScript.Instance.saplings[tree]--;
                        Instantiate(trees[tree], mouse.point, Quaternion.identity);
                    }
                }
            }
            else
            {
                previewObjectRenderer.materials[1].SetColor("_Color", Color.red);
            }
        }
    }

    public void UpdateTool()
    {
        axeDamage = axes[currentAxeUpgrade].toolDamage;
        sawDamage = saws[currentSawUpgrade].toolDamage;
    }

    void DestroyTool()
    {
        Destroy(gameObject);
    }
}

[System.Serializable]
public struct Tool
{
    public int toolId;
    public Texture2D toolSprite;
    public float toolPrice;
    public float toolDamage;

    public Tool(Texture2D _toolSprite, float _toolPrice, float _toolDamage, int _toolId)
    {
        toolSprite = _toolSprite;
        toolPrice = _toolPrice;
        toolDamage = _toolDamage;
        toolId = _toolId;
    }
}

