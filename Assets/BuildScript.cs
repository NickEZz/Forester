using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildScript : MonoBehaviour
{
    [SerializeField] bool buildMode;
    [SerializeField] int selectedBuilding;

    [SerializeField] GameObject[] buildings;

    [SerializeField] GameObject grid;
    Mesh gridMesh;

    [SerializeField] GameObject buildingArea;

    [SerializeField] GameObject previewObject;

    [SerializeField] LayerMask groundMask;
    [SerializeField] Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        gridMesh = grid.GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
        }

        if (buildMode)
        {
            if (buildingArea == null)
            {
                grid.SetActive(true);
                previewObject.SetActive(true);
            }

            RaycastHit areaCheck;
            if (Physics.Raycast(cam.gameObject.transform.position, cam.gameObject.transform.forward, out areaCheck, 30f, groundMask))
            {
                buildingArea = areaCheck.collider.gameObject;
                grid.transform.position = buildingArea.transform.position + new Vector3(0f, 0.15f, 0f);
            }

            Vector2 input = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(input);

            RaycastHit mouse;
            Physics.Raycast(ray, out mouse, 20f, groundMask);


            previewObject.transform.position = mouse.point; // Jotenki sn‰pp‰‰ previewobjekti gridiin

            if (Input.GetMouseButtonDown(0))
            {
                // Spawnaa rakennus
            }
        }
        else
        {
            grid.SetActive(false);
            buildingArea = null;
            previewObject.SetActive(false);
        }
    }
}


public class Building
{
    
   
}
