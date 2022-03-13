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
    [SerializeField] TextMeshProUGUI[] saplingCounters;

    [SerializeField] GameObject[] elementsToHide;

    [SerializeField] ToolScript toolScript;
    [SerializeField] BuildScript buildScript;

    [SerializeField] RawImage currentBuildingIcon;
    [SerializeField] Image[] buildingButtons;

    [SerializeField] RawImage currentToolIcon;
    [SerializeField] Texture2D[] toolIcons;

    [SerializeField] RawImage[] shopToolIcons;
    [SerializeField] TextMeshProUGUI[] shopToolPrices;

    [SerializeField] GameObject pineSapling;
    [SerializeField] GameObject birchSapling;

    [SerializeField] AudioManager audioManager;


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
        for (int i = 0; i < saplingCounters.Length; i++)
        {
            saplingCounters[i].text = StorageScript.Instance.saplings[i].ToString();
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
        //samanlainen rakennuksille

        currentToolIcon.texture = toolIcons[toolScript.tool];
        //currentBuildingIcon.texture = buildingIcons[buildScript.selectedBuilding];

        if (buildScript.buildMode)
        {
            for (int i = 0; i < elementsToHide.Length - 1; i++)
            {
                elementsToHide[i].SetActive(false);
            }
            elementsToHide[3].SetActive(true);

            for (int i = 0; i < buildingButtons.Length; i++)
            {
                if (i == buildScript.selectedBuilding)
                {
                    buildingButtons[i].color = Color.gray;
                }
                else
                {
                    buildingButtons[i].color = Color.white;
                }
            }
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
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectAxe()
    {
        toolScript.tool = 0;
        currentToolIcon.texture = toolIcons[0];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectSaw()
    {
        toolScript.tool = 1;
        currentToolIcon.texture = toolIcons[1];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectSpruceSapling()
    {
        toolScript.tool = 2;
        currentToolIcon.texture = toolIcons[2];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectPineSapling()
    {
        toolScript.tool = 3;
        currentToolIcon.texture = toolIcons[2];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectBirchSapling()
    {
        toolScript.tool = 4;
        currentToolIcon.texture = toolIcons[2];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void ToggleBuildMode()
    {
        buildScript.buildMode = !buildScript.buildMode;
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectBuildingOne()
    {
        buildScript.selectedBuilding = 0;
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectBuildingTwo()
    {
        buildScript.selectedBuilding = 1;
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectBuildingThree()
    {
        buildScript.selectedBuilding = 2;
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void ToggleStore()
    {
        elementsToHide[4].SetActive(!elementsToHide[4].activeSelf);
        buildScript.inStore = elementsToHide[4].activeSelf;
        audioManager.PlaySound("click", Vector3.zero);
    }
    
    public void SellOneSpruce()
    {
        if (StorageScript.Instance.wood[0] >= 1)
        {
            StorageScript.Instance.wood[0]--;
            StorageScript.Instance.money += 60f;
        }
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SellTenSpruce()
    {
        if (StorageScript.Instance.wood[0] >= 10)
        {
            StorageScript.Instance.wood[0] -= 10f;
            StorageScript.Instance.money += 10f * 60f;
        }
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SellOnePine()
    {
        if (StorageScript.Instance.wood[1] >= 1)
        {
            StorageScript.Instance.wood[1]--;
            StorageScript.Instance.money += 130f;
        }
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SellTenPine()
    {
        if (StorageScript.Instance.wood[1] >= 10)
        {
            StorageScript.Instance.wood[1] -= 10f;
            StorageScript.Instance.money += 10f * 130f;
        }
        audioManager.PlaySound("click", Vector3.zero);
    }
    public void SellOneBirch()
    {
        if (StorageScript.Instance.wood[2] >= 1)
        {
            StorageScript.Instance.wood[2]--;
            StorageScript.Instance.money += 200f;
        }
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SellTenBirch()
    {
        if (StorageScript.Instance.wood[2] >= 10)
        {
            StorageScript.Instance.wood[2] -= 10f;
            StorageScript.Instance.money += 10f * 200f;
        }
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void BuyAxeUpgrade()
    {
        if (StorageScript.Instance.money >= toolScript.axes[toolScript.currentAxeUpgrade + 1].toolPrice)
        {
            toolScript.currentAxeUpgrade++;
            StorageScript.Instance.money -= toolScript.axes[toolScript.currentAxeUpgrade].toolPrice;
            toolScript.UpdateTool();
        }
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void BuySawUpgrade()
    {
        if (StorageScript.Instance.money >= toolScript.saws[toolScript.currentSawUpgrade + 1].toolPrice)
        {
            toolScript.currentSawUpgrade++;
            StorageScript.Instance.money -= toolScript.saws[toolScript.currentSawUpgrade].toolPrice;
            toolScript.UpdateTool();
        }
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void BuySpruceSapling()
    {
        if (StorageScript.Instance.money >= 10f)
        {
            StorageScript.Instance.saplings[0]++;
            StorageScript.Instance.money -= 10f;
        }
        audioManager.PlaySound("click", Vector3.zero);
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
        audioManager.PlaySound("click", Vector3.zero);
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
        audioManager.PlaySound("click", Vector3.zero);
    }
}
