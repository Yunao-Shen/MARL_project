using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChaserFOVTrigger : MonoBehaviour
{
    public ChaserAgentAI hideChaser;
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
            hideChaser.agentInFOV = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hideChaser.agentInFOV = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hideChaser.agentInFOV = false;
        }
    }
}
