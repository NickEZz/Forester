using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewareaScript : MonoBehaviour
{
    GameObject newAreaWindow;

    TextMeshProUGUI currentMultiplierText;
    TextMeshProUGUI newMultiplierText;

    float woodMultiplier;

    private void Awake()
    {
        /*
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenNewAreaWindow()
    {
        //currentMultiplierText = StorageScript.Instance.woodMultiplier;
        

        
        newAreaWindow.SetActive(true);
    }

    public void Cancel()
    {
        newAreaWindow.SetActive(false);
    }

    public void Accept()
    {
        //StorageScript.Instance.woodMultiplier = woodMultiplier;
        SaveManager.Instance.SaveGameData();
        //fade.FadeOut();
    }
}
