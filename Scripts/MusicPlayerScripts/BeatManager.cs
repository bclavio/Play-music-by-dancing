using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{

    public AudioClip drumBeat, bassTone;

    [HideInInspector]
    public int bassSetAmount;

    [HideInInspector]
    public AudioSource drumAudioSource, bassAudioSource;

    public float volume;

    public bool useBeat;

    public float toneFollowBeat, rythmFollowBeat;

    [HideInInspector]
    public float melodicTimer, rythmTimer;

    bool isPlaying;

    [HideInInspector]
    public bool startBass;

    [HideInInspector]
    public UserInput getRythmInput;

    // Use this for initialization
    
    void Awake()
    {
        getRythmInput = gameObject.AddComponent<UserInput>();
    }

    void Start()
    {

        

        drumAudioSource = gameObject.AddComponent<AudioSource>();
        bassAudioSource = gameObject.AddComponent<AudioSource>();
        isPlaying = false;

        melodicTimer = toneFollowBeat;
        rythmTimer = rythmFollowBeat;

     

    }

    // Update is called once per frame
    void Update()
    {

        melodicTimer = melodicTimer - Time.deltaTime;
        rythmTimer = rythmTimer - Time.deltaTime;

        if (drumAudioSource.isPlaying)
        {
            isPlaying = true;
        }
        if (drumAudioSource.isPlaying == false)
        {
            isPlaying = false;
        }

        if (useBeat && !isPlaying)
        {

            drumAudioSource.PlayOneShot(drumBeat, volume);
            melodicTimer = toneFollowBeat;
            rythmTimer = rythmFollowBeat;

        }
        if (!useBeat)
        {
            drumAudioSource.Stop();
            isPlaying = false;
        }

        


    }



}
