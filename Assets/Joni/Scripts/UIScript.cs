using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI woodCounter;
    [SerializeField] TextMeshProUGUI moneyCounter;

    

    [SerializeField] ToolScript toolScript;

    [SerializeField] GameObject toolMenu;

    [SerializeField] Image currentToolIcon;
    [SerializeField] Image[] toolIcons;

    private void Update()
    {
        woodCounter.text = Mathf.Round(StorageScript.Instance.wood).ToString();
        moneyCounter.text = Mathf.Round(StorageScript.Instance.money).ToString();
    }

    public void ToggleToolMenu()
    {
        toolMenu.SetActive(!toolMenu.activeSelf);
    }

    public void SelectAxe()
    {
        toolScript.tool = 0;
        ToggleToolMenu();
    }

    public void SelectSaw()
    {
        toolScript.tool = 1;
        ToggleToolMenu();
    }

    public void SelectSpruceSapling()
    {
        toolScript.tool = 2;
        ToggleToolMenu();
    }
}
