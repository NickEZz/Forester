using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] float sensitivity;
    [SerializeField] float zoomSpeed;
    
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

        float scroll = Input.mouseScrollDelta.y * zoomSpeed;
        targetPos = Mathf.Clamp(targetPos, 1, 14) - scroll;

        float dir = targetPos - transform.position.y;

        transform.position += new Vector3(0, dir * 5 * Time.deltaTime, 0);
    }
}
