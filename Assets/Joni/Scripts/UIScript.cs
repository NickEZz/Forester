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


    [SerializeField] private GameObject buildingMenu;
    [SerializeField] private Image[] buildingButtons;

    [SerializeField] private GameObject storeButton;
    [SerializeField] private GameObject store;

    [SerializeField] private GameObject woodStoreMenu;
    [SerializeField] private GameObject toolStoreMenu;
    [SerializeField] private GameObject saplingStoreMenu;

    [SerializeField] private GameObject toolButtons;
    [SerializeField] private GameObject saplingMenu;
    [SerializeField] private GameObject toolMenu;

    [SerializeField] private Image[] test;

    [SerializeField] private ToolScript toolScript;
    [SerializeField] private BuildScript buildScript;

    [SerializeField] private RawImage currentBuildingIcon;

    [SerializeField] private int lastSelectedTool;
    [SerializeField] private int lastSelectedSapling;

    [SerializeField] private RawImage currentToolIcon;
    [SerializeField] private Texture2D[] toolIcons;
    [SerializeField] private RawImage[] toolMenuToolIcons;
    [SerializeField] private RawImage currentSaplingIcon;
    [SerializeField] private TextMeshProUGUI saplingAmountText;

    [SerializeField] private RawImage[] shopToolIcons;
    [SerializeField] private TextMeshProUGUI[] shopToolPrices;

    [SerializeField] private GameObject[] lockedPineCovers, lockedBirchCovers, lockedChainsawCovers;
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

        saplingAmountText.text = StorageScript.Instance.saplings[0].ToString();
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

        toolMenuToolIcons[0].texture = toolIcons[0];
        toolMenuToolIcons[1].texture = toolIcons[1];

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

        if (StorageScript.Instance.currentChainsawUpgrade > 0)
        {
            shopToolPrices[2].text = "Bought";
        }
        else
        {
            shopToolPrices[2].text = StorageScript.Instance.chainsaw.toolPrice + "€";
        }
        
        // Piilottaa saplingit jota ei voi ostaa vielä
        switch (StorageScript.Instance.currentSector)
        {
            case 0:
                if (!lockedPineCovers[0].activeSelf || !lockedPineCovers[1].activeSelf)
                {
                    lockedPineCovers[0].SetActive(true);
                    lockedPineCovers[1].SetActive(true);
                    lockedPineCovers[2].SetActive(true);
                }
                if (!lockedBirchCovers[0].activeSelf || !lockedBirchCovers[1].activeSelf)
                {   
                    lockedBirchCovers[0].SetActive(true);
                    lockedBirchCovers[1].SetActive(true);
                    lockedBirchCovers[2].SetActive(true);
                }
                break;
            case 1:
                for (int i = 0; i < lockedPineCovers.Length; i++)
                {
                    if (lockedPineCovers[i].activeSelf)
                    {
                        lockedPineCovers[i].SetActive(false);
                    }
                }
                if (!lockedBirchCovers[0].activeSelf || lockedBirchCovers[1].activeSelf)
                {
                    lockedBirchCovers[0].SetActive(true);
                    lockedBirchCovers[1].SetActive(true);
                }
                break;
            case 2:
                for (int i = 0; i < lockedPineCovers.Length; i++)
                {
                    if (lockedPineCovers[i].activeSelf)
                    {
                        lockedPineCovers[i].SetActive(false);
                    }
                }
                for (int i = 0; i < lockedBirchCovers.Length; i++)
                {
                    if (lockedBirchCovers[i].activeSelf)
                    {
                        lockedBirchCovers[i].SetActive(false);
                    }
                }
                break;
        }

        if (StorageScript.Instance.currentAxeUpgrade == StorageScript.Instance.axes.Length - 1 && StorageScript.Instance.currentSawUpgrade == StorageScript.Instance.saws.Length - 1)
        {
            if (lockedChainsawCovers[0].activeSelf)
            {
                lockedChainsawCovers[0].SetActive(false);
            }
        }

        if (StorageScript.Instance.currentChainsawUpgrade > 0)
        {
            if (lockedChainsawCovers[1].activeSelf)
            {
                lockedChainsawCovers[1].SetActive(false);
            }
        }

        // Kun menee buildmodeen, piilottaa joitain ui elementtejä ja tuo muita esille
        if (buildScript.buildMode)
        {
            storeButton.SetActive(false);
            toolButtons.SetActive(false);
            toolMenu.SetActive(false);
            saplingMenu.SetActive(false);
            buildingMenu.SetActive(true);

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
            storeButton.SetActive(true);
            toolButtons.SetActive(true);
            buildingMenu.SetActive(false);

            if (toolScript.tool < 3)
            {
                currentToolIcon.texture = toolIcons[toolScript.tool];
                test[1].color = Color.white;
                test[0].color = Color.gray;
            }
            else
            {
                currentSaplingIcon.texture = StorageScript.Instance.saplingTypes[toolScript.tool - 3].sprite;
                saplingAmountText.text = StorageScript.Instance.saplings[toolScript.tool - 3].ToString();
                test[0].color = Color.white;
                test[1].color = Color.gray;
            }
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

    public void Chainsaw()
    {
        toolScript.tool = 2;
        lastSelectedTool = 2;
        currentToolIcon.texture = toolIcons[2];
        ToggleToolMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectSpruceSapling()
    {
        toolScript.tool = 3;
        lastSelectedSapling = 3;
        ToggleSaplingMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectPineSapling()
    {
        toolScript.tool = 4;
        lastSelectedSapling = 4;
        ToggleSaplingMenu();
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void SelectBirchSapling()
    {
        toolScript.tool = 5;
        lastSelectedSapling = 5;
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
        store.SetActive(!store.activeSelf);
        buildScript.inStore = store.activeSelf;
        audioManager.PlaySound("click", Vector3.zero);
    }

    public void OpenStoreWoodMenu()
    {
        toolStoreMenu.SetActive(false);
        saplingStoreMenu.SetActive(false);
        woodStoreMenu.SetActive(true);
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

    public void OpenStoreToolMenu()
    {
        saplingStoreMenu.SetActive(false);
        woodStoreMenu.SetActive(false);
        toolStoreMenu.SetActive(true);
        audioManager.PlaySound("click", Vector3.zero);
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

    public void BuyChainsaw()
    {
        if (StorageScript.Instance.currentChainsawUpgrade == 0)
        {
            if (StorageScript.Instance.money >= StorageScript.Instance.chainsaw.toolPrice)
            {
                audioManager.PlaySound("transactionsound", Vector3.zero);
                StorageScript.Instance.currentChainsawUpgrade++;
                StorageScript.Instance.money -= StorageScript.Instance.chainsaw.toolPrice;
                toolScript.UpdateTool();

                // price text to "Bought"
            }
        }
    }

    public void OpenStoreSaplingMenu()
    {
        toolStoreMenu.SetActive(false);
        woodStoreMenu.SetActive(false);
        saplingStoreMenu.SetActive(true);
        audioManager.PlaySound("click", Vector3.zero);
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
