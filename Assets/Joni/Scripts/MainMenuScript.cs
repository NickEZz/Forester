using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] CanvasGroup text;
    [SerializeField] float textFadeSpeed;
    [SerializeField] float textFadeDirection;
    [SerializeField] float textMinAlpha;

    Fade fade;
    bool pressedKey;

    // Start is called before the first frame update
    void Start()
    {
        fade = GetComponent<Fade>();
        fade.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            fade.FadeOut();
            pressedKey = true;
        }

        if (pressedKey)
        {
            if (!fade.fading)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        text.alpha += textFadeDirection * textFadeSpeed * Time.deltaTime;
        if (text.alpha >= 1)
        {
            textFadeDirection = -1f;
        }
        if (text.alpha <= textMinAlpha)
        {
            textFadeDirection = 1f;
        }
    }

    public void ExitGame()
    {
        print("Quit");
        Application.Quit();
    }
}
