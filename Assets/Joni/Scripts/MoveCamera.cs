using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float sensitivity;
    [SerializeField] float zoomSpeed;
    [SerializeField] float minCameraHeight;
    [SerializeField] float maxCameraHeight;

    [SerializeField] float[] zoomedInBorders;
    
    float tempSpeed;

    float targetPos;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float keySpeed = transform.position.y / 100;

        float keyX = Input.GetAxis("Horizontal") * keySpeed * sensitivity;
        float keyY = Input.GetAxis("Vertical") * keySpeed * sensitivity;

        transform.position += new Vector3(keyX, 0, keyY);

        if (Input.GetMouseButton(1))
        {
            float mouseSpeed = transform.position.y / 20;

            float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * sensitivity;

            transform.position -= new Vector3(mouseX, 0, mouseY);
        }

        var pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, zoomedInBorders[0] + transform.position.y, zoomedInBorders[1] - transform.position.y);
        pos.z = Mathf.Clamp(transform.position.z, zoomedInBorders[2], zoomedInBorders[3] - 2 * transform.position.y);
        transform.position = pos;

        float scroll = Input.mouseScrollDelta.y * zoomSpeed;
        targetPos = Mathf.Clamp(targetPos, minCameraHeight, maxCameraHeight) - scroll;

        float dir = targetPos - transform.position.y;

        transform.position += new Vector3(0, dir * 5 * Time.deltaTime, 0);
    }
}
