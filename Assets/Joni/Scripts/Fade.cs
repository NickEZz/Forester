using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public bool fading;
    [SerializeField] float fadingDirection;
    [SerializeField] float fadingSpeed;
    [SerializeField] CanvasGroup fadeElement;

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            fadeElement.alpha += fadingDirection * fadingSpeed * Time.deltaTime;
            if (fadeElement.alpha == 0 || fadeElement.alpha == 1)
            {
                fading = false;
            }
        }
    }

    public void FadeIn()
    {
        fadeElement.alpha = 1f;
        fadingDirection = -1f;
        fading = true;
    }

    public void FadeOut()
    {
        fadeElement.alpha = 0;
        fadingDirection = 1f;
        fading = true;
    }
}
