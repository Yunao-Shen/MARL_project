using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAgents;

public class ChaserAgent : Agent
{

    public GameObject myAgent;
    public GameObject chaserHead;
    [HideInInspector]
    public HideAgent hideAgent;

    public GameObject[] seekLocations;
    private float positionDoneThreshold = 0.2f;

    public GameObject[] chaserInitialPositions;

    [HideInInspector]
    public NavMeshAgent navAgent;

    [HideInInspector]
    public GameObject currentTarget;
    public GameObject targetLocationIndicator;

    [HideInInspector]
    public bool agentInLOS = false;
    [HideInInspector]
    public bool agentInFOV = false;
    [HideInInspector]
    public bool chasingAgent = false;
    public bool showDebug = false;

    [HideInInspector]
    public Vector3 lastKnownAgentLocation;

    private Rigidbody chaserRigidbody;

    public HideAcademy hideAcademy;

    public GameObject winZone;

    void Start()
    {
        this.navAgent = this.GetComponent<NavMeshAgent>();
        this.chaserRigidbody = this.GetComponent<Rigidbody>();

        this.hideAgent = myAgent.GetComponent<HideAgent>();

        currentTarget = seekLocations[Random.Range(0, seekLocations.Length)];
        navAgent.speed = hideAcademy.chaserSpeed;
        navAgent.SetDestination(currentTarget.transform.position);

        this.targetLocationIndicator.transform.parent = null;
        lastKnownAgentLocation = Vector3.zero;

    }

    void MoveChaser()
    {
        // If chaser didnt find escapee, pick next patrol location
        if (Mathf.Abs((currentTarget.transform.position - this.transform.position).magnitude) < positionDoneThreshold && (chasingAgent == false))
        {
            PickNextLocation();
        }

        // If chaser find escapee, pick next patrol location after reaching last known escapee location
        if ((Mathf.Abs((lastKnownAgentLocation - this.transform.position).magnitude) < positionDoneThreshold) && (chasingAgent == true) && (agentInFOV == false))
        {
            chasingAgent = false;
            PickNextLocation();
        }

        // chasing escapee
        if (chasingAgent == true)
        {
            ChaseAgent();
        }
    }

    public void InLOS()  // if escapee in line of sight, check FOV and update UI
    {
        if (agentInFOV)
        {
            if (showDebug) { Debug.DrawLine(this.transform.position, myAgent.transform.position, Color.red); }
            agentInLOS = true;
            chasingAgent = true;
            lastKnownAgentLocation = myAgent.transform.position;
            hideAgent.StatusText.text = "Status: Detected!";
            hideAgent.StatusText.color = Color.red;
            hideAgent.DistanceText.text = Vector3.Distance(myAgent.transform.position, this.transform.position).ToString("F3");
        }
        else
        {
            hideAgent.StatusText.text = "Status: Stealth";
            hideAgent.StatusText.color = Color.blue;
            hideAgent.DistanceText.text = "";
        }
    }

    void PlaceTargetIndicator()
    {
        if (chasingAgent == false)
        {
            targetLocationIndicator.transform.position = currentTarget.transform.position;
        }
        else
        {
            targetLocationIndicator.transform.position = lastKnownAgentLocation;
        }
    }

    public void PickNextLocation()
    {
        GameObject tempCurrentTarget = seekLocations[Random.Range(0, seekLocations.Length)];
        while (tempCurrentTarget == currentTarget)    // avoid picking current location
        {
            tempCurrentTarget = seekLocations[Random.Range(0, seekLocations.Length)];
        }
        if (tempCurrentTarget != currentTarget)
        {
            currentTarget = tempCurrentTarget;
            navAgent.SetDestination(currentTarget.transform.position);
            targetLocationIndicator.transform.position = currentTarget.transform.position;
        }
    }

    public void ChaseAgent()
    {
        currentTarget = myAgent;
        navAgent.SetDestination(lastKnownAgentLocation);
    }

    public override void CollectObservations()
    {
        AddVectorObs(1);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        PlaceTargetIndicator();
        MoveChaser();
    }

    public override void AgentReset()
    {
        // Get curriculum variables from academy
        float chaserSpeed = hideAcademy.GetComponent<HideAcademy>().chaserSpeed;
        navAgent.speed = chaserSpeed;

        this.chaserRigidbody.isKinematic = true;
        // reset transform
        GameObject chaserInitialPosition = chaserInitialPositions[Random.Range(0, chaserInitialPositions.Length)];
        this.transform.position = new Vector3(chaserInitialPosition.transform.position.x, this.transform.position.y, chaserInitialPosition.transform.position.z);
        this.transform.rotation = chaserInitialPosition.transform.rotation;
        // reset varaiales
        agentInLOS = false;
        chasingAgent = false;
        currentTarget = null;
        lastKnownAgentLocation = Vector3.zero;
        this.chaserRigidbody.isKinematic = false;
        // random start
        PickNextLocation();
    }

}
