using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartupAlert : MonoBehaviour
{
    [SerializeField] GameObject alertWindow;
    [SerializeField] TextMeshProUGUI alertText;

    public void ShowAlert(float totalWood)
    {
        alertWindow.SetActive(true);
        alertText.text = "You earned a total of\n " + totalWood + " wood\n while you were offline!";
    }

    public void CloseAlert()
    {
        alertWindow.SetActive(false);
    }
}
