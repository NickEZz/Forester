using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    [SerializeField] float carSpeed;

    private void Update()
    {
        transform.position += -transform.up * carSpeed * Time.deltaTime;
    }
}