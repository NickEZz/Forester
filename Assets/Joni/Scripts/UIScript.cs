using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI woodCounter;
    [SerializeField] TextMeshProUGUI moneyCounter;

    [SerializeField] GameObject[] elementsToHide;

    [SerializeField] ToolScript toolScript;
    [SerializeField] BuildScript buildScript;

    [SerializeField] RawImage currentBuildingIcon;
    [SerializeField] Texture[] buildingIcons;

    [SerializeField] RawImage currentToolIcon;
    [SerializeField] Texture[] toolIcons;

    private void Update()
    {
        woodCounter.text = Mathf.Round(StorageScript.Instance.wood).ToString();
        moneyCounter.text = Mathf.Round(StorageScript.Instance.money).ToString();

        currentToolIcon.texture = toolIcons[toolScript.tool];
        //currentBuildingIcon.texture = buildingIcons[buildScript.selectedBuilding];

        if (buildScript.buildMode)
        {
            for (int i = 0; i < elementsToHide.Length - 1; i++)
            {
                elementsToHide[i].SetActive(false);
            }
            elementsToHide[3].SetActive(true);
        }
        else
        {
            elementsToHide[0].SetActive(true);
            elementsToHide[1].SetActive(true);
            elementsToHide[3].SetActive(false);
        }
    }

    public void ToggleToolMenu()
    {
        elementsToHide[2].SetActive(!elementsToHide[2].activeSelf);
    }

    public void SelectAxe()
    {
        toolScript.tool = 0;
        currentToolIcon.texture = toolIcons[0];
        ToggleToolMenu();
    }

    public void SelectSaw()
    {
        toolScript.tool = 1;
        currentToolIcon.texture = toolIcons[1];
        ToggleToolMenu();
    }

    public void SelectSpruceSapling()
    {
        toolScript.tool = 2;
        currentToolIcon.texture = toolIcons[2];
        ToggleToolMenu();
    }

    public void ToggleBuildMode()
    {
        buildScript.buildMode = !buildScript.buildMode;
    }

    public void SelectBuildingOne()
    {
        buildScript.selectedBuilding = 0;
    }

    public void SelectBuildingTwo()
    {
        buildScript.selectedBuilding = 1;
    }

    public void SelectBuildingThree()
    {
        buildScript.selectedBuilding = 2;
    }

    public void ToggleStore()
    {
        elementsToHide[4].SetActive(!elementsToHide[4].activeSelf);
        buildScript.inStore = elementsToHide[4].activeSelf;
    }
}
