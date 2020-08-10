using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayers : MonoBehaviour
{

    public static TutorialPlayers instance;

    public List<ulong> playersPlayedTut;
    public GameObject tutObj;
    public float timeToStart;

    [HideInInspector]
    public float timeToStartTemp;

    void Start()
    {

        if (instance == null)
        {
            instance = this;
            playersPlayedTut = new List<ulong>();
            playersPlayedTut.Clear();
            timeToStartTemp = timeToStart;
            DontDestroyOnLoad(tutObj);           
        }
        else
        {
            DestroyImmediate(tutObj);
        }

    }
}
