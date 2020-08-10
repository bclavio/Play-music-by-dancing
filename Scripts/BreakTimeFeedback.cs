using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTimeFeedback : MonoBehaviour {

    public AudioClip feedback;

    AudioSource feedbackSrc;

    public float timeBeforeFeedback;

    bool chekingTime;

	// Use this for initialization
	void Start () {
        feedbackSrc = gameObject.AddComponent<AudioSource>();
        chekingTime = true;
	}
	
	// Update is called once per frame
	void Update () {

        if(chekingTime && timeBeforeFeedback > 0)
        {
            timeBeforeFeedback = timeBeforeFeedback - Time.deltaTime;
        }
        else if(chekingTime && timeBeforeFeedback <= 0)
        {
            Debug.Log("audio played");
            feedbackSrc.PlayOneShot(feedback);
            chekingTime = false;
        }	
	}
}
