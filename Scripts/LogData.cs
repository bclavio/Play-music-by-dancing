using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LogData : MonoBehaviour
{
    public static LogData instance;
    DateTime dateAndTime;

    //[HideInInspector]
    public List<ulong> pID;
    //[HideInInspector]
    public List<string> color;
    //[HideInInspector]
    public List<bool> active;
    //[HideInInspector]
    public List<float> timeBeingActive, timeSpentAlone, playAlone, timePlayingTogether, timePlayingTogetherHands, timePlayingTogetherBodies, timeBothNotPlaying, timeNotPlayingButPartnerPlays, timeLowHand, timeHighHand, timeBackBody, timeForwBody, timeNullInput;
    //[HideInInspector]
    public List<int> didH5;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        dateAndTime = DateTime.Now;
        pID = new List<ulong>();
        color = new List<string>();
        active = new List<bool>();
        timeBeingActive = new List<float>();
        timeSpentAlone = new List<float>();
        playAlone = new List<float>();
        timePlayingTogether = new List<float>();
        timePlayingTogetherHands = new List<float>();
        timePlayingTogetherBodies = new List<float>();
        timeBothNotPlaying = new List<float>();
        timeNotPlayingButPartnerPlays = new List<float>();
        timeLowHand = new List<float>();
        timeHighHand = new List<float>();
        timeBackBody = new List<float>();
        timeForwBody = new List<float>();
        timeNullInput = new List<float>();
        didH5 = new List<int>();

    }

    public void WriteFile()
    {
        if (!File.Exists("C:/Users/User/Desktop/DataCollection/" + dateAndTime.ToString("yyyy_MM_dd__HH_mm_ss") + ".txt"))
        {
            //File.AppendAllText("C:/Users/User/Desktop/DataCollection/" + dateAndTime.ToString("yyyy_MM_dd__HH_mm_ss") + ".txt", "Game,PlayerID,Color,TimeWasActive,Active,TimeSpentAlone,PlayedAlone,TimeDidntPlayButPartnerPlayed,PlayedTogether,PlayedTogetherHands,PlayedTogetherBodies,BothDidntPlay,H5s,LowHand,HighHand,BackBody,ForwardBody,NullInput");

            for (int i = 0; i < pID.Count; i++)
            {//"F2" is being used in order to keep only the first two decimals in the float number
                File.AppendAllText("C:/Users/User/Desktop/DataCollection/" + dateAndTime.ToString("yyyy_MM_dd__HH_mm_ss") + ".txt",   //THIS PATH NEEDS TO BE CHANGED BECAUSE IT WON'T WORK IN A DIFFERENT COMPUTER.
                    "\r\n" + SavedData.instance.game + "," + pID[i] + "," + color[i] + "," + timeBeingActive[i].ToString("F2") + "," + active[i] + "," + timeSpentAlone[i].ToString("F2") + "," + playAlone[i].ToString("F2") + "," + 
                    timeNotPlayingButPartnerPlays[i].ToString("F2") + "," + timePlayingTogether[i].ToString("F2") + "," + timePlayingTogetherHands[i].ToString("F2") + "," + timePlayingTogetherBodies[i].ToString("F2") + "," +
                    timeBothNotPlaying[i].ToString("F2") + "," + didH5[i] + "," + timeLowHand[i].ToString("F2") + "," + timeHighHand[i].ToString("F2") + "," + timeBackBody[i].ToString("F2") + "," + timeForwBody[i].ToString("F2") + "," + 
                    timeNullInput[i].ToString("F2"));
            }
        }

    }
}
