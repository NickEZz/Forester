using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject activeMenu;

    [SerializeField] private TextMeshProUGUI totalWoodCounter;
    [SerializeField] private TextMeshProUGUI[] moneyCounters;

    [SerializeField] private TextMeshProUGUI[] woodCounters;
    [SerializeField] private TextMeshProUGUI[] saplingCounters;

    [SerializeField] private GameObject saplingMenu;
    [SerializeField] private GameObject toolMenu;

    [SerializeField] private GameObject[] elementsToHide;

    [SerializeField] private ToolScript toolScript;
    [SerializeField] private BuildScript buildScript;

    [SerializeField] private RawImage currentBuildingIcon;
    [SerializeField] private Image[] buildingButtons;


    [SerializeField] private int lastSelectedTool;
    [SerializeField] private int lastSelectedSapling;

    [SerializeField] private RawImage currentToolIcon;
    [SerializeField] private Texture2D[] toolIcons;
    [SerializeField] private RawImage currentSaplingIcon;
    [SerializeField] private TextMeshProUGUI saplingAmountText;

    [SerializeField] private RawImage[] shopToolIcons;
    [SerializeField] private TextMeshProUGUI[] shopToolPrices;

    [SerializeField] private GameObject[] pineSaplingButtons, birchSaplingButtons;
    //[SerializeField] private GameObject[] birchSapling;

    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject exitConfirmWindow;

    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject resetConfirmation;
    [SerializeField] private Toggle fullscreenToggle;
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [SerializeField] private MoveCamera moveCamera;
    [SerializeField] private AudioManager audioManager;

    bool requestQuit;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        if (PlayerPrefs.HasKey("FullScreen"))
        {
            if (PlayerPrefs.GetInt("FullScreen") == 0) // Jos peli ei ole fullscreen
            {
                fullscreenToggle.isOn = false;
                Screen.SetResolution(PlayerPrefs.GetInt("WindowSizeX"), PlayerPrefs.GetInt("WindowSizeY"), false);
            }
            else // Jos on
            {
                fullscreenToggle.isOn = true;
            }
        }
        else
        {
            Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, fullscreenToggle);
        }
    }

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

        // Vaihtaa kirveen ja sahan kuvakkeet jos niitä on päivitetty
        toolIcons[0] = StorageScript.Instance.axes[StorageScript.Instance.currentAxeUpgrade].toolSprite;
        toolIcons[1] = StorageScript.Instance.saws[StorageScript.Instance.currentSawUpgrade].toolSprite;

        for (int i = 0; i < shopToolIcons.Length; i++)
        {
            if (StorageScript.Instance.currentAxeUpgrade == StorageScript.Instance.axes.Length - 1) // Jos on max päivitys
            {
                shopToolPrices[0].text = "Max";
                shopToolIcons[0].texture = StorageScript.Instance.axes[StorageScript.Instance.currentAxeUpgrade].toolSprite;
            }
            else
            {
                shopToolIcons[0].texture = StorageScript.Instance.axes[StorageScript.Instance.currentAxeUpgrade + 1].toolSprite;
                shopToolPrices[0].text = StorageScript.Instance.axes[StorageScript.Instance.currentAxeUpgrade + 1].toolPrice.ToString() + "€";
            }

            if (StorageScript.Instance.currentSawUpgrade == StorageScript.Instance.saws.Length - 1) // Jos on max päivitys
            {
                shopToolPrices[1].text = "Max";
                shopToolIcons[1].texture = StorageScript.Instance.saws[StorageScript.Instance.currentSawUpgrade].toolSprite;
            }
            else
            {
                shopToolIcons[1].texture = StorageScript.Instance.saws[StorageScript.Instance.currentSawUpgrade + 1].toolSprite;
                shopToolPrices[1].text = StorageScript.Instance.saws[StorageScript.Instance.currentSawUpgrade + 1].toolPrice.ToString() + "€";
            }
        }

        
        // Piilottaa saplingit jota ei voi ostaa vielä
        switch (StorageScript.Instance.currentSector)
        {
            case 0:
                if (pineSaplingButtons[0].activeSelf || pineSaplingButtons[1].activeSelf)
                {
                    pineSaplingButtons[0].SetActive(false);
                    pineSaplingButtons[1].SetActive(false);
                }
                if (birchSaplingButtons[0].activeSelf || birchSaplingButtons[1].activeSelf)
                {
                    birchSaplingButtons[0].SetActive(false);
                    birchSaplingButtons[1].SetActive(false);
                }
                break;
            case 1:
                for (int i = 0; i < pineSaplingButtons.Length; i++)
                {
                    if (!pineSaplingButtons[i].activeSelf)
                    {
                        pineSaplingButtons[i].SetActive(true);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < pineSaplingButtons.Length; i++)
                {
                    if (!pineSaplingButtons[i].activeSelf)
                    {
                        pineSaplingButtons[i].SetActive(true);
                    }
                }
                for (int i = 0; i < birchSaplingButtons.Length; i++)
                {
                    if (!birchSaplingButtons[i].activeSelf)
                    {
                        birchSaplingButtons[i].SetActive(true);
                    }
                }
                break;
        }

        if (toolScript.tool < 2)
        {   
            currentToolIcon.texture = toolIcons[toolScript.tool];
        }
        else
        {
            currentSaplingIcon.texture = StorageScript.Instance.saplingTypes[toolScript.tool - 2].sprite;
            saplingAmountText.text = StorageScript.Instance.saplings[0].ToString();
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

        Screen.fullScreen = fullscreenToggle.isOn;
        if (fullscreenToggle.isOn)
        {
            Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, fullscreenToggle);
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
            PlayerPrefs.SetInt("WindowSizeX", Screen.width);
            PlayerPrefs.SetInt("WindowSizeY", Screen.height);
        }

        if (requestQuit)
        {
            if (SaveManager.Instance.saving == false)
            {
                print("quit");
                Application.Quit();
            }
        }
    }

    void ClosePrevMenu(GameObject openMenu)
    {
        
    }

    public void ToggleToolMenu()
    {
        toolScript.tool = lastSelectedTool;
        saplingMenu.SetActive(false);
        toolMenu.SetActive(!toolMenu.activeSelf);
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void ToggleSaplingMenu()
    {
        toolScript.tool = lastSelectedSapling;
        toolMenu.SetActive(false);
        saplingMenu.SetActive(!saplingMenu.activeSelf);
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectAxe()
    {
        toolScript.tool = 0;
        lastSelectedTool = 0;
        currentToolIcon.texture = toolIcons[0];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectSaw()
    {
        toolScript.tool = 1;
        lastSelectedTool = 1;
        currentToolIcon.texture = toolIcons[1];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectSpruceSapling()
    {
        toolScript.tool = 2;
        lastSelectedSapling = 2;
        ToggleSaplingMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectPineSapling()
    {
        toolScript.tool = 3;
        lastSelectedSapling = 3;
        ToggleSaplingMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectBirchSapling()
    {
        toolScript.tool = 4;
        lastSelectedSapling = 4;
        ToggleSaplingMenu();
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
        if (StorageScript.Instance.money >= StorageScript.Instance.axes[StorageScript.Instance.currentAxeUpgrade + 1].toolPrice)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            StorageScript.Instance.currentAxeUpgrade++;
            StorageScript.Instance.money -= StorageScript.Instance.axes[StorageScript.Instance.currentAxeUpgrade].toolPrice;
            toolScript.UpdateTool();
        }
    }

    public void BuySawUpgrade()
    {
        if (StorageScript.Instance.money >= StorageScript.Instance.saws[StorageScript.Instance.currentSawUpgrade + 1].toolPrice)
        {
            audioManager.PlaySound("transactionsound", Vector3.zero);
            StorageScript.Instance.currentSawUpgrade++;
            StorageScript.Instance.money -= StorageScript.Instance.saws[StorageScript.Instance.currentSawUpgrade].toolPrice;
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
    public void BuyBirchSapling(float amount)
    {
        if (StorageScript.Instance.currentSector >= 2)
        {
            if (StorageScript.Instance.money >= 200f * amount)
            {
                audioManager.PlaySound("transactionsound", Vector3.zero);
                StorageScript.Instance.saplings[2] += (int)amount;
                StorageScript.Instance.money -= 200f * amount;
            }
        }
    }

    public void ToggleGameMenu()
    {
        optionsMenu.SetActive(false);
        gameMenu.SetActive(!gameMenu.activeSelf);
        exitConfirmWindow.SetActive(false);
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void ToggleOptionsMenu()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        //SaveManager.Instance.SaveSetting("Volume", volumeSlider.value);
    }

    public void SaveSensitivity()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
        //SaveManager.Instance.SaveSetting("Sensitivity", sensitivitySlider.value);
    }

    public void ResetSaveGameButton()
    {
        resetConfirmation.SetActive(true);
        audioManager.PlaySound("click", Vector3.zero);
    }
    
    public void ConfirmReset(bool choice)
    {
        if (choice)
        {
            print("reset");
            SaveManager.Instance.ResetSaveData();
        }
        else
        {
            print("canceled reset");
            resetConfirmation.SetActive(false);
            audioManager.PlaySound("click", Vector3.zero);
        }
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
            print("saved before quit");
            SaveManager.Instance.SaveGameData();
            requestQuit = true;
        }
        else
        {
            print("canceled quit");
            exitConfirmWindow.SetActive(false);
            audioManager.PlaySound("click", Vector3.zero);
        }
    }
}
