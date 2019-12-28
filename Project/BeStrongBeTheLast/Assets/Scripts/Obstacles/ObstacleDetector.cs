using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    internal List<GameObject> detectedObstacles;

    private void Start()
    {
        detectedObstacles = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacles") && !detectedObstacles.Contains(other.gameObject))
        {
            detectedObstacles.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Obstacles") && detectedObstacles.Contains(other.gameObject))
            detectedObstacles.Remove(other.gameObject);
    }
}
