using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildScript : MonoBehaviour
{
    public bool buildMode;
    public int selectedBuilding;

    public bool inStore;

    [SerializeField] bool movingBuilding;

    [SerializeField] Building[] buildings;

    [SerializeField] GameObject grid;
    Mesh gridMesh;

    [SerializeField] Vector3 gridSize = default;
    [SerializeField] float gridMoveSpeed;

    [SerializeField] GameObject buildingArea;

    [SerializeField] GameObject previewObject;
    Renderer previewObjectRenderer;
    Vector3 lastPos;
    Vector3 previewTargetPos;

    [SerializeField] LayerMask[] layerMasks;
    [SerializeField] Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        previewObjectRenderer = previewObject.GetComponent<Renderer>();
    }


    /*
     * Todo
     *
     */


    // Update is called once per frame
    void Update() // Kommentoin my�hemmin ku koko scripti valmis tai pyynn�st�!!!! tii�n ett� on v�h�n sekava viel�
    {
        if (Input.GetKeyDown(KeyCode.B) && !inStore)
        {
            buildMode = !buildMode;
        }

        if (buildMode)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                selectedBuilding--;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                selectedBuilding++;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateHouse();
            }

            if (buildingArea == null)
            {
                grid.SetActive(true);
                previewObject.SetActive(true);
            }

            RaycastHit areaCheck;
            if (Physics.Raycast(cam.gameObject.transform.position, cam.gameObject.transform.forward, out areaCheck, 30f, layerMasks[0]))
            {
                buildingArea = areaCheck.collider.gameObject;
                Vector3 gridDir = CalculateDirection(areaCheck.point, grid, false);
                grid.transform.position += gridDir * gridMoveSpeed * Time.deltaTime;
            }



            Vector2 input = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(input);

            RaycastHit mouse;
            if (Physics.Raycast(ray, out mouse, 20f, layerMasks[2]))
            {
                Vector3 previewDir = CalculateDirection(mouse.point, previewObject, true);

                lastPos = mouse.point;

                previewObject.transform.position += previewDir * gridMoveSpeed * Time.deltaTime;
            }
            else
            {
                Vector3 direction = CalculateDirection(lastPos, previewObject, true);

                previewObject.transform.position += direction * gridMoveSpeed * Time.deltaTime;
            }

            Collider[] colliders = Physics.OverlapBox(previewTargetPos, buildings[selectedBuilding].buildingSize, Quaternion.identity, layerMasks[1]);
            if (colliders.Length == 0)
            {
                previewObjectRenderer.material.SetColor("_Color", Color.green);

                if (Input.GetMouseButtonDown(0) && !movingBuilding && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    if (StorageScript.Instance.money >= buildings[selectedBuilding].moneyCost && StorageScript.Instance.wood >= buildings[selectedBuilding].woodCost)
                    {
                        SpawnHouse(selectedBuilding);
                    }
                    else
                    {
                        // Joku teksti ett� ei riit� raha/puu
                    }
                }
            }
            else
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject.layer == 9 && Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        if (!movingBuilding)
                        {
                            StartCoroutine(MoveBuilding(colliders[i].gameObject));
                        }
                    }
                    else if (colliders[i].gameObject.layer != 9)
                    {
                        previewObjectRenderer.material.SetColor("_Color", Color.red);
                    }
                }   
            }
        }
        else
        {
            grid.SetActive(false);
            buildingArea = null;
            previewObject.SetActive(false);
        }
    }


    // methods vvv
    Vector3 CalculateDirection(Vector3 targetPos, GameObject moveableObject, bool saveData)
    {
        if (saveData)
        {
            previewTargetPos = new Vector3(
                Mathf.Round(targetPos.x / gridSize.x) * gridSize.x,
                targetPos.y,
                Mathf.Round(targetPos.z / gridSize.z) * gridSize.z);
        }

        Vector3 test = new Vector3(
            Mathf.Round(targetPos.x / gridSize.x) * gridSize.x,
            targetPos.y,
            Mathf.Round(targetPos.z / gridSize.z) * gridSize.z);

        Vector3 direction = test - moveableObject.transform.position;

        //Vector3 direction = previewTargetPos - previewObject.transform.position;

        return direction;
    }

    IEnumerator MoveBuilding(GameObject movableObject)
    {
        previewObject.transform.rotation = movableObject.transform.rotation;

        while (true)
        {
            RaycastHit hit;
            Physics.Raycast(previewTargetPos + new Vector3(0f, 2f, 0f), Vector3.down, out hit, 10f, layerMasks[0]);

            movingBuilding = true;

            movableObject.transform.position = new Vector3(previewObject.transform.position.x, hit.point.y, previewObject.transform.position.z);
            movableObject.transform.rotation = previewObject.transform.rotation;

            yield return null;

            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                movableObject.transform.position = new Vector3(previewTargetPos.x, hit.point.y, previewTargetPos.z);
                break;
            }
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
            StorageScript.Instance.money -= buildings[select].moneyCost;
            StorageScript.Instance.wood -= buildings[select].woodCost;

            GameObject building = Instantiate(buildings[select].buildingPrefab, hit.point + new Vector3(0, 0.14f, 0), previewObject.transform.rotation);
            building.GetComponent<HouseScript>().CopyVars(buildings[select].workingPower, targetArea);
        }
        else
        {
            // Joku teksti ett� max m��r� taloja
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
    public float woodCost;
    public float moneyCost;
    public float workingPower;
    public Vector3 buildingSize;

    public Building(GameObject _buildingPrefab, float _woodCost, float _moneyCost, Vector3 _buildingSize, float _workingPower)
    {
        buildingPrefab = _buildingPrefab;
        woodCost = _woodCost;
        moneyCost = _moneyCost;
        buildingSize = _buildingSize;
        workingPower = _workingPower;
    }
}

