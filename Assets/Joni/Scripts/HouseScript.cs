using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour
{
    public int buildingLevel;

    public float workingPower;

    public bool upgradeable;
    public float[] upgradeWoodCost;
    public float upgradeMoneyCost;

    bool beingBuilt = true;

    public float buildTime;

    public float yOffset;

    public int chosenColor;
    [SerializeField] int materialIndex;
    [SerializeField] MeshRenderer meshRenderer;


    [SerializeField] LayerMask groundMask;

    public AreaScript targetArea;

    // Start is called before the first frame update
    void Start()
    {
        StorageScript.Instance.buildingsInGame.Add(gameObject);
        

        //MeshRenderer meshRenderer = houseBase.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            Material[] materials = meshRenderer.materials;
            materials[materialIndex] = StorageScript.Instance.buildingTypes[buildingLevel].colors[chosenColor];
            meshRenderer.sharedMaterials = materials;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        if (targetArea == null)
        {
            RaycastHit areaCheck;
            if (Physics.Raycast(new Vector3(transform.position.x, 10f, transform.position.z), Vector3.down, out areaCheck, 20f, groundMask))
            {
                targetArea = areaCheck.collider.GetComponent<AreaScript>();
                targetArea.totalBuildingsInArea++;
            }
        }

        if (beingBuilt)
        {
            buildTime -= Time.deltaTime;
            if (buildTime <= 0)
            {
                targetArea.builtBuildingsInArea++;
                targetArea.workingPower += workingPower;
                StorageScript.Instance.totalWorkingPower += workingPower;
                StorageScript.Instance.areas[targetArea.areaId].workingPower += workingPower;
                beingBuilt = false;
            }
        }
    }

    public bool UpgradeHouse()
    {
        if (StorageScript.Instance.money >= upgradeMoneyCost && StorageScript.Instance.wood[buildingLevel + 1] >= upgradeWoodCost[buildingLevel + 1])
        {
            FindObjectOfType<AudioManager>().PlaySound("construction", transform.position);

            StorageScript.Instance.money -= upgradeMoneyCost;
            StorageScript.Instance.wood[buildingLevel + 1] -= upgradeWoodCost[buildingLevel + 1];
            GameObject newBuilding = Instantiate(StorageScript.Instance.buildingTypes[buildingLevel + 1].buildingPrefab, transform.position, transform.rotation);
            HouseScript newBuildingScript = newBuilding.GetComponent<HouseScript>();
            newBuildingScript.chosenColor = chosenColor;
            newBuildingScript.buildingLevel = buildingLevel + 1;

            targetArea.totalBuildingsInArea--;
            targetArea.builtBuildingsInArea--;
            targetArea.workingPower -= workingPower;
            StorageScript.Instance.totalWorkingPower -= workingPower;
            StorageScript.Instance.areas[targetArea.areaId].workingPower -= workingPower;
            StorageScript.Instance.buildingsInGame.Remove(gameObject);

            Destroy(gameObject);

            return true;
        }
        else
        {
            return false;
        }
    }
}

[System.Serializable]
public class BuildingSaveData
{
    public int buildingLevel;
    public float timer;
    public int mat;
    public CustomVector position;
    public float yRotation;

    public BuildingSaveData(int _buildingLevel, float _timer, int _mat, CustomVector _position, float _yRotation)
    {
        buildingLevel = _buildingLevel;
        timer = _timer;
        mat = _mat;
        position = _position;
        yRotation = _yRotation;
    }
}