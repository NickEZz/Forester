using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDespawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}