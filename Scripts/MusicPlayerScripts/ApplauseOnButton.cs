using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplauseOnButton : MonoBehaviour {

    public List<AudioClip> feedback = new List<AudioClip>();
    int feedbackSelector, prevFeedback;
   // public Particle fireworks;
    public GameObject visualFeedback;
    public string inputKey;
    public float visualDisplayTime;
    public bool isFireworks;
    AudioSource applauseSource;
    bool visualCountdown;
    float visualTimer;

    GameObject[] fireworksObjects;
    ParticleSystem[] fireworksSystem;

	// Use this for initialization
	void Start () {
        applauseSource = gameObject.AddComponent<AudioSource>();

        feedbackSelector = 0;
        prevFeedback = 0;

        if (isFireworks)
        {
            GetParticleSystem();
        }
        if (isFireworks)
        {
            visualFeedback.SetActive(true);
            StopParticleSystem();
        }
        else
        {
            visualFeedback.SetActive(false);
        } 
        visualCountdown = false;
        visualTimer = visualDisplayTime;

    }
	
	// Update is called once per frame
	void Update () {

        RandomizeFeedback();

        if (/*Input.GetButtonDown(inputKey)*/ Manager.instance.didHigh5 == true && !applauseSource.isPlaying)
        {
            applauseSource.PlayOneShot(feedback[feedbackSelector]);
            
            if (isFireworks)
            {
                PlayParticleSystem();
            }
            else
            {
                visualFeedback.SetActive(true);
            }
            visualCountdown = true;
            Manager.instance.didHigh5 = false;       //I don't know if it is the correct placement here. It plays two applauses before it stops.
        }

        if (visualCountdown)
        {
            visualTimer = visualTimer - Time.deltaTime;
        }
        

        if (visualTimer <= 0)
        {
           
            if (isFireworks)
            {
                StopParticleSystem();
            }
            else
            {
                visualFeedback.SetActive(false);
            }
            
            visualCountdown = false;
            visualTimer = visualDisplayTime;
        }

	}

    void GetParticleSystem()
    {
        int childObjectAmount;

        childObjectAmount = visualFeedback.transform.childCount;
        fireworksSystem = new ParticleSystem[childObjectAmount];

        for(int i = 0; i < childObjectAmount; i++)
        {
            fireworksSystem[i] = visualFeedback.transform.GetChild(i).GetComponent<ParticleSystem>();
        }

    }

    void PlayParticleSystem()
    {
        for(int i = 0; i < fireworksSystem.Length; i++)
        {
            fireworksSystem[i].Play(true);
        }
    }

    void StopParticleSystem()
    {
        for (int i = 0; i < fireworksSystem.Length; i++)
        {
            fireworksSystem[i].Stop(true);
        }
    }

    void RandomizeFeedback()
    {

        feedbackSelector = Random.Range(0, feedback.Count);

        if(feedbackSelector == prevFeedback)
        {
            feedbackSelector = Random.Range(0, feedback.Count);
            prevFeedback = feedbackSelector;
        }
        else
        {
            prevFeedback = feedbackSelector;
        }


    }
}
