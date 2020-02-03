using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreBoard : MonoBehaviour
{

    public bool chaserData;  // 1: chasers' scoreboard;  0: escapees' scoreboard
    public Text winsText;
    public Text lossesText;
    public Text winRateText;
    public HideAcademy hideAcademy;


    void Start()
    {
        
    }

    void Update()
    {
        
        if (hideAcademy.losses + hideAcademy.wins <= hideAcademy.testRounds || hideAcademy.test)
        {
            if(chaserData==false) //escapees' scoreboard
            {
                winsText.text = "Wins: " + hideAcademy.wins.ToString();
                lossesText.text = "Rounds: " + (hideAcademy.losses + hideAcademy.wins).ToString();
                winRateText.text = "Win Rate: " + (hideAcademy.wins / (hideAcademy.losses + hideAcademy.wins)).ToString("F3");
            }
            else                  //chasers' scoreboard
            {
                winsText.text = "Wins: " + hideAcademy.losses.ToString();
                lossesText.text = "Rounds: " + (hideAcademy.losses + hideAcademy.wins).ToString();
                winRateText.text = "Win Rate: " + (hideAcademy.losses / (hideAcademy.losses + hideAcademy.wins)).ToString("F3");
            }

        }
        
    }
}
