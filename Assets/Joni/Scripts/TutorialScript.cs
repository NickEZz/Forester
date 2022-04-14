using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialScript : MonoBehaviour
{
    public bool tutorial;

    [SerializeField] int currentScene;

    [SerializeField] TutorialScene[] scenes;

    [SerializeField] float cameraSpeed;

    [SerializeField] MoveCamera cameraScript;

    [SerializeField] GameObject previousMenu;

    int amountOfTrees;
    public int treesCutDown;

    public float originalWoodCost;
    public float originalMoneyCost;

    // Start is called before the first frame update
    void Start()
    {
        originalMoneyCost = StorageScript.Instance.buildingTypes[0].moneyCost;
        originalWoodCost = StorageScript.Instance.buildingTypes[0].woodCost[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial)
        {
            switch (currentScene)
            {
                case 0:
                    if (Input.GetMouseButtonUp(0))
                    {
                        NextScene();
                    }
                    break;
                case 1:
                    if (Input.GetMouseButtonUp(0))
                    {
                        NextScene();
                    }
                    break;
                case 2:
                    if (Input.GetMouseButtonUp(0))
                    {
                        NextScene();
                    }
                    break;
                case 3:
                    if (scenes[currentScene].relatedObject.activeSelf)
                    {
                        previousMenu = scenes[currentScene].relatedObject;
                        NextScene();
                    }
                    break;
                case 4:
                    if (scenes[currentScene].relatedObject.GetComponent<ToolScript>().tool == 2 && !previousMenu.activeSelf)
                    {
                        NextScene();
                    }
                    break;
                case 5:
                    scenes[currentScene].relatedObject.GetComponent<TextMeshProUGUI>().text = StorageScript.Instance.treesInGame.Count + "/10";

                    if (StorageScript.Instance.treesInGame.Count >= 10)
                    {
                        NextScene();
                    }
                    break;
                case 6:
                    for (int i = 0; i < StorageScript.Instance.treesInGame.Count; i++)
                    {
                        TreeScript treeScript = StorageScript.Instance.treesInGame[i].GetComponent<TreeScript>();

                        treeScript.growthEveryFrame = 0.02f;

                        if (treeScript.adultTree)
                        {
                            amountOfTrees = StorageScript.Instance.treesInGame.Count;

                            NextScene();
                        }
                    }
                    break;
                case 7:
                    if (Input.GetMouseButtonUp(0))
                    {
                        NextScene();
                    }
                    break;
                case 8:
                    if (scenes[currentScene].relatedObject.activeSelf)
                    {
                        previousMenu = scenes[currentScene].relatedObject.gameObject;
                        NextScene();
                    }
                    

                    /*
                    scenes[currentScene].relatedObject.GetComponent<TextMeshProUGUI>().text = treesCutDown + "/5";

                    if (treesCutDown >= 5)
                    {
                        NextScene();
                    }
                    */
                    break;
                case 9:
                    if (scenes[currentScene].relatedObject.GetComponent<ToolScript>().tool < 2 && !previousMenu.activeSelf)
                    {
                        NextScene();
                    }

                    /*
                    if (scenes[currentScene].relatedObject.activeSelf)
                    {
                        NextScene();
                    }
                    */
                    break;
                case 10:
                    scenes[currentScene].relatedObject.GetComponent<TextMeshProUGUI>().text = treesCutDown + "/5";

                    if (treesCutDown >= 5)
                    {
                        NextScene();
                    }
                    /*
                    if (Input.GetMouseButtonUp(0))
                    {
                        NextScene();
                    }
                    */
                    break;
                case 11:
                    if (scenes[currentScene].relatedObject.activeSelf)
                    {
                        NextScene();
                    }

                    /*
                    scenes[currentScene].relatedObject.SetActive(true);

                    if (StorageScript.Instance.wood[0] <= 0)
                    {
                        NextScene();
                    }
                    */
                    break;
                case 12:
                    scenes[currentScene].relatedObject.SetActive(true);

                    if (Input.GetMouseButtonUp(0))
                    {
                        NextScene();
                    }
                    break;
                case 13:
                    scenes[currentScene].relatedObject.SetActive(true);

                    if (StorageScript.Instance.wood[0] <= 0)
                    {
                        NextScene();
                    }
                    break;
                case 14:
                    if (!scenes[currentScene].relatedObject.activeSelf)
                    {
                        NextScene();
                    }
                    break;
                case 15:
                    if (scenes[currentScene].relatedObject.activeSelf)
                    {
                        NextScene();
                    }
                    break;
                case 16:
                    scenes[currentScene].relatedObject.SetActive(true);

                    StorageScript.Instance.buildingTypes[0].moneyCost = 0f;
                    StorageScript.Instance.buildingTypes[0].woodCost[0] = 0f;

                    if (StorageScript.Instance.buildingsInGame.Count > 0)
                    {
                        StorageScript.Instance.buildingTypes[0].moneyCost = originalMoneyCost;
                        StorageScript.Instance.buildingTypes[0].woodCost[0] = originalWoodCost;
                        NextScene();
                    }
                    break;
                case 17:
                    if (Input.GetMouseButtonDown(0))
                    {
                        NextScene();
                    }
                    break;
                case 18:
                    if (!scenes[currentScene].relatedObject.activeSelf)
                    {
                        NextScene();
                    }
                    break;
                case 19:
                    if (Input.GetMouseButtonUp(0))
                    {
                        NextScene();
                    }
                    break;
                case 20:
                    if (Input.GetMouseButton(0))
                    {
                        StopTutorial();
                    }
                    break;
            }
        }
        else
        {
            StorageScript.Instance.buildingTypes[0].moneyCost = originalMoneyCost;
            StorageScript.Instance.buildingTypes[0].woodCost[0] = originalWoodCost;
        }
    }

    public void StartTutorial()
    {
        tutorial = true;
        currentScene = 0;
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].gameObject.SetActive(false);
        }
        scenes[currentScene].gameObject.SetActive(true);
    }

    public void NextScene()
    {
        scenes[currentScene].gameObject.SetActive(false);
        currentScene++;
        scenes[currentScene].gameObject.SetActive(true);
    }

    public void PreviousScene()
    {
        scenes[currentScene].gameObject.SetActive(false);
        currentScene--;
        scenes[currentScene].gameObject.SetActive(true);
    }

    public void StopTutorial()
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].gameObject.SetActive(false);
        }
        tutorial = false;
    }
}

[System.Serializable]
public class TutorialScene
{
    public GameObject gameObject;
    public GameObject relatedObject;
    public Vector3 cameraPosition;
}