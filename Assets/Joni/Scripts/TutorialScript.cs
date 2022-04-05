using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] bool tutorial;

    [SerializeField] int currentScene;

    [SerializeField] TutorialScene[] scenes;

    [SerializeField] float cameraSpeed;

    [SerializeField] MoveCamera cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial)
        {
            if (scenes[currentScene].moveCamera)
            {
                if (scenes[currentScene].trackingObject)
                {
                    /*
                    Vector3 dir = (scenes[currentScene].cameraPosition - cameraScript.transform.position) * cameraSpeed * Time.deltaTime;

                    cameraScript.transform.position += dir;
                    cameraScript.targetPos = scenes[currentScene].cameraPosition.y;
                    */
                }
                else
                {
                    Vector3 dir = (scenes[currentScene].cameraPosition - cameraScript.transform.position) * cameraSpeed * Time.deltaTime;

                    cameraScript.transform.position += dir;
                    cameraScript.targetPos = scenes[currentScene].cameraPosition.y;
                }
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
    public bool moveCamera;

    public bool trackingObject;
    public Transform trackedObject;
    public Vector3 cameraPosition;
}
