using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kinect = Windows.Kinect;


/*                                                                THIS SCRIPT IS NOT FINISHED!!!!!!!
 
IT IS THE CREATION OF A STATE MACHINE FOR A STRUCTURED TUTORIAL. BUT WE SKIPPED BECAUSE WE DECIDED THAT WE ARE NOT GOING TO HAVE A STRUCTURED TUTORIAL BUT JUST A VIDEO TUTORIAL*/

public class TutorialManager : MonoBehaviour {

    public static TutorialManager instance;

    public enum tutorialStages 
    {
        bothRaiseHands,
        reachZones,
        movementZones,
        highFive,
        gameplay
    }

    public GameObject tutorialCanvas;
    Text text;

    public List<string> tutorialTexts = new List<string>();
    public List<AudioClip> voiceOvers = new List<AudioClip>();

    public List<bool> raisedHands = new List<bool>(); // debug purposes. Remove public after. Try making dynamic: Fixed for 4 joints for 2 players. Order of joint = p1 left, p1 right, p2 left, p2 right.


    public tutorialStages currentStage; // debug purposes. Remove public after

    float timer;
    float raiseHandTimer;

    public float standardDisplayTime;
    public float maxRaiseHandTime;

    public int setMaxPlayers;

    [HideInInspector]
    public int textCounter;
    int playerCounter;

    bool startTime, handsRaised, finished;


	// Use this for initialization
	void Start () {

        if (instance == null)
        {
            instance = this;
        }

        currentStage = tutorialStages.bothRaiseHands;

        text = tutorialCanvas.GetComponentInChildren<Text>();

        textCounter = 0;
        playerCounter = 0;

        timer = 0;
        raiseHandTimer = 0;

        startTime = false;
        handsRaised = false;
        finished = false;

        for(int hand = 0; hand < raisedHands.Count; hand++)
        {
            raisedHands[hand] = false;
        }
	}

    void Update()
    {
        if (startTime)
        {
            timer = timer + Time.deltaTime;

        }
        if(timer >= standardDisplayTime)
        {
            textCounter++;
            timer = 0;
            startTime = false;
        }
        if (handsRaised)
        {
            raiseHandTimer = raiseHandTimer + Time.deltaTime;
        }
        if(raiseHandTimer >= maxRaiseHandTime && !finished)
        {

            finished = true;
            handsRaised = false;
        }


    }
	
    void OnGUI()
    {

        if(Manager.instance.playersId.Count > 1 && Manager.instance.playersId.Count <= setMaxPlayers) // Fixed to be at least more than one player. 
        {
            
            playerCounter = Manager.instance.playersId.Count;
            Debug.Log("playercounter = " + playerCounter);
            switch (currentStage)
            {
                case tutorialStages.bothRaiseHands:
                    //call funtion for voiceover
                    text.text = tutorialTexts[textCounter];
                    if (textCounter < 3)
                    {
                        startTime = true;
                    }
                    CheckRaisedHands();
                    if (Manager.instance.tutorialRaisedHandsDown && finished)
                    {
                        currentStage = tutorialStages.reachZones;
                    }
                    break;

                case tutorialStages.reachZones:
                    print("reachZones");
                    break;

                case tutorialStages.movementZones:
                    break;

                case tutorialStages.highFive:
                    break;

                case tutorialStages.gameplay:
                    break;


            }
        }
        

    }

    void CheckRaisedHands()
    {
        //for(int i = 0; i < playerCounter; i++) // i runs through the active players
        //{
        //    var jpi = 0; //This counts the joints inside preferred Joints (prefJoints)

        //    foreach (Kinect.JointType joint in Manager.instance.prefJoints)
        //    {
        //        if(jpi != 0)
        //        {
        //            if (Manager.instance.playersJointsHeight[i][jpi] != 0)
        //            {
        //                if(i == 0)
        //                {
        //                    if(jpi == 1)
        //                    {
        //                        raisedHands[0] = true;
        //                    }
        //                    else
        //                    {
        //                        raisedHands[1] = true;
        //                    }
        //                }
        //                else
        //                {
        //                    if (jpi == 1)
        //                    {
        //                        raisedHands[2] = true;
        //                    }
        //                    else
        //                    {
        //                        raisedHands[3] = true;
        //                    }
        //                }
                        
        //            }
        //        }            
        //        jpi++;
        //    }
        //}

        var handCounter = 0;

        foreach (bool hand in raisedHands)
        {
            if(hand == true)
            {
                handCounter++;
            }
            
        }
        Debug.Log("raisedHands.Count = " + raisedHands.Count);
        Debug.Log("handCounter = " + handCounter);

        if (handCounter == raisedHands.Count)
        {
            handsRaised = true;
        }
        
    }

}
