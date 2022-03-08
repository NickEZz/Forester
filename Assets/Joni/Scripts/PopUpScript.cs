using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] GameObject popUp;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI text;

    AreaScript target;
    [SerializeField] MapManager mapManager;

    [SerializeField] LayerMask unpurchasedAreaMask;

    [SerializeField] Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(input);

        RaycastHit mouse;

        if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(ray, out mouse, 100f, unpurchasedAreaMask))
            {
                if (mouse.collider.gameObject.layer == 10)
                {
                    target = mouse.collider.GetComponent<AreaScript>();

                    if (target.sector <= StorageScript.Instance.currentSector)
                    {
                        text.text = target.price.ToString() + "€";
                        popUp.SetActive(true);
                        popUp.transform.position = new Vector3(mouse.point.x, 2f, mouse.point.z);
                    }
                    else if (target.sector == StorageScript.Instance.currentSector + 1)
                    {
                        if (mapManager.sectorsOwned[StorageScript.Instance.currentSector] == mapManager.sectorAreaAmounts[StorageScript.Instance.currentSector]) //
                        {
                            text.text = target.price.ToString() + "€";
                            popUp.SetActive(true);
                            popUp.transform.position = new Vector3(mouse.point.x, 2f, mouse.point.z);
                        }
                    }
                }
            }
            else
            {
                popUp.SetActive(false);
            }
        }
        
    }

    public void ButtonPushed()
    {
        bool bought = target.BuyArea();
        if (bought)
        {
            popUp.SetActive(false);
            mapManager.sectorsOwned[StorageScript.Instance.currentSector]++;
        }
        else
        {
            text.text = "Not enough money!";
        }
        
    }
}
