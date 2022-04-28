using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour
{
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] vehiclePrefabs;

    [SerializeField] float percentChance;

    [SerializeField] float spawnTimer;
    [SerializeField] float timer;

    private void Awake()
    {
        timer = spawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = spawnTimer;
            if (Random.Range(0, 101) <= percentChance)
            {
                GameObject chosenVehicle = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
                GameObject chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                Instantiate(chosenVehicle, chosenSpawnPoint.transform.position, chosenSpawnPoint.transform.rotation);
            }
        }
    }
}
