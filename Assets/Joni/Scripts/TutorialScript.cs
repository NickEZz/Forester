using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] bool tutorial;

    [SerializeField] int currentScene;

    [SerializeField] TutorialScene[] scenes;

    [SerializeField] float cameraSpeed;

    [SerializeField] MoveCamera cameraScript;

    [SerializeField] GameObject previousMenu;

    int amountOfTrees;
    int treesCutDown;

    // Start is called before the first frame update
    void Start()
    {
        
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
                    treesCutDown = amountOfTrees - StorageScript.Instance.treesInGame.Count;

                    scenes[currentScene].relatedObject.GetComponent<TextMeshProUGUI>().text = treesCutDown + "/5";


                    break;
            }
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
}

[System.Serializable]
public class TutorialScene
{
    public GameObject gameObject;
    public GameObject relatedObject;
    public Vector3 cameraPosition;
}

/*choose spruce sapling

plant 5-10 spruce trees

explain tree spawning system

open tool menu

explain tools, select axe or saw

cut trees down

explain cost of house

open store

explain tool upgrades/store

sell wood

close store

open build menu

give player first house for free and explain what they do

close build menu

explain buying areas

done explaining basics, good luck
*/
