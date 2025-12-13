using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Transform objectHolder;
    public float repeatRate = 0.1f;

    float time = 0f;

    public bool isPlaying = false;

    void Update()
    {
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z));

        time += Time.deltaTime;
        while (time >= repeatRate)
        {
            time -= repeatRate;
            Spawn();
        }
    }

    void Spawn()
    {
        if (isPlaying && CanSpawn())
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity, objectHolder);
        }
    }

    private bool CanSpawn()
    {
        float rayLength = 15f;

        bool canSpawn = Physics.Raycast(transform.position, -transform.up, rayLength);

        return canSpawn;
    }
}