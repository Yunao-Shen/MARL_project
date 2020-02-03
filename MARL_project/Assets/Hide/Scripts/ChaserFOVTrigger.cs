﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserFOVTrigger : MonoBehaviour
{
    public ChaserAgent hideChaser;
    public bool showDebug = false;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (showDebug)
                Debug.Log("Agent in FOV");
            //hideChaser.RaycastToAgent();
            hideChaser.agentInFOV = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //hideChaser.RaycastToAgent();
            hideChaser.agentInFOV = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //hideChaser.RaycastToAgent();
            //hideChaser.agentInLOS = false;
            hideChaser.agentInFOV = false;
        }
    }
}
