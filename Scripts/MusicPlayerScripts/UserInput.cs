using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {

    //preset input strings. Input variables must match these strings for input
    public string inputHigh, inputMid, inputLow, targetForwardInput, targetBackInput;

    //input variables to set an input to
    //public string handInput1, handInput2, forwardInput, backInput;

    [HideInInspector]
    public string userInput, userDepth, getInput, depthInput;

    [HideInInspector]
    public bool isPlayer2;

    public AudioSource audioSource;

    // Use this for initialization
    void Start () {

        audioSource = gameObject.GetComponent<AudioSource>();

	}
	
}
