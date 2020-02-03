using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAgents;

public class ChaserAgentAI : Agent
{
    [Header("Specific to Hide/Escape")]
    public GameObject HideSpace;
    public GameObject[] chaserInitialPositions;
    private Vector3 agentVelocity;
    public GameObject myAgent;
    public GameObject chaserHead;
    public bool debug = false;

    private float distanceXNeg = 0f;
    private float distanceXPos = 0f;
    private float distanceZNeg = 0f;
    private float distanceZPos = 0f;
    private float distanceUL = 0f;
    private float distanceUR = 0f;
    private float distanceDL = 0f;
    private float distanceDR = 0f;
    private float distanceUR2 = 0f;
    private float distanceDR2 = 0f;
    private float distanceUR3 = 0f;
    private float distanceDR3 = 0f;
    private float distanceUR4 = 0f;
    private float distanceDR4 = 0f;

    [HideInInspector]
    public HideAgentwAIChaser hideAgent;
    private Rigidbody rb;
    [HideInInspector]
    public NavMeshAgent navAgent;
    [HideInInspector]
    private Rigidbody chaserRigidbody;
    [HideInInspector]
    public bool agentInLOS = false;
    [HideInInspector]
    public bool agentInFOV = false;
    [HideInInspector]
    public Vector3 lastKnownAgentLocation;
    [HideInInspector]
    public Vector3 lastKnownAgentDirection;
    [HideInInspector]
    public bool chasingAgent = false;
    [HideInInspector]
    public float totalReward;

    public GameObject targetLocationIndicator;
    private float positionDoneThreshold = 0.15f;
    private bool chaserHitByRaycast = false;
    private float lastDistanceBetweenChaser = 0;
    private float distanceBetweenChaser = 0;
    private float angleToEscaper;

    private float reward = 0.0f;
    

    public HideAcademy hideAcademy;
    public GameObject winZone;

    private void Awake()
    {
        agentParameters.maxStep = hideAcademy.totalSteps;
    }

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        this.chaserRigidbody = this.GetComponent<Rigidbody>();
        this.hideAgent = myAgent.GetComponent<HideAgentwAIChaser>();
        lastKnownAgentLocation = new Vector3(0f, 2.5f, -18f);
        lastKnownAgentDirection = new Vector3(0f,0f,1f);
        totalReward = 0;
    }

    void RaycastInDirections()
    {

        //Anlge towards chaser
        Vector3 direction = (lastKnownAgentLocation - this.transform.position).normalized;
        angleToEscaper = Vector3.Dot(direction, this.transform.forward);

        RaycastHit raycastHit;

        // Raycast down
        Physics.Raycast(this.transform.position, this.transform.right, out raycastHit);
        distanceXPos = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast up
        Physics.Raycast(this.transform.position, -this.transform.right, out raycastHit);
        distanceXNeg = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast right
        Physics.Raycast(this.transform.position, this.transform.forward, out raycastHit);
        distanceZPos = (raycastHit.point - this.transform.position).magnitude;
        Debug.DrawLine(raycastHit.point, this.transform.position, Color.red);


        // Raycast left
        Physics.Raycast(this.transform.position, -this.transform.forward, out raycastHit);
        distanceZNeg = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast upleft
        Physics.Raycast(this.transform.position, (-this.transform.right - this.transform.forward).normalized, out raycastHit);
        distanceUL = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast upright
        Physics.Raycast(this.transform.position, (-this.transform.right + this.transform.forward).normalized, out raycastHit);
        distanceUR = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast downleft
        Physics.Raycast(this.transform.position, (this.transform.right - this.transform.forward).normalized, out raycastHit);
        distanceDL = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast downright
        Physics.Raycast(this.transform.position, (this.transform.right + this.transform.forward).normalized, out raycastHit);
        distanceDR = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast upright2
        Physics.Raycast(this.transform.position, (-this.transform.right + this.transform.forward * 2f).normalized, out raycastHit);
        distanceUR2 = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }


        // Raycast downright2
        Physics.Raycast(this.transform.position, (this.transform.right + this.transform.forward * 2f).normalized, out raycastHit);
        distanceDR2 = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast upright3
        Physics.Raycast(this.transform.position, ((-this.transform.right + this.transform.forward * 2f).normalized + (-this.transform.right + this.transform.forward).normalized).normalized, out raycastHit);
        distanceUR3 = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast downright3
        Physics.Raycast(this.transform.position, ((this.transform.right + this.transform.forward * 2f).normalized + (this.transform.right + this.transform.forward).normalized).normalized, out raycastHit);
        distanceDR3 = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast upright4
        Physics.Raycast(this.transform.position, ((-this.transform.right + this.transform.forward * 2f).normalized + this.transform.forward).normalized, out raycastHit);
        distanceUR4 = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }

        // Raycast downright4
        Physics.Raycast(this.transform.position, ((this.transform.right + this.transform.forward * 2f).normalized + this.transform.forward).normalized, out raycastHit);
        distanceDR4 = (raycastHit.point - this.transform.position).magnitude;

        if (debug)
        {
            Debug.DrawLine(raycastHit.point, this.transform.position, Color.yellow);
        }
    }

    public void InLOS()    // if escapee in line of sight, check FOV and update UI
    {
        if (agentInFOV)
        {
            if (debug) { Debug.DrawLine(this.transform.position, myAgent.transform.position, Color.red); }
            agentInLOS = true;
            chasingAgent = true;
            lastKnownAgentLocation = myAgent.transform.position;
            lastKnownAgentDirection = myAgent.transform.forward;
            hideAgent.StatusText.text = "Status: Detected!";
            hideAgent.StatusText.color = Color.red;
            hideAgent.DistanceText.text = (Vector3.Distance(myAgent.transform.position, this.transform.position) - 1).ToString("F1");
        }
        else
        {
            chasingAgent = false;
            hideAgent.StatusText.text = "Status: Stealth";
            hideAgent.StatusText.color = Color.blue;
            hideAgent.DistanceText.text = "";
        }
    }

    public override void CollectObservations()
    {
        // Where is the escaper? is escaper detected? the direction of escaper?
        if (agentInFOV && hideAgent.escaperHitByRaycast)
        {
            AddVectorObs((HideSpace.transform.position.x - myAgent.transform.position.x) / hideAcademy.envWidth);
            AddVectorObs((HideSpace.transform.position.z - myAgent.transform.position.z) / hideAcademy.envLenth);
            AddVectorObs(1);
            AddVectorObs(myAgent.transform.forward.x);
            AddVectorObs(myAgent.transform.forward.z);
        }
        else
        {
            AddVectorObs((HideSpace.transform.position.x - lastKnownAgentLocation.x) / hideAcademy.envWidth);
            AddVectorObs((HideSpace.transform.position.z - lastKnownAgentLocation.z) / hideAcademy.envLenth);
            AddVectorObs(0);
            AddVectorObs(lastKnownAgentDirection.x);
            AddVectorObs(lastKnownAgentDirection.z);
        }
        // Escaper's distance from agent
        if (agentInFOV)
        {
            AddVectorObs((myAgent.transform.position.x - this.transform.position.x) / (hideAcademy.envWidth * 2));
            AddVectorObs((myAgent.transform.position.z - this.transform.position.z) / (hideAcademy.envLenth * 2));
        }
        else
        {
            AddVectorObs((lastKnownAgentLocation.x - this.transform.position.x) / (hideAcademy.envWidth * 2));
            AddVectorObs((lastKnownAgentLocation.z - this.transform.position.z) / (hideAcademy.envLenth * 2));
        }

        // Where is the agent
        AddVectorObs((HideSpace.transform.position.x - this.transform.position.x) / hideAcademy.envWidth);
        AddVectorObs((HideSpace.transform.position.z - this.transform.position.z) / hideAcademy.envLenth);
        // Velocity of agent
        AddVectorObs(chaserRigidbody.velocity.x / hideAcademy.chaserSpeed);
        AddVectorObs(chaserRigidbody.velocity.z / hideAcademy.chaserSpeed);
        // Agent's forward
        AddVectorObs(this.transform.forward.x);
        AddVectorObs(this.transform.forward.z);
        // Angle towards 
        AddVectorObs(angleToEscaper);
        // Distance in each direction from agent
        AddVectorObs(distanceXPos / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceXNeg / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceZPos / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceZNeg / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceUL / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceUR / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceDL / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceDR / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceUR2 / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceDR2 / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceUR3 / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceDR3 / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceUR4 / (hideAcademy.envLenth * 2));
        AddVectorObs(distanceDR4 / (hideAcademy.envLenth * 2));
        

    }

    public void MoveAgent(float[] act)
    {
        float directionX = 0;
        float directionZ = 0;

        if (act[0] == 1)
        {
            directionX = -1;
        }
        if (act[1] == 1)
        {
            if (directionX == 0)
            {
                directionX = 1;
            }
            else
            {
                directionX = 0;
            }

        }
        if (act[2] == 1)
        {
            directionZ = -1;
        }
        if (act[3] == 1)
        {
            if (directionZ == 0)
            {
                directionZ = 1;
            }
            else
            {
                directionZ = 0;
            }

        }

        this.rb.AddForce(-this.transform.forward * directionX * 10f);
        this.transform.Rotate(0f, directionZ * hideAcademy.angularSpeed, 0f);
        if (rb.velocity.magnitude > hideAcademy.chaserSpeed)    // limit to max speed
        {
            rb.velocity = rb.velocity.normalized * hideAcademy.chaserSpeed;
        }
    }

    public override void AgentAction(float[] act, string TextAction)
    {
        reward = 0f;
        RaycastInDirections();
        if (hideAgent.escaperHitByRaycast) { InLOS(); }
        else
        {
            chasingAgent = false;
            hideAgent.StatusText.text = "Status: Stealth";
            hideAgent.StatusText.color = Color.blue;
            hideAgent.DistanceText.text = "";
        }
        MoveAgent(act);
        totalReward += reward;
        AddReward(reward);

    }
    public override void AgentReset()
    {
        // reset transform
        gameObject.transform.position = chaserInitialPositions[Random.Range(0, chaserInitialPositions.Length)].transform.position;
        gameObject.transform.rotation = chaserInitialPositions[Random.Range(0, chaserInitialPositions.Length)].transform.rotation;
        agentInLOS = false;
        chasingAgent = false;
        reward = 0;
        totalReward = 0;
        lastKnownAgentLocation = new Vector3(0f, 2.5f, -18f);
        lastKnownAgentDirection = new Vector3(0f, 0f, 1f);
    }
}
