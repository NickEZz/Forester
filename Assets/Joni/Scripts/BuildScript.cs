using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildScript : MonoBehaviour
{
    public bool buildMode;
    public int selectedBuilding;

    public bool inStore;

    [SerializeField] bool movingBuilding;

    public List<GameObject> gridList;

    [SerializeField] Vector3 cellSize = default;

    [SerializeField] GameObject previewObject;
    Renderer previewObjectRenderer;
    Vector3 lastPos;
    Vector3 previewTargetPos;
    [SerializeField] float previewObjectMoveSpeed;
    [SerializeField] Vector3 previewObjectSize;

    [SerializeField] LayerMask[] layerMasks;
    [SerializeField] Camera cam;

    GameObject previousArea;
    GameObject currentArea;

    // Start is called before the first frame update
    void Start()
    {
        previewObjectRenderer = previewObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() // Kommentoin myöhemmin ku koko scripti valmis tiiän että on vähän sekava 
    {
        if (Input.GetKeyDown(KeyCode.B) && !inStore)
        {
            buildMode = !buildMode;
        }

        if (buildMode)
        {
            for (int i = 0; i < gridList.Count; i++) // Haluan että se tekee tän vaan kerran
            {
                gridList[i].SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                selectedBuilding--;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                selectedBuilding++;
            }
            selectedBuilding = Mathf.Clamp(selectedBuilding, 0, StorageScript.Instance.buildingTypes.Length - 1);
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateHouse();
            }

            previewObject.transform.localScale = previewObjectSize * 2;

            Vector2 input = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(input);

            RaycastHit mouse;
            if (Physics.Raycast(ray, out mouse, 100f, layerMasks[2]))
            {
                Vector3 previewDir = CalculateDirection(mouse.point, previewObject, true);

                lastPos = mouse.point;

                previewObject.transform.position += previewDir * previewObjectMoveSpeed * Time.deltaTime;

                currentArea = mouse.collider.gameObject;

                if (!movingBuilding)
                {
                    if (currentArea != previousArea)
                    {
                        if (previousArea != null)
                        {
                            previousArea.GetComponentInParent<AreaScript>().focused = false;
                        }
                        currentArea.GetComponentInParent<AreaScript>().focused = true;
                    }

                    previousArea = mouse.collider.gameObject;
                }
                
                
            }
            else
            {
                Vector3 direction = CalculateDirection(lastPos, previewObject, true);

                previewObject.transform.position += direction * previewObjectMoveSpeed * Time.deltaTime;
            }

            if (!movingBuilding)
            {
                Collider[] colliders = Physics.OverlapBox(previewTargetPos, previewObjectSize, Quaternion.identity, layerMasks[1]); // tämä
                if (colliders.Length == 0) // Jos muita rakennuksia ei ole edessä
                {
                    previewObject.SetActive(true);
                    previewObjectRenderer.material.SetColor("_Color", Color.green);

                    if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        if (StorageScript.Instance.money >= StorageScript.Instance.buildingTypes[selectedBuilding].moneyCost && StorageScript.Instance.wood[selectedBuilding] >= StorageScript.Instance.buildingTypes[selectedBuilding].woodCost[selectedBuilding])
                        {
                            SpawnHouse(selectedBuilding);
                        }
                        else
                        {
                            // Joku teksti että ei riitä raha/puu
                        }
                    }
                }
                else // Jos jotakin on edessä
                {
                    Collider[] smallArea = Physics.OverlapBox(previewTargetPos, new Vector3(0.3f, 0.2f, 0.3f), Quaternion.identity, layerMasks[3]);
                    if (smallArea.Length > 0)
                    {
                        if (smallArea[0].gameObject.layer == 9)
                        {
                            previewObject.SetActive(false);
                            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                            {
                                StartCoroutine(MoveBuilding(smallArea[0].gameObject));
                            }
                        }
                        
                    }
                    else
                    {
                        previewObject.SetActive(true);
                        previewObjectRenderer.material.SetColor("_Color", Color.red);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < gridList.Count; i++) // Haluan että se tekee tän vaan kerran
            {
                gridList[i].SetActive(false);
            }

            previewObject.SetActive(false);

            if (currentArea != null)
            {
                currentArea.GetComponentInParent<AreaScript>().focused = false;
                currentArea = null;
            }
            if (previousArea != null)
            {
                previousArea.GetComponentInParent<AreaScript>().focused = false;
                previousArea = null;
            }
        }
    }


    // methods vvv
    Vector3 CalculateDirection(Vector3 targetPos, GameObject moveableObject, bool saveData)
    {
        if (saveData)
        {
            previewTargetPos = new Vector3(
                Mathf.Round(targetPos.x / cellSize.x) * cellSize.x,
                targetPos.y,
                Mathf.Round(targetPos.z / cellSize.z) * cellSize.z);
        }

        Vector3 test = new Vector3(
            Mathf.Round(targetPos.x / cellSize.x) * cellSize.x,
            targetPos.y,
            Mathf.Round(targetPos.z / cellSize.z) * cellSize.z);

        Vector3 direction = test - moveableObject.transform.position;

        return direction;
    }

    IEnumerator MoveBuilding(GameObject movableObject)
    {
        GameObject originalArea = movableObject.GetComponent<HouseScript>().targetArea.gameObject;
        HouseScript houseScript = movableObject.GetComponent<HouseScript>();
        Vector3 oldPos = movableObject.transform.position;
        previewObject.transform.rotation = movableObject.transform.rotation;
        previewObject.SetActive(true);

        while (true)
        {
            RaycastHit hit;
            Physics.Raycast(previewTargetPos + new Vector3(0f, 2f, 0f), Vector3.down, out hit, 10f, layerMasks[0]);

            movingBuilding = true;

            movableObject.transform.position = new Vector3(previewObject.transform.position.x, hit.point.y + 0.2f, previewObject.transform.position.z);
            movableObject.transform.rotation = previewObject.transform.rotation;

            Collider[] collidingHouses = Physics.OverlapBox(previewTargetPos, previewObjectSize, Quaternion.identity, layerMasks[1]); 
            if (collidingHouses.Length <= 1)
            {
                if (hit.collider.gameObject != originalArea)
                {
                    previewObjectRenderer.material.SetColor("_Color", Color.red);

                    if (Input.GetMouseButtonUp(0))
                    {
                        movableObject.transform.position = oldPos;
                        break;
                    }
                }
                else
                {
                    previewObjectRenderer.material.SetColor("_Color", Color.green);

                    if (Input.GetMouseButtonUp(0))
                    {
                        //movableObject.transform.position = new Vector3(previewTargetPos.x, hit.point.y + 0.2f, previewTargetPos.z);

                        movableObject.transform.position = new Vector3(previewTargetPos.x, hit.point.y + houseScript.yOffset, previewTargetPos.z);
                        break;
                    }
                }
                

                
            }
            else
            {
                previewObjectRenderer.material.SetColor("_Color", Color.red);
                if (Input.GetMouseButtonUp(0))
                {
                    movableObject.transform.position = oldPos;
                    break;
                }
            }
 
            yield return null;            
        }

        movingBuilding = false;

        yield break;
    }

    void SpawnHouse(int select)
    {
        RaycastHit hit;
        Physics.Raycast(previewTargetPos + new Vector3(0f, 2f, 0f), Vector3.down, out hit, 10f, layerMasks[0]);

        AreaScript targetArea = hit.collider.gameObject.GetComponent<AreaScript>();

        if (targetArea.totalBuildingsInArea < 6)
        {
            FindObjectOfType<AudioManager>().PlaySound("construction", hit.point);

            GameObject building = Instantiate(StorageScript.Instance.buildingTypes[select].buildingPrefab, hit.point, previewObject.transform.rotation); // buildings[select].buildingPrefab
            HouseScript houseScript = building.GetComponent<HouseScript>();

            building.transform.position += new Vector3(0, houseScript.yOffset, 0);

            StorageScript.Instance.money -= StorageScript.Instance.buildingTypes[select].moneyCost;
            StorageScript.Instance.wood[select] -= StorageScript.Instance.buildingTypes[select].woodCost[select];

            //StorageScript.Instance.money -= houseScript.moneyCost;
            //StorageScript.Instance.wood[select] -= houseScript.woodCost[houseScript.buildingLevel];
            
            houseScript.buildingLevel = select;
            houseScript.chosenColor = Random.Range(0, StorageScript.Instance.buildingTypes[select].colors.Length);

            SaveManager.Instance.SaveGameData();
        }
        else
        {
            // Joku teksti että max määrä taloja
        }
    }

    void RotateHouse()
    {
        previewObject.transform.rotation = previewObject.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
    }
}

[System.Serializable]
public struct Building
{
    public GameObject buildingPrefab;
    public float[] woodCost;
    public float moneyCost;
    public Material[] colors;

    public Building(GameObject _buildingPrefab, float[] _woodCost, float _moneyCost, Material[] _colors)
    {
        buildingPrefab = _buildingPrefab;
        woodCost = _woodCost;
        moneyCost = _moneyCost;
        colors = _colors;
    }
}

