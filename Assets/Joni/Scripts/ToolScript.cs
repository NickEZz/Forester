using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolScript : MonoBehaviour
{
    public int tool;
    [SerializeField] float axeDamage;
    [SerializeField] float sawDamage;
    [SerializeField] float chainsawDamage;

    public bool sawing = false;

    [SerializeField] float chainsawSpeed;
    float timer;

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
        trees = StorageScript.Instance.treeTypes;
        buildScript = GetComponent<BuildScript>();
        UpdateTool();
    }

    // Update is called once per frame
    void Update()
    {
        if (!buildScript.buildMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                tool--;

                if (StorageScript.Instance.currentChainsawUpgrade == 0 && tool == 2)
                {
                    tool--;
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                tool++;

                if (StorageScript.Instance.currentChainsawUpgrade == 0 && tool == 2)
                {
                    tool++;
                }

                
            }
            tool = Mathf.Clamp(tool++, 0, 5 - (2 - StorageScript.Instance.currentSector));

            Vector2 input = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(input);

            RaycastHit mouse;

            if (tool <= 2)
            {
                previewObject.SetActive(false);
            }

            switch (tool)
            {
                case 0: // Kirves

                    if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Physics.Raycast(ray, out mouse, 40f, layerMasks[2]))
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
                        if (Physics.Raycast(ray, out mouse, 40f, layerMasks[2]))
                        {
                            if (mouse.collider.GetComponent<TreeScript>().hp > 0)
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

                case 2: 
                    if (StorageScript.Instance.currentChainsawUpgrade > 0)
                    {
                        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                        {
                            if (Physics.Raycast(ray, out mouse, 40f, layerMasks[2]))
                            {
                                mouse.collider.GetComponent<TreeScript>().StartChainsaw();
                            }
                        }
                        if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                        {
                            if (Physics.Raycast(ray, out mouse, 40f, layerMasks[2]))
                            {
                                if (mouse.collider.GetComponent<TreeScript>().hp > 0)
                                {
                                    timer -= chainsawSpeed * Time.deltaTime;

                                    if (timer <= 0)
                                    {
                                        timer = 1;

                                        mouse.collider.GetComponent<TreeScript>().ChainsawTree(chainsawDamage);
                                    }
                                }
                            }
                            else
                            {
                                AudioManager am = FindObjectOfType<AudioManager>();
                                if (am.IsPlaying("chainsawloop"))
                                {
                                    am.StopSound("chainsawloop");
                                    am.PlaySound("chainsawstop", Vector3.zero);
                                }
                            }
                        }
                        if (Input.GetMouseButtonUp(0))
                        {
                            timer = 1;

                            if (Physics.Raycast(ray, out mouse, 40f, layerMasks[2]))
                            {
                                TreeScript treeScript = mouse.collider.GetComponent<TreeScript>();

                                if (treeScript.hp > 0 && treeScript.adultTree) 
                                {
                                    treeScript.StopChainsaw();
                                }
                            }
                        }
                    }

                    
                    break;

                case 3:
                    SpawnTree(0, ray);

                    
                    break;

                case 4:
                    SpawnTree(1, ray);

                    
                    break;
                case 5:
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
                        GameObject newTree = Instantiate(trees[tree], mouse.point, Quaternion.identity);
                        TreeScript newTreeScript = newTree.GetComponent<TreeScript>();
                        newTreeScript.treeHeight = Random.Range(newTreeScript.averageTreeHeight - 0.02f, newTreeScript.averageTreeHeight + 0.02f); // Arpoo random numeron puun korkeudelle
                        newTree.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0);
                        //StorageScript.Instance.treesInGame.Add(newTree);
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
        axeDamage = StorageScript.Instance.axes[StorageScript.Instance.currentAxeUpgrade].toolDamage;
        sawDamage = StorageScript.Instance.saws[StorageScript.Instance.currentSawUpgrade].toolDamage;
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
    public Sprite toolSprite;
    public float toolPrice;
    public float toolDamage;

    public Tool(Sprite _toolSprite, float _toolPrice, float _toolDamage, int _toolId)
    {
        toolSprite = _toolSprite;
        toolPrice = _toolPrice;
        toolDamage = _toolDamage;
        toolId = _toolId;
    }
}

