using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeveloperScript : MonoBehaviour
{
    [SerializeField] bool devMode;
    [SerializeField] string[] parameters;

    [SerializeField] GameObject hud;
    [SerializeField] GameObject console;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            console.SetActive(!console.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            //hud.SetActive(!hud.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Return)) // Kun painaa enteriä consoleen
        {
            string input = console.GetComponent<TMP_InputField>().text; // Ottaa pelaajan inputin
            string[] split = input.Split(' '); // Jakaa tekstin osiin
            parameters = split; // tallentaa osat 

            Invoke(parameters[0], 0f); // Käyttää komennon minkä pelaaja syötti

            console.GetComponent<TMP_InputField>().text = "";
        }
    }

    void Cheats()
    {
        devMode = bool.Parse(parameters[1]);
    }

    void AddResource()
    {
        if (devMode)
        {
            switch (parameters[1])
            {
                case "money":
                    StorageScript.Instance.money += float.Parse(parameters[2]);
                    break;

                case "spruce":
                    StorageScript.Instance.wood[0] += float.Parse(parameters[2]);
                    break;

                case "pine":
                    StorageScript.Instance.wood[1] += float.Parse(parameters[2]);
                    break;

                case "birch":
                    StorageScript.Instance.wood[2] += float.Parse(parameters[2]);
                    break;

                case "sprucesapling":
                    StorageScript.Instance.saplings[0] += int.Parse(parameters[2]);
                    break;

                case "pinesapling":
                    StorageScript.Instance.saplings[0] += int.Parse(parameters[2]);
                    break;

                case "birchsapling":
                    StorageScript.Instance.saplings[0] += int.Parse(parameters[2]);
                    break;
            }
        }
    } 

    void TimeScale()
    {
        if (devMode)
        {
            Time.timeScale = float.Parse(parameters[1]);
        }
    }

    void ToggleUI()
    {
        if (devMode)
        {
            hud.SetActive(!hud.activeSelf);
        }
    }
}
