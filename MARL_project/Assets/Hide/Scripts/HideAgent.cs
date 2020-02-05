using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class HideAgent : Agent
{

    [Header("Specific to Hide/Escape")]
    public GameObject HideSpace;
    //public GameObject playGroundManager;
    public ScenarioManager scenarioManager;
    public GameObject chaser;
    public GameObject chaserIndicator;
    private ChaserAgent hideChaser;
    public GameObject[] spawnLocations;
    private Vector3 agentVelocity;
    
    public bool debug = false;
    public bool trainChaser = false;
    public int roundTime = 90;

    private Rigidbody rb;
    
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


    private float distanceFromChaser = 0;
    private float lastDistanceFromWinzone =45f;
    private float distanceFromWinzone = 0;
    private float reward = 0.0f;
    private int seconds;
    private bool isResetting = false;
    private float noObstoWinzone = 0f;
    private float angleToWinzone = 0f;
    private float angleToChaser = 0f;
    private float highspeed = 2;

    public HideAcademy hideAcademy;
    public GameObject winZone;

    [Header("GUI")]
    public Text stepsText;
    public Text scoreText;
    public Text winsText;
    public Text lossesText;
    public Text ClockCountDown;
    public Text StatusText;
    public Text DistanceText;
    public Text uparrowText;
    public Text downarrowText;
    public Text leftarrowText;
    public Text rightarrowText;
    private float totalReward;
    [HideInInspector]
    public bool chaserHitByRaycast = false;
    [HideInInspector]
    public bool escaperHitByRaycast = false;
    [HideInInspector]
    public int wins;
    [HideInInspector]
    public int losses;
    [HideInInspector]
    public Vector3 lastKnownChaserLocation;
    [HideInInspector]
    public Vector3 lastKnownChaserDirection;

    private void Start()
    {
        lastKnownChaserLocation = HideSpace.transform.position;
        hideChaser = chaser.GetComponent<ChaserAgent>();
        rb = this.GetComponent<Rigidbody>();
        totalReward = 0;
        seconds = roundTime;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        UpdateGUI();
    }

    void UpdateGUI()
    {
        stepsText.text = "Steps: " + GetStepCount().ToString();
        scoreText.text = "Score: " + totalReward.ToString("F3");
        if (trainChaser == true )
        {
            winsText.text = "Wins: " + losses.ToString();
            lossesText.text = "Losses: " + wins.ToString();
        }
        else
        {
            winsText.text = "Wins: " + wins.ToString();
            lossesText.text = "Losses: " + losses.ToString();
        }
        ClockCountDown.text = "Time: " + ((agentParameters.maxStep - GetStepCount()) / 100).ToString();
        StatusText.text = "Status: Stealth";
        StatusText.color = Color.blue;
        DistanceText.text = "";
    }

    private void OnTriggerEnter(Collider other)  // reach win zone
    {
        if (other.tag == "WinZone")
        {
            if (isResetting == false)
            {
                isResetting = true;
                AddReward(1f);
                hideChaser.Done();
                Done();
                hideAcademy.wins += 1;
                wins += 1;

            }
        }
    }

    private void OnCollisionEnter(Collision collision)  //  get caught
    {
        if (collision.gameObject == chaser.transform.gameObject)
        {
            if (isResetting == false)
            {
                isResetting = true;
                AddReward(-1f);
                hideChaser.Done();
                Done();
                hideAcademy.losses += 1;
                losses += 1;
                
            }
            
        }
    }

    IEnumerator TimeCountDown()
    {
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1f / Time.timeScale);
            seconds -= 1;
        }
        if (seconds == 0)
        {
            if (isResetting == false)
            {
                isResetting = true;
                AddReward(-1f);
                hideChaser.Done();
                Done();
                losses += 1;
                hideAcademy.losses += 1;

            }
        }

    }

    public bool IsInView()   //  check if in FOV
    {
        float threshold = Mathf.Cos((hideAcademy.escaperFOV * Mathf.PI) / 180);
        Vector3 direction = (chaser.transform.position - this.transform.position).normalized;
        if (Vector3.Dot(direction, this.transform.forward) > threshold )
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    void RaycastAtChaser()    // check if in LOS
    {
        RaycastHit raycastHit;
        Physics.Raycast(this.transform.position, chaser.transform.position - this.transform.position, out raycastHit);
        if (raycastHit.collider.name == "Chaser" )
        {
            escaperHitByRaycast = true;
            if (IsInView())
            {
                lastKnownChaserLocation = chaser.transform.position;
                lastKnownChaserDirection = chaser.transform.forward;
                chaserIndicator.transform.position = lastKnownChaserLocation;
                chaserHitByRaycast = true;
                if (debug) { Debug.DrawLine(raycastHit.point, this.transform.position, Color.blue); }
            }
        }
        else
        {
            escaperHitByRaycast = false;
            chaserHitByRaycast = false;
        }
        //Anlge towards chaser
        Vector3 direction = (lastKnownChaserLocation - this.transform.position).normalized;
        angleToChaser = Vector3.Dot(direction, this.transform.forward);
    }

    void RaycastInDirections()
    {
        RaycastHit raycastHit;

        
        //Raycast winzone
        Physics.Raycast(this.transform.position, winZone.transform.position - this.transform.position, out raycastHit);
        if (raycastHit.collider.name == "WinZone") { noObstoWinzone = 1f; }
        else { noObstoWinzone = 0f; }

        //Anlge towards winzone
        Vector3 direction = (winZone.transform.position - this.transform.position).normalized;
        angleToWinzone = Vector3.Dot(direction, this.transform.forward);

        
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
        Debug.DrawLine(raycastHit.point, this.transform.position, Color.blue);


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

    public override void CollectObservations()
    {
        // find chaser?
        if (chaserHitByRaycast)
            AddVectorObs(1);
        else
            AddVectorObs(0);
        // Where is the chaser
        AddVectorObs((HideSpace.transform.position.x - lastKnownChaserLocation.x) / hideAcademy.envWidth );
        AddVectorObs((HideSpace.transform.position.z - lastKnownChaserLocation.z) / hideAcademy.envLenth );
        // Is the agent being chasing
        if (hideChaser.chasingAgent)
            AddVectorObs(1);
        else
            AddVectorObs(0);
        // Chaser's forward
        AddVectorObs(lastKnownChaserDirection.x);
        AddVectorObs(lastKnownChaserDirection.z);
        // Agent's forward
        AddVectorObs(this.transform.forward.x);
        AddVectorObs(this.transform.forward.z);
        // Angle towards chaser
        AddVectorObs(angleToChaser);
        // Chaser's distance from agent
        AddVectorObs((lastKnownChaserLocation.x - this.transform.position.x) / (hideAcademy.envWidth * 2));
        AddVectorObs((lastKnownChaserLocation.z - this.transform.position.z) / (hideAcademy.envLenth * 2));
        // Where is the agent
        AddVectorObs((HideSpace.transform.position.x - this.transform.position.x) / hideAcademy.envWidth);
        AddVectorObs((HideSpace.transform.position.z - this.transform.position.z) / hideAcademy.envLenth);
        // Velocity of agent
        AddVectorObs((rb.velocity.x) / hideAcademy.escaperSpeed);
        AddVectorObs((rb.velocity.z) / hideAcademy.escaperSpeed);
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
        // Where is the win zone 
        AddVectorObs((HideSpace.transform.position.x - winZone.transform.position.x) / hideAcademy.envWidth);
        AddVectorObs((HideSpace.transform.position.z - winZone.transform.position.z) / hideAcademy.envLenth);
        // Distance from win zone
        AddVectorObs(Vector3.Distance(this.transform.position, winZone.transform.position));
        // Is winzone blocked 
        AddVectorObs(noObstoWinzone);
        // Angle towards win zone
        AddVectorObs(angleToWinzone);
    }

    public void MoveAgent(float[] act)
    {
        float directionX = 0;
        float directionZ = 0;
        uparrowText.color = Color.black;
        downarrowText.color = Color.black;
        leftarrowText.color = Color.black;
        rightarrowText.color = Color.black;
        float escapespeed = act[4] == 1? hideAcademy.escaperSpeed*highspeed:hideAcademy.escaperSpeed;

        if (act[0] == 1)
        {
            directionX = act[4] ==1? -1*highspeed:-1;
            uparrowText.color = Color.red;
            downarrowText.color = Color.black;
        }
        if (act[1] == 1)
        {
            if (directionX == 0)
            {
                directionX = act[4]== 1? 1*highspeed:1;
                uparrowText.color = Color.black;
                downarrowText.color = Color.red;
            }
            else
            {
                directionX = 0;
                uparrowText.color = Color.black;
                downarrowText.color = Color.black;
            }

        }
        if (act[2] == 1)
        {
            directionZ = -1;
            leftarrowText.color = Color.red;
            rightarrowText.color = Color.black;
        }
        if (act[3] == 1)
        {
            if (directionZ == 0)
            {
                directionZ = 1;
                leftarrowText.color = Color.black;
                rightarrowText.color = Color.red;
            }
            else
            {
                directionZ = 0;
                leftarrowText.color = Color.black;
                rightarrowText.color = Color.black;
            }
            
        }
        
        this.rb.AddForce(-this.transform.forward * directionX * 10f);
        this.transform.Rotate(0f, directionZ * hideAcademy.angularSpeed, 0f);
        
        if (rb.velocity.magnitude > escapespeed)    // limit to max speed
        {
            rb.velocity = rb.velocity.normalized * escapespeed;
        }
    }

    public override void AgentAction(float[] act, string TextAction)
    {
        reward = 0f;
        RaycastAtChaser();
        RaycastInDirections();
        if (escaperHitByRaycast) { hideChaser.InLOS();}
        if (GetStepCount() == (agentParameters.maxStep - 1))
        {
            AddReward(-1f);
        }
        MoveAgent(act);
        totalReward += reward;
        AddReward(reward);

    }

    public override void AgentReset()
    {
        scenarioManager.GeneratePlayGround();
        // reset agents
        gameObject.transform.position = spawnLocations[Random.Range(0, spawnLocations.Length)].transform.position;
        gameObject.transform.rotation = spawnLocations[Random.Range(0, spawnLocations.Length)].transform.rotation;
        reward = 0;
        totalReward = 0;
        seconds = roundTime;
        StopAllCoroutines();
        StartCoroutine(TimeCountDown());
        // reser UI
        StatusText.text = "Status: Stealth";
        StatusText.color = Color.blue;
        DistanceText.text = "";
        lastKnownChaserLocation = HideSpace.transform.position;
        lastKnownChaserDirection = new Vector3(0f, 0f, -1f);
        chaserIndicator.transform.position = HideSpace.transform.position;
        isResetting = false;
    }
}


