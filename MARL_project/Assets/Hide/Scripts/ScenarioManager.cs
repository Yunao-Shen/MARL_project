using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public GameObject[] walls;
    public GameObject[] winZoneInitialPositions;
    public GameObject winZone;
    public float probability = 0.75f;

    public void GeneratePlayGround()
    {
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
            if (Random.Range(0f,1f) < probability)
            {
                wall.SetActive(true);
            }
        }
        winZone.transform.position = winZoneInitialPositions[Random.Range(0, winZoneInitialPositions.Length)].transform.position;
    }

}
