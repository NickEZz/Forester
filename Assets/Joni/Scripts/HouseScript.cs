using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour
{
    [SerializeField] bool building;

    public float workingPower;

    [SerializeField] float buildTime;
    [SerializeField] float timer;

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

    public void CopyVars(float _workingPower, AreaScript _targetArea)
    {
        workingPower = _workingPower;
        targetArea = _targetArea;
    }
}
