using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;
using UnityEngine.SceneManagement;

public class SupportFunctions : MonoBehaviour {

    public static SupportFunctions instance;

	void Start () {
		if(instance == null)
        {
            instance = this;
        }
	}


    public void CheckHighFive()
    {
        if (Manager.instance.playersId.Count > 1)
        {
            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                var j = 0;  //j counts the joints in the prefJoints
                foreach (Kinect.JointType joint in Manager.instance.prefJoints)
                {
                    if (jt == joint)
                    {
                        if (j != 0)
                        {
                            if (j == 1) //means handLeft
                            {
                                if (Mathf.Floor(Manager.instance.bodyJointsWorld[0][j, 2]) == Mathf.Floor(Manager.instance.bodyJointsWorld[1][j + 1, 2]) || Mathf.Floor(Manager.instance.bodyJointsWorld[0][j, 2]) == Mathf.Floor(Manager.instance.bodyJointsWorld[1][j, 2])) //Restriction only for two players
                                {
                                    var differenceX_LR = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 0] - Manager.instance.bodyJointsWorld[1][j + 1, 0]);
                                    var differenceY_LR = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 1] - Manager.instance.bodyJointsWorld[1][j + 1, 1]);
                                    var differenceX_LL = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 0] - Manager.instance.bodyJointsWorld[1][j, 0]);
                                    var differenceY_LL = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 1] - Manager.instance.bodyJointsWorld[1][j, 1]);
                                    if ((Mathf.Round(differenceX_LR * Manager.instance.high5Distance) == 0 && Mathf.Round(differenceY_LR * Manager.instance.high5Distance) == 0) || (Mathf.Round(differenceX_LL * Manager.instance.high5Distance) == 0 && Mathf.Round(differenceY_LL * Manager.instance.high5Distance) == 0)) //Seems that the hands need to be very close. Change 5 to 7.5 like (10 seems so far away)
                                    {
                                        if (Manager.instance.ableToHigh5Left == true)
                                        {
                                            for (int i = 0; i < LogData.instance.pID.Count; i++)
                                            {
                                                if (LogData.instance.active[i] == true)
                                                {
                                                    LogData.instance.didH5[i]++;
                                                }
                                            }
                                            Manager.instance.didHigh5 = true;    //Needs to be set on false when the fireworks were played;
                                            Manager.instance.ableToHigh5Left = false;
                                        }
                                    }
                                    else if ((Mathf.Round(differenceX_LR * Manager.instance.high5Distance) != 0 && Mathf.Round(differenceY_LR * Manager.instance.high5Distance) != 0) && (Mathf.Round(differenceX_LL * Manager.instance.high5Distance) != 0 && Mathf.Round(differenceY_LL * Manager.instance.high5Distance) != 0))
                                    {
                                        Manager.instance.ableToHigh5Left = true;
                                    }
                                }
                            }
                            else if (j == 2) //means handRight
                            {
                                if (Mathf.Floor(Manager.instance.bodyJointsWorld[0][j, 2]) == Mathf.Floor(Manager.instance.bodyJointsWorld[1][j - 1, 2]) || Mathf.Floor(Manager.instance.bodyJointsWorld[0][j, 2]) == Mathf.Floor(Manager.instance.bodyJointsWorld[1][j, 2])) //Restriction only for two players
                                {
                                    var differenceX_RL = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 0] - Manager.instance.bodyJointsWorld[1][j - 1, 0]);
                                    var differenceY_RL = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 1] - Manager.instance.bodyJointsWorld[1][j - 1, 1]);
                                    var differenceX_RR = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 0] - Manager.instance.bodyJointsWorld[1][j, 0]);
                                    var differenceY_RR = Mathf.Abs(Manager.instance.bodyJointsWorld[0][j, 1] - Manager.instance.bodyJointsWorld[1][j, 1]);
                                    if ((Mathf.Round(differenceX_RL * Manager.instance.high5Distance) == 0 && Mathf.Round(differenceY_RL * Manager.instance.high5Distance) == 0) || (Mathf.Round(differenceX_RR * Manager.instance.high5Distance) == 0 && Mathf.Round(differenceY_RR * Manager.instance.high5Distance) == 0))
                                    {
                                        if (Manager.instance.ableToHigh5Right == true)
                                        {
                                            for (int i = 0; i < LogData.instance.pID.Count; i++)
                                            {
                                                if (LogData.instance.active[i] == true)
                                                {
                                                    LogData.instance.didH5[i]++;
                                                }
                                            }
                                            Manager.instance.didHigh5 = true;    //Needs to be set on false when the fireworks were played;
                                            Manager.instance.ableToHigh5Right = false;
                                        }
                                    }
                                    else if ((Mathf.Round(differenceX_RL * Manager.instance.high5Distance) != 0 && Mathf.Round(differenceY_RL * Manager.instance.high5Distance) != 0) && (Mathf.Round(differenceX_RR * Manager.instance.high5Distance) != 0 && Mathf.Round(differenceY_RR * Manager.instance.high5Distance) != 0))
                                    {
                                        Manager.instance.ableToHigh5Right = true;
                                    }
                                }
                            }

                        }
                    }
                    j++;
                }
            }
        }
    }

    public void SortMinMax(float height, int player, int joint)
    {
        Manager.instance.playersMinMaxHeight[player][joint, 0] = Manager.instance.jointsHeight[0];

        if (height > Manager.instance.playersMinMaxHeight[player][joint, 1])
        {
            Manager.instance.playersMinMaxHeight[player][joint, 1] = height;
        }
    }

    public void SortLine(float height, int player, int joint)
    {
        Manager.instance.playersMinMaxHeightInPixels[player][joint, 0] = Manager.instance.spineMidHeightInPixels[player];

        if (height < Manager.instance.playersMinMaxHeightInPixels[player][joint, 1])
        {
            Manager.instance.playersMinMaxHeightInPixels[player][joint, 1] = height;
        }
    }

    public void CheckHandnBodyForNotes()
    {
        /////////////////////FIXED CASE ONLY FOR TWO PLAYERS AND ONLY FOR TWO HANDS
        if (Manager.instance.playersId.Count > 1)
        {
            try
            {
                if (Manager.instance.playersJointsHeight[0][1] > Manager.instance.playersJointsHeight[0][0] && (Manager.instance.playersJointsHeight[1][1] > Manager.instance.playersJointsHeight[1][0] || Manager.instance.playersJointsHeight[1][2] > Manager.instance.playersJointsHeight[1][0])
                    || Manager.instance.playersJointsHeight[0][2] > Manager.instance.playersJointsHeight[0][0] && (Manager.instance.playersJointsHeight[1][1] > Manager.instance.playersJointsHeight[1][0] || Manager.instance.playersJointsHeight[1][2] > Manager.instance.playersJointsHeight[1][0]))
                {
                    Manager.instance.playTones = true;
                }
                else
                {
                    Manager.instance.playTones = false;
                }

                if (((Manager.instance.playersJointsHeight[0][1] > Manager.instance.playersJointsHeight[0][0] || Manager.instance.playersJointsHeight[0][2] > Manager.instance.playersJointsHeight[0][0]) && Manager.instance.clipEndedP2 == false) || ((Manager.instance.playersJointsHeight[1][1] > Manager.instance.playersJointsHeight[1][0] || Manager.instance.playersJointsHeight[1][2] > Manager.instance.playersJointsHeight[1][0]) && Manager.instance.clipEndedP1 == false))

                {
                    Manager.instance.playTonesBodyHands = true;
                }
                else
                {
                    Manager.instance.playTonesBodyHands = false;
                }


            }
            catch
            {
                //print("Something went wrong with detecting the position of either the left hand or right hand or both from the player1 or player2");
            }
        }
        //////////////////////////////////////////////////////////////////////////
    }

    public void CheckHowPlayMusic() //This function collects data for how much time they spent on playing high/low/back/forward
    {
        //Checks for player1
        if (Manager.instance.p1UInput.userInput == "low")
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Blue")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeLowHand[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p1UInput.userInput == "high")
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Blue")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeHighHand[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p1MovedBack == true)
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Blue")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeBackBody[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p1MovedFrw == true)
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Blue")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeForwBody[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p1UInput.userInput == null)
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Blue")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeNullInput[i] += Time.deltaTime;
                    }
                }
            }
        }

        //Checks for player2
        if (Manager.instance.p2UInput.userInput == "low")
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Red")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeLowHand[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p2UInput.userInput == "high")
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Red")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeHighHand[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p2MovedBack == true)
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Red")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeBackBody[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p2MovedFrw == true)
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Red")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeForwBody[i] += Time.deltaTime;
                    }
                }
            }
        }
        if (Manager.instance.p2UInput.userInput == null)
        {
            for (int i = 0; i < LogData.instance.color.Count; i++)
            {
                if (LogData.instance.color[i] == "Red")
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeNullInput[i] += Time.deltaTime;
                    }
                }
            }
        }

    }

    public void HowPlayTogether()
    {
        if (Manager.instance.playersId.Count > 1)
        {
            if (Manager.instance.playTones == true)
            {
                for (int i = 0; i < LogData.instance.pID.Count; i++)
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timePlayingTogetherHands[i] += Time.deltaTime;
                    }
                }
            }

            if (Manager.instance.playTonesBody == true)
            {
                for (int i = 0; i < LogData.instance.pID.Count; i++)
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timePlayingTogetherBodies[i] += Time.deltaTime;
                    }
                }
            }


            if (Manager.instance.p1UInput.userInput == null && Manager.instance.p2UInput.userInput == null)
            {
                for (int i = 0; i < LogData.instance.pID.Count; i++)
                {
                    if (LogData.instance.active[i] == true)
                    {
                        LogData.instance.timeBothNotPlaying[i] += Time.deltaTime;
                    }
                }
            }
        }
    }

    public void CheckActivePlayers()
    {
        for (int i = 0; i < Manager.instance.playersId.Count; i++)
        {
            bool playerFound = false;
            for (int j = 0; j < Manager.instance.IDs.Length; j++)
            {
                if (Manager.instance.playersId[i] == Manager.instance.IDs[j])
                {
                    playerFound = true;
                }
            }

            if (playerFound == false)
            {
                Manager.instance.bodyJoints.RemoveAt(i);
                Manager.instance.bodyJointsWorld.RemoveAt(i);
                Manager.instance.playersJointsHeight.RemoveAt(i);
                Manager.instance.playersMinMaxHeight.RemoveAt(i);
                Manager.instance.playersMinMaxHeightInPixels.RemoveAt(i);
                Manager.instance.spineMidHeightInPixels.RemoveAt(i);
                Manager.instance.spineMidPos.RemoveAt(i);
                Manager.instance.spineMidPosThreshold.RemoveAt(i);
                Manager.instance.playersMidHighLowMat.RemoveAt(i);
                Manager.instance.initializeMidPos[i] = false;
                Manager.instance.zoneChanged[i] = false;
                for (int k = 0; k < LogData.instance.pID.Count; k++)
                {
                    if (LogData.instance.pID[k] == Manager.instance.playersId[i])
                    {
                        if (i == 0)
                        {
                            Manager.instance.playTones = false;
                            Manager.instance.playTonesBody = false;
                            Manager.instance.playTonesBodyHands = false;
                            for (int p = 0; p < LogData.instance.color.Count; p++)
                            {
                                if (LogData.instance.color[p] == "Red")
                                {
                                    if (LogData.instance.active[p] == true)
                                    {
                                        //Means p2 changes to p1. Add a new row where p2 becomes blue and the previous red becomes inactive
                                        LogData.instance.pID.Add(LogData.instance.pID[p]);
                                        LogData.instance.color.Add("Blue");
                                        LogData.instance.active.Add(true);
                                        LogData.instance.timeBeingActive.Add(0);
                                        LogData.instance.timeSpentAlone.Add(0);
                                        LogData.instance.playAlone.Add(0);
                                        LogData.instance.timeNotPlayingButPartnerPlays.Add(0);
                                        LogData.instance.timePlayingTogether.Add(0);
                                        LogData.instance.timePlayingTogetherHands.Add(0);
                                        LogData.instance.timePlayingTogetherBodies.Add(0);
                                        LogData.instance.timeBothNotPlaying.Add(0);
                                        LogData.instance.timeLowHand.Add(0);
                                        LogData.instance.timeHighHand.Add(0);
                                        LogData.instance.timeBackBody.Add(0);
                                        LogData.instance.timeForwBody.Add(0);
                                        LogData.instance.timeNullInput.Add(0);
                                        LogData.instance.didH5.Add(0);
                                        LogData.instance.active[p] = false;
                                        //LogData.instance.color[p] = "Blue";
                                    }
                                }
                            }
                        }
                        else if (i == 1)
                        {
                            Manager.instance.playTones = false;
                            Manager.instance.playTonesBody = false;
                            Manager.instance.playTonesBodyHands = false;
                        }
                        LogData.instance.active[k] = false;
                    }
                }
                Manager.instance.playersId.RemoveAt(i);
            }
        }
    }

    public void AddRowToData()
    {
        for (int i = 0; i < Manager.instance.playersId.Count; i++)
        {
            if (i < 2)
            {
                bool foundID = false;
                for (int j = 0; j < LogData.instance.pID.Count; j++)
                {
                    if (LogData.instance.pID[j] == Manager.instance.playersId[i])
                    {
                        foundID = true;
                    }
                }

                if (foundID == false)
                {
                    LogData.instance.pID.Add(Manager.instance.playersId[i]);
                    LogData.instance.timeBeingActive.Add(0);
                    LogData.instance.timeSpentAlone.Add(0);
                    LogData.instance.playAlone.Add(0);
                    LogData.instance.timeNotPlayingButPartnerPlays.Add(0);
                    LogData.instance.timePlayingTogether.Add(0);
                    LogData.instance.timePlayingTogetherHands.Add(0);
                    LogData.instance.timePlayingTogetherBodies.Add(0);
                    LogData.instance.timeBothNotPlaying.Add(0);
                    LogData.instance.timeLowHand.Add(0);
                    LogData.instance.timeHighHand.Add(0);
                    LogData.instance.timeBackBody.Add(0);
                    LogData.instance.timeForwBody.Add(0);
                    LogData.instance.timeNullInput.Add(0);
                    LogData.instance.didH5.Add(0);
                    if (i == 0)
                    {
                        for (int k = 0; k < LogData.instance.color.Count; k++)
                        {
                            if (LogData.instance.color[k] == "Blue")
                            {
                                LogData.instance.active[k] = false;
                            }
                        }
                        LogData.instance.color.Add("Blue");
                        LogData.instance.active.Add(true);
                    }
                    else
                    {
                        for (int k = 0; k < LogData.instance.color.Count; k++)
                        {
                            if (LogData.instance.color[k] == "Red")
                            {
                                LogData.instance.active[k] = false;
                            }
                        }
                        LogData.instance.color.Add("Red");
                        LogData.instance.active.Add(true);
                    }
                }
            }
        }
    }

    public void TimeBeingActive()
    {
        for (int i = 0; i < LogData.instance.pID.Count; i++)
        {
            if (LogData.instance.active[i] == true)
            {
                LogData.instance.timeBeingActive[i] += Time.deltaTime;
            }
        }
    }

    public void TimeSpentAlone()
    {
        if (Manager.instance.playersId.Count == 1)
        {
            for (int i = 0; i < LogData.instance.active.Count; i++)
            {
                if (LogData.instance.active[i] == true)
                {
                    LogData.instance.timeSpentAlone[i] += Time.deltaTime;
                }
            }
        }
    }

    public void PlayAlone()
    {
        if (Manager.instance.playersId.Count > 1)
        {
            if ((Manager.instance.p1UInput.userInput == "high" || Manager.instance.p1UInput.userInput == "low" || Manager.instance.p1UInput.userInput == Manager.instance.p1UInput.targetBackInput || Manager.instance.p1UInput.userInput == Manager.instance.p1UInput.targetForwardInput) && Manager.instance.p2UInput.userInput == null)  //This statement doesn't work properly cause of p1UInput.targetBackInput and p1UInput.targetForwardInput
            {
                for (int i = 0; i < LogData.instance.color.Count; i++)
                {
                    if (LogData.instance.color[i] == "Blue")
                    {
                        if (LogData.instance.active[i] == true)
                        {
                            LogData.instance.playAlone[i] += Time.deltaTime;
                        }
                    }
                    else if (LogData.instance.color[i] == "Red")
                    {
                        if (LogData.instance.active[i] == true)
                        {
                            LogData.instance.timeNotPlayingButPartnerPlays[i] += Time.deltaTime;
                        }
                    }
                }
            }
            else if ((Manager.instance.p2UInput.userInput == "high" || Manager.instance.p2UInput.userInput == "low" || Manager.instance.p2UInput.userInput == Manager.instance.p2UInput.targetBackInput || Manager.instance.p2UInput.userInput == Manager.instance.p2UInput.targetForwardInput) && Manager.instance.p1UInput.userInput == null)  //This statement doesn't work properly cause of p2UInput.targetBackInput and p2UInput.targetForwardInput
            {
                for (int i = 0; i < LogData.instance.color.Count; i++)
                {
                    if (LogData.instance.color[i] == "Red")
                    {
                        if (LogData.instance.active[i] == true)
                        {
                            LogData.instance.playAlone[i] += Time.deltaTime;
                        }
                    }
                    else if (LogData.instance.color[i] == "Blue")
                    {
                        if (LogData.instance.active[i] == true)
                        {
                            LogData.instance.timeNotPlayingButPartnerPlays[i] += Time.deltaTime;
                        }
                    }
                }
            }
        }
    }
}
