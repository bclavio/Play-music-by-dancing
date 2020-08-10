using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TonesFeedback : MonoBehaviour
{

    public List<Material> nodes = new List<Material>();
    public List<AudioClip> positiveFeedback = new List<AudioClip>();
    public AudioClip highFive;
    AudioSource pfSource;

    public float timeBeforeStart;
    float startCheckTimer;
    public float changeToneTime;
    float timeCounter;
    float playTogetherTimer, playTogetherCooldown;
    int prevFeedback;

    int togetherCounter;

    bool firstPlay, onCooldown, musicStarted;
    public int cooldown, initialFeedback, nextFeedback;

    void Start()
    {
        GetComponent<ParticleSystemRenderer>().material = nodes[0];
        timeCounter = 0;
        GetComponent<ParticleSystem>().enableEmission = false;
        pfSource = gameObject.AddComponent<AudioSource>();

        playTogetherCooldown = 5;
        playTogetherTimer = 0;
        firstPlay = true;
        onCooldown = false;
        togetherCounter = 0;
        startCheckTimer = timeBeforeStart;
        musicStarted = false;
    }

    void Update()
    {
        if (timeCounter > changeToneTime)
        {
            var randomTone = Random.Range(0, nodes.Count);
            GetComponent<ParticleSystemRenderer>().material = nodes[randomTone];
            timeCounter = 0;
        }
        timeCounter += Time.deltaTime;

        if (!musicStarted)
        {
            startCheckTimer = startCheckTimer - Time.deltaTime;
        }
        if(!musicStarted && startCheckTimer < 0)
        {
            musicStarted = true;
        }

        if (onCooldown)
        {
            playTogetherCooldown = playTogetherCooldown + Time.deltaTime;
            if(playTogetherCooldown >= cooldown)
            {
                firstPlay = true;
                playTogetherTimer = 0;
                onCooldown = false;
            }
        }

        if (Manager.instance.playTones == true || Manager.instance.playTonesBody == true || Manager.instance.playTonesBodyHands == true)
        {

            if (musicStarted)
            {
                
                playTogetherTimer = playTogetherTimer + Time.deltaTime;

                int randomFeedback = Random.Range(0, positiveFeedback.Count);
                if (randomFeedback == prevFeedback)
                {
                    randomFeedback = Random.Range(0, positiveFeedback.Count);
                }

                if (playTogetherTimer > initialFeedback && firstPlay && !onCooldown)
                {
                    pfSource.PlayOneShot(positiveFeedback[randomFeedback]);
                    prevFeedback = randomFeedback;
                    firstPlay = false;
                    playTogetherTimer = 0;
                    togetherCounter++;
                }
                else if (playTogetherTimer >= nextFeedback && !firstPlay && !onCooldown)
                {
                    pfSource.PlayOneShot(positiveFeedback[randomFeedback]);
                    prevFeedback = randomFeedback;

                    playTogetherTimer = 0;
                    togetherCounter++;
                }
                if (togetherCounter >= 2)
                {
                    if (!pfSource.isPlaying)
                    {
                        pfSource.PlayOneShot(highFive);
                        togetherCounter = 0;
                    }
                }
            }
            GetComponent<ParticleSystem>().enableEmission = true;
            for (int i = 0; i < LogData.instance.pID.Count; i++)
            {
                if (LogData.instance.active[i] == true)
                {
                    LogData.instance.timePlayingTogether[i] += Time.deltaTime;
                }
            }
        }
        else
        {
            if (!firstPlay)
            {
                playTogetherCooldown = 0;
                onCooldown = true;
            }
            GetComponent<ParticleSystem>().enableEmission = false;
        }
    }
}
