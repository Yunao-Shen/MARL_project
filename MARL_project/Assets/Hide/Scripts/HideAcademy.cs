using UnityEngine;
using MLAgents;

public class HideAcademy : Academy
{
    public int totalSteps; // max steps for each training episode
    public float chaserSpeed;
    public float escaperSpeed;
    public float angularSpeed;
    public float escaperFOV;
    public bool test = false; // 0: stop scoreboard after reach testRounds;  1: always keep scoreboard alive
    public int testRounds = 1000;
    public float highspeedrate = 2;

    [HideInInspector]
    public float envLenth;
    [HideInInspector]
    public float envWidth;
    [HideInInspector]
    public float wins;
    [HideInInspector]
    public float losses;
    [HideInInspector]
    public float winRate;

    public override void AcademyReset()
    {

    }

    public override void AcademyStep()
    {
        winRate = wins / (wins + losses);
    }
}


