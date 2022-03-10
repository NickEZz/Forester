using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totalWoodCounter;
    [SerializeField] TextMeshProUGUI[] moneyCounters;

    [SerializeField] TextMeshProUGUI[] woodCounters;

    [SerializeField] GameObject[] elementsToHide;

    [SerializeField] ToolScript toolScript;
    [SerializeField] BuildScript buildScript;

    [SerializeField] RawImage currentBuildingIcon;
    [SerializeField] Texture[] buildingIcons;

    [SerializeField] RawImage currentToolIcon;
    [SerializeField] Texture2D[] toolIcons;

    [SerializeField] RawImage[] shopToolIcons;
    [SerializeField] TextMeshProUGUI[] shopToolPrices;

    [SerializeField] GameObject pineSapling;
    [SerializeField] GameObject birchSapling;


    private void Update()
    {
        totalWoodCounter.text = StorageScript.Instance.totalWood.ToString("F1") + "m³";

        for (int i = 0; i < moneyCounters.Length; i++)
        {
            moneyCounters[i].text = StorageScript.Instance.money.ToString("F1") + "€";
        }
        for (int i = 0; i < woodCounters.Length; i++)
        {
            woodCounters[i].text = StorageScript.Instance.wood[i].ToString("F1") + "m³";
        }

        toolIcons[0] = toolScript.axes[toolScript.currentAxeUpgrade].toolSprite;
        toolIcons[1] = toolScript.saws[toolScript.currentSawUpgrade].toolSprite;

        for (int i = 0; i < shopToolIcons.Length; i++)
        {
            if (toolScript.currentAxeUpgrade == toolScript.axes.Length - 1) // Jos on max päivitys
            {
                shopToolPrices[0].text = "Max";
                shopToolIcons[0].texture = toolScript.axes[toolScript.currentAxeUpgrade].toolSprite;
            }
            else
            {
                shopToolIcons[0].texture = toolScript.axes[toolScript.currentAxeUpgrade + 1].toolSprite;
                shopToolPrices[0].text = toolScript.axes[toolScript.currentAxeUpgrade + 1].toolPrice.ToString() + "€";
            }

            if (toolScript.currentSawUpgrade == toolScript.saws.Length - 1) // Jos on max päivitys
            {
                shopToolPrices[1].text = "Max";
                shopToolIcons[1].texture = toolScript.saws[toolScript.currentSawUpgrade].toolSprite;
            }
            else
            {
                shopToolIcons[1].texture = toolScript.saws[toolScript.currentSawUpgrade + 1].toolSprite;
                shopToolPrices[1].text = toolScript.saws[toolScript.currentSawUpgrade + 1].toolPrice.ToString() + "€";
            }
        }

        switch (StorageScript.Instance.currentSector)
        {
            case 0:
                if (pineSapling.activeSelf)
                {
                    pineSapling.SetActive(false);
                }
                if (birchSapling.activeSelf)
                {
                    birchSapling.SetActive(false);
                }
                break;
            case 1:
                if (!pineSapling.activeSelf)
                {
                    pineSapling.SetActive(true);
                }
                break;
            case 2:
                if (!pineSapling.activeSelf)
                {
                    pineSapling.SetActive(true);
                }
                if (!birchSapling.activeSelf)
                {
                    birchSapling.SetActive(true);
                }
                break;
        }

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
    
    public void SellOneSpruce()
    {
        if (StorageScript.Instance.wood[0] >= 1)
        {
            StorageScript.Instance.wood[0]--;
            StorageScript.Instance.money += 60f;
        }
    }

    public void SellTenSpruce()
    {
        if (StorageScript.Instance.wood[0] >= 10)
        {
            StorageScript.Instance.wood[0] -= 10f;
            StorageScript.Instance.money += 10f * 60f;
        }
    }

    public void SellOnePine()
    {
        if (StorageScript.Instance.wood[1] >= 1)
        {
            StorageScript.Instance.wood[1]--;
            StorageScript.Instance.money += 130f;
        }
    }

    public void SellTenPine()
    {
        if (StorageScript.Instance.wood[1] >= 10)
        {
            StorageScript.Instance.wood[1] -= 10f;
            StorageScript.Instance.money += 10f * 130f;
        }
    }
    public void SellOneBirch()
    {
        if (StorageScript.Instance.wood[2] >= 1)
        {
            StorageScript.Instance.wood[2]--;
            StorageScript.Instance.money += 200f;
        }
    }

    public void SellTenBirch()
    {
        if (StorageScript.Instance.wood[2] >= 10)
        {
            StorageScript.Instance.wood[2] -= 10f;
            StorageScript.Instance.money += 10f * 200f;
        }
    }

    public void BuyAxeUpgrade()
    {
        if (StorageScript.Instance.money >= toolScript.axes[toolScript.currentAxeUpgrade + 1].toolPrice)
        {
            toolScript.currentAxeUpgrade++;
            StorageScript.Instance.money -= toolScript.axes[toolScript.currentAxeUpgrade].toolPrice;
            toolScript.UpdateTool();
        }
    }

    public void BuySawUpgrade()
    {
        if (StorageScript.Instance.money >= toolScript.saws[toolScript.currentSawUpgrade + 1].toolPrice)
        {
            toolScript.currentSawUpgrade++;
            StorageScript.Instance.money -= toolScript.saws[toolScript.currentSawUpgrade].toolPrice;
            toolScript.UpdateTool();
        }
    }

    public void BuySpruceSapling()
    {
        if (StorageScript.Instance.money >= 10f)
        {
            StorageScript.Instance.saplings[0]++;
            StorageScript.Instance.money -= 10f;
        }
    }
    public void BuyPineSapling()
    {
        if (StorageScript.Instance.currentSector >= 1)
        {
            if (StorageScript.Instance.money >= 10f)
            {
                StorageScript.Instance.saplings[1]++;
                StorageScript.Instance.money -= 10f;
            }
        }
    }
    public void BuyBirchSapling()
    {
        if (StorageScript.Instance.currentSector >= 2)
        {
            if (StorageScript.Instance.money >= 10f)
            {
                StorageScript.Instance.saplings[2]++;
                StorageScript.Instance.money -= 10f;
            }
        }
    }
}
