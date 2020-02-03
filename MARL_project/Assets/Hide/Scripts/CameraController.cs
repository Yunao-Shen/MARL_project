using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Agent;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - Agent.transform.position;
    }

    void Update()
    {
        transform.position = Agent.transform.position + offset;
        transform.rotation = Agent.transform.rotation;
    }
}
