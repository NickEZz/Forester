using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildScript : MonoBehaviour
{
    public bool buildMode;
    [SerializeField] int selectedBuilding;

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
     * rotate? vois olla iha hyvä
     * 
     * talon rakentamisessa kestää aikaa
     * 
     * puun hakkuun automatisointi kun on rakennuksia: 
     * eli jokaisella rakennuksella oma tuottomäärä, laske ne kaikki yhteen
     * sen kokonaisen tuottomäärän perusteella joko: 
     * 
     * anna puuta kokoajan pelaajalle ilman että puita katoaa kartalta
     * tai
     * kaikki paitsi 1-5 puuta kaatuu joka ? 10 min vaikka
     */


    // Update is called once per frame
    void Update() // Kommentoin myöhemmin ku koko scripti valmis tai pyynnöstä!!!! tiiän että on vähän sekava vielä
    {
        if (Input.GetKeyDown(KeyCode.B))
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

                if (Input.GetMouseButtonDown(0) && !movingBuilding)
                {
                    if (StorageScript.Instance.money >= buildings[selectedBuilding].moneyCost && StorageScript.Instance.wood >= buildings[selectedBuilding].woodCost)
                    {
                        SpawnHouse(selectedBuilding);
                    }
                    else
                    {
                        // Joku teksti että ei riitä raha/puu
                    }
                }
            }
            else
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject.layer == 9 && Input.GetMouseButtonDown(0))
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

    void DestroyBuilding(float workingPower) // Probably not needed
    {
        // Joku tuhoamisanimaatio tähän
        StorageScript.Instance.workingPower -= workingPower;
        Destroy(gameObject);
    }

    IEnumerator MoveBuilding(GameObject movableObject)
    {
        while (true)
        {
            movingBuilding = true;

            movableObject.transform.position = previewObject.transform.position;

            yield return null;

            if (Input.GetMouseButtonDown(0))
            {
                movableObject.transform.position = previewTargetPos;
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

        if (targetArea.buildingsInArea < 5)
        {
            targetArea.buildingsInArea++;
            StorageScript.Instance.money -= buildings[select].moneyCost;
            StorageScript.Instance.wood -= buildings[select].woodCost;
            StorageScript.Instance.workingPower += buildings[select].workingPower;
            Instantiate(buildings[select].buildingPrefab, hit.point + new Vector3(0, 0.14f, 0), Quaternion.identity);
        }
        else
        {
            // Joku teksti että max määrä taloja
        }
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

