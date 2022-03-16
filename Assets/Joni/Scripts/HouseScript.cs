using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour
{
    public int buildingLevel;

    public bool upgradeable;
    public float[] upgradeCost;
    public float moneyCost;
    Building[] buildings;

    Material mat;

    [SerializeField] bool building;

    float workingPower;

    [SerializeField] float buildTime;
    [SerializeField] float timer;

    [SerializeField] int materialIndex;

    AreaScript targetArea;

    // Start is called before the first frame update
    void Start()
    {
        building = true;
        timer = buildTime;
        targetArea.totalBuildingsInArea++;
    }

    // Update is called once per frame
    void Update()
    {
        if (building)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                targetArea.builtBuildingsInArea++;
                targetArea.workingPower += workingPower;
                building = false;
            }
        }
    }

    public void SetupHouse(int _buildingLevel, float _workingPower, AreaScript _targetArea, Material color, Building[] _buildings)
    {
        //workingPower = _workingPower;
        buildingLevel = _buildingLevel;
        workingPower = _buildings[_buildingLevel].workingPower;
        targetArea = _targetArea;
        buildings = _buildings;
        mat = color;

        MeshRenderer renderer = GetComponent<MeshRenderer>(); // Ei muuta väriä
        Material[] materials = renderer.materials;
        materials[materialIndex] = color;
        renderer.sharedMaterials = materials;
    }

    public bool UpgradeHouse()
    {
        if (StorageScript.Instance.money >= moneyCost && StorageScript.Instance.wood[buildingLevel + 1] >= upgradeCost[buildingLevel + 1])
        {
            StorageScript.Instance.money -= moneyCost;
            StorageScript.Instance.wood[buildingLevel + 1] -= upgradeCost[buildingLevel];
            GameObject newBuilding = Instantiate(buildings[buildingLevel + 1].buildingPrefab, transform.position, transform.rotation);
            newBuilding.GetComponent<HouseScript>().SetupHouse(buildingLevel + 1, buildings[buildingLevel + 1].workingPower, targetArea, mat, buildings);

            targetArea.totalBuildingsInArea--;
            targetArea.builtBuildingsInArea--;
            targetArea.workingPower -= workingPower;

            Destroy(gameObject);

            return true;
        }
        else
        {
            return false;
        }
    }
}
