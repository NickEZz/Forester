using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalWoodCounter;
    [SerializeField] private TextMeshProUGUI[] moneyCounters;

    [SerializeField] private TextMeshProUGUI[] woodCounters;
    [SerializeField] private TextMeshProUGUI[] saplingCounters;

    [SerializeField] private GameObject[] elementsToHide;

    [SerializeField] private ToolScript toolScript;
    [SerializeField] private BuildScript buildScript;

    [SerializeField] private RawImage currentBuildingIcon;
    [SerializeField] private Image[] buildingButtons;

    [SerializeField] private RawImage currentToolIcon;
    [SerializeField] private Texture2D[] toolIcons;
    [SerializeField] private TextMeshProUGUI saplingNameText;
    [SerializeField] private TextMeshProUGUI saplingAmountText;

    [SerializeField] private RawImage[] shopToolIcons;
    [SerializeField] private TextMeshProUGUI[] shopToolPrices;

    [SerializeField] private GameObject pineSapling;
    [SerializeField] private GameObject birchSapling;

    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject exitConfirmWindow;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;

    [SerializeField] private MoveCamera moveCamera;
    [SerializeField] private AudioManager audioManager;


    private void Update()
    {
        // Kauppa ja raha/puu counterit
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

        // Piilottaa saplingit jota ei voi ostaa vielä
        switch (StorageScript.Instance.currentSector)
        {
            case 0:
                if (pineSapling.activeSelf)
                {
                    pineSapling.SetActive(false);
                }
                /*
                if (birchSapling.activeSelf)
                {
                    birchSapling.SetActive(false);
                }
                */
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
                /*if (!birchSapling.activeSelf)
                {
                    //birchSapling.SetActive(true);
                }*/
                break;
        }
        
        // Vaihtaa työkalun kuvakkeen oikeaksi kuvakkeeksi
        //currentToolIcon.texture = toolIcons[toolScript.tool];
        switch (toolScript.tool)
        {
            default:
                saplingNameText.gameObject.SetActive(false);
                currentToolIcon.gameObject.SetActive(true);
                currentToolIcon.texture = toolIcons[toolScript.tool];
                break;
            case 2:
                currentToolIcon.gameObject.SetActive(false);
                saplingNameText.gameObject.SetActive(true);
                saplingNameText.text = "Spruce sapling";
                saplingAmountText.text = StorageScript.Instance.saplings[0].ToString();
                break;
            case 3:
                currentToolIcon.gameObject.SetActive(false);
                saplingNameText.gameObject.SetActive(true);
                saplingNameText.text = "Pine sapling";
                saplingAmountText.text = StorageScript.Instance.saplings[1].ToString();
                break;
            
        }


        // Kun menee buildmodeen, piilottaa joitain ui elementtejä ja tuo muita esille
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

        // Volume ja sensitivity sliderit
        audioManager.volumeMaster = volumeSlider.value;
        moveCamera.sensitivity = sensitivitySlider.value;
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
    
    public void SellSpruce(float amount)
    {
        if (StorageScript.Instance.wood[0] >= amount)
        {
            StorageScript.Instance.wood[0] -= amount;
            StorageScript.Instance.money += amount * 60f;
            audioManager.PlaySound("transactionsound", Vector3.zero);
        }
        
    }

    public void SellAllSpruce()
    {
        if (StorageScript.Instance.wood[0] > 0)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            StorageScript.Instance.money += StorageScript.Instance.wood[0] * 60f;
            StorageScript.Instance.wood[0] -= StorageScript.Instance.wood[0];

        }

    }

    public void SellPine(float amount)
    {
        if (StorageScript.Instance.wood[1] >= amount)
        {
            StorageScript.Instance.wood[1] -= amount;
            StorageScript.Instance.money += amount * 130f;
            audioManager.PlaySound("transactionsound", Vector3.zero);
        }
        
    }

    public void SellAllPine()
    {
        if (StorageScript.Instance.wood[1] > 0)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            StorageScript.Instance.money += StorageScript.Instance.wood[1] * 130f;
            StorageScript.Instance.wood[1] -= StorageScript.Instance.wood[1];

        }
    }

    public void SellBirch(float amount)
    {
        if (StorageScript.Instance.wood[2] >= amount)
        {
            StorageScript.Instance.wood[2] -= amount;
            StorageScript.Instance.money += amount * 200f;
            audioManager.PlaySound("transactionsound", Vector3.zero);
        }
    }

    public void SellAllBirch()
    {
        if (StorageScript.Instance.wood[2] > 0)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            StorageScript.Instance.money += StorageScript.Instance.wood[2] * 200f;
            StorageScript.Instance.wood[2] -= StorageScript.Instance.wood[2];
            
        }
    }

    public void BuyAxeUpgrade()
    {
        if (StorageScript.Instance.money >= toolScript.axes[toolScript.currentAxeUpgrade + 1].toolPrice)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            toolScript.currentAxeUpgrade++;
            StorageScript.Instance.money -= toolScript.axes[toolScript.currentAxeUpgrade].toolPrice;
            toolScript.UpdateTool();
        }
    }

    public void BuySawUpgrade()
    {
        if (StorageScript.Instance.money >= toolScript.saws[toolScript.currentSawUpgrade + 1].toolPrice)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            toolScript.currentSawUpgrade++;
            StorageScript.Instance.money -= toolScript.saws[toolScript.currentSawUpgrade].toolPrice;
            toolScript.UpdateTool();
        }
        
    }

    public void BuySpruceSapling(float amount)
    {
        if (StorageScript.Instance.money >= 50f * amount)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            StorageScript.Instance.saplings[0] += (int)amount;
            StorageScript.Instance.money -= 50f * amount;
        }
        
    }
    public void BuyPineSapling(float amount)
    {
        if (StorageScript.Instance.currentSector >= 1)
        {
            if (StorageScript.Instance.money >= 100f * amount)
            {
                audioManager.PlaySound("transactionsound", Vector3.zero);
                StorageScript.Instance.saplings[1] += (int)amount;
                StorageScript.Instance.money -= 100f * amount;
            }
        }
        
    }
    public void BuyBirchSapling()
    {
        if (StorageScript.Instance.currentSector >= 2)
        {
            if (StorageScript.Instance.money >= 2000f)
            {
                audioManager.PlaySound("transactionsound", Vector3.zero);
                StorageScript.Instance.saplings[2]++;
                StorageScript.Instance.money -= 2000f;
            }
        }
    }

    public void ToggleGameMenu()
    {
        gameMenu.SetActive(!gameMenu.activeSelf);
        exitConfirmWindow.SetActive(false);
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void ExitButton()
    {
        exitConfirmWindow.SetActive(true);
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void ConfirmExit(bool choice)
    {
        if (choice)
        {
            print("quit");
            Application.Quit();
        }
        else
        {
            print("canceled quit");
            exitConfirmWindow.SetActive(false);
            audioManager.PlaySound("click", Vector3.zero);
        }
    }
}
