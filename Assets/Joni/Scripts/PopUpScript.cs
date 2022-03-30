using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PopUpScript : MonoBehaviour
{
    // Area popup
    [SerializeField] GameObject areaPopUp;
    [SerializeField] Button areaPopUpButton;
    [SerializeField] TextMeshProUGUI areaPopUpText;

    AreaScript target;
    [SerializeField] MapManager mapManager;

    // house upgrade popup
    [SerializeField] GameObject upgradePopUp;
    [SerializeField] TextMeshProUGUI upgradePopUpText;
    [SerializeField] TextMeshProUGUI upgradePopUpButtonText;
    HouseScript targetHouse;

    [SerializeField] string[] woodNames;

    [SerializeField] BuildScript buildScript;

    [SerializeField] LayerMask targetMask;

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
            if (Physics.Raycast(ray, out mouse, 100f, targetMask))
            {
                if (mouse.collider.gameObject.layer == 10)
                {
                    upgradePopUp.SetActive(false);

                    target = mouse.collider.GetComponent<AreaScript>();

                    if (target.sector <= StorageScript.Instance.currentSector)
                    {
                        areaPopUpText.text = target.price.ToString() + "€";
                        areaPopUp.SetActive(true);
                        areaPopUp.transform.position = new Vector3(mouse.point.x, 2f, mouse.point.z);
                    }
                    else if (target.sector == StorageScript.Instance.currentSector + 1)
                    {
                        if (mapManager.sectorsOwned[StorageScript.Instance.currentSector] == mapManager.sectorAreaAmounts[StorageScript.Instance.currentSector]) //
                        {
                            areaPopUpText.text = target.price.ToString() + "€";
                            areaPopUp.SetActive(true);
                            areaPopUp.transform.position = new Vector3(mouse.point.x, 2f, mouse.point.z);
                        }
                    }
                }
                else if (mouse.collider.gameObject.layer == 9 && !buildScript.buildMode)
                {
                    areaPopUp.SetActive(false);
                    HouseScript selectedHouse = mouse.collider.GetComponent<HouseScript>();

                    if (selectedHouse.buildingLevel < StorageScript.Instance.currentSector)
                    {
                        if (selectedHouse.upgradeable)
                        {
                            upgradePopUp.transform.position = mouse.collider.transform.position + Vector3.up * 2f;
                            upgradePopUpButtonText.text = "Upgrade!";
                            upgradePopUpText.text = ("Cost: " + selectedHouse.upgradeWoodCost[selectedHouse.buildingLevel + 1] + "m³ " + woodNames[selectedHouse.buildingLevel + 1] + ", " + selectedHouse.upgradeMoneyCost + "€");
                            targetHouse = selectedHouse;
                            upgradePopUp.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                areaPopUp.SetActive(false);
                upgradePopUp.SetActive(false);
            }
        }
        
    }

    public void ButtonPushed()
    {
        bool bought = target.BuyArea();
        if (bought)
        {
            areaPopUp.SetActive(false);
            mapManager.sectorsOwned[StorageScript.Instance.currentSector]++;
        }
        else
        {
            areaPopUpText.text = "Not enough money!";
        }
    }

    public void UpgradeButton()
    {
        bool success = targetHouse.UpgradeHouse();
        if (success)
        {
            upgradePopUp.SetActive(false);
            targetHouse = null;
        }
        else
        {
            upgradePopUpButtonText.text = "Not enough money!";
        }
    }
}
