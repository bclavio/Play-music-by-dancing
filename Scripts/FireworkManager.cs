using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkManager : MonoBehaviour {

    public GameObject visualFeedback;
    public float visualDisplayTime;

    bool visualCountdown;
    float visualTimer;
    GameObject[] fireworksObjects;
    ParticleSystem[] fireworksSystem;

    [HideInInspector]
    public bool activateFireworks;
    // Use this for initialization
    void Start () {

        GetParticleSystem();
        StopParticleSystem();

        visualCountdown = false;
        activateFireworks = false;

        visualTimer = visualDisplayTime;

	}
	
	// Update is called once per frame
	void Update () {

        if (activateFireworks)
        {    
            PlayParticleSystem();
            visualCountdown = true;
            activateFireworks = false;
        }
        if (visualCountdown)
        {
            visualTimer = visualTimer - Time.deltaTime;
        }
        if (visualTimer <= 0)
        {
            StopParticleSystem();

            visualCountdown = false;
            visualTimer = visualDisplayTime;
            activateFireworks = false;
        }

    }

    void GetParticleSystem()
    {
        int childObjectAmount;

        childObjectAmount = visualFeedback.transform.childCount;
        fireworksSystem = new ParticleSystem[childObjectAmount];

        for (int i = 0; i < childObjectAmount; i++)
        {
            fireworksSystem[i] = visualFeedback.transform.GetChild(i).GetComponent<ParticleSystem>();
        }

    }

    void PlayParticleSystem()
    {
        for (int i = 0; i < fireworksSystem.Length; i++)
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

}
