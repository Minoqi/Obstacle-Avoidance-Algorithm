using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Variables
    public GameObject agent;
    public bool targetSafeZone;
    public Transform boundsTop, boundsBottom, boundsRight, boundsLeft;

    public void GenerateNewLocation()
    {
        transform.position = new Vector3(Random.Range(boundsTop.position.x, boundsBottom.position.x), 0, Random.Range(boundsLeft.position.z, boundsRight.position.z));

        if (!targetSafeZone)
        {
            transform.position = new Vector3(Random.Range(boundsTop.position.x, boundsBottom.position.x), 0, Random.Range(boundsLeft.position.z, boundsRight.position.z));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            targetSafeZone = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            targetSafeZone = true;
        }
    }
}
