using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public GameObject melodicObject, rythmObject;

    toneHolder[] melodicSets, rythmSets;
    int melodicSetAmount, rythmSetAmount;

    public AudioClip drumClip, applause;

    public float volume;
    
    AudioClip rythmSetOneHigh, rythmSetOneLow, rythmSetTwoHigh, rythmSetTwoLow;
    AudioClip melodicSetOneHigh, melodicSetOneLow, melodicSetTwoHigh, melodicSetTwoLow;

    AudioSource drumSource;
    AudioSource rythmSourceOneHigh, rythmSourceOneLow, rythmSourceTwoHigh, rythmSourceTwoLow;
    AudioSource melodicSourceOneHigh, melodicSourceOneLow, melodicSourceTwoHigh, melodicSourceTwoLow;

    public bool rythmUseTwoSets, melodicUseTwoSets;

    [HideInInspector]
    public bool startMusic;

    public float mainTrackTime;
    float mainTrackTimer;

    float melodicBeatTimer, rythmBeatTimer, beatTimer;
    public float melodicBeatInterval, rythmBeatInterval, beatInterval;
    public float timeOffset;
    public float earlyApplauseTime;

    [HideInInspector]
    public bool melodyPlayable, rythmPlayable, inputReady, playApplause;

    timeManager timeSetter;

    FireworkManager fireworkManager;

    IntroCountDown countDown;



    // Use this for initialization
    void Start()
    {
        melodyPlayable = false;
        rythmPlayable = false;
        inputReady = false;
        GetToneSets();
        AudioSetup();

        startMusic = false;

        mainTrackTime = drumClip.length;

        mainTrackTimer = 0;
        beatTimer = beatInterval;
        melodicBeatTimer = melodicBeatInterval;
        rythmBeatTimer = rythmBeatInterval;

        playApplause = false;

        timeSetter = GetComponent<timeManager>();
        fireworkManager = GetComponent<FireworkManager>();
        countDown = GetComponent<IntroCountDown>();

    }

    // Update is called once per frame
    void Update()
    {

        if (startMusic)
        {
            mainTrackTimer = mainTrackTimer + Time.deltaTime;

            int maintrackSeconds = (int)mainTrackTimer;

            //Debug.Log("main track time = " + maintrackSeconds);
            beatTimer = beatTimer - Time.deltaTime;
            melodicBeatTimer = melodicBeatTimer - Time.deltaTime;
            rythmBeatTimer = rythmBeatTimer - Time.deltaTime;
        }

        ManageTime();

        DisableSounds();
        EnableSounds();
        ResetBeatTime();


        if (mainTrackTimer >= mainTrackTime - earlyApplauseTime && !playApplause)
        {
            startMusic = false;
            StopMusic();
            mainTrackTimer = 0;
            PlayApplause();
            playApplause = true;

            StartCoroutine("AfterEnding");
        }
        if (!drumSource.isPlaying && playApplause)
        {
            drumSource.mute = true;
            playApplause = false;
        }

        /*
        TimeCheck(beatTimer, beatInterval);
        TimeCheck(melodicBeatTimer, melodicBeatInterval);
        TimeCheck(rythmBeatTimer, rythmBeatInterval);
        */

        /*
        CheckPlayability(melodyPlayable, beatTimer, melodicBeatTimer);
        CheckPlayability(rythmPlayable, beatTimer, rythmBeatTimer);
        */


        if (startMusic)
        {

            if (!drumSource.isPlaying)
            {
                if (!inputReady)
                {
                    ResetTimers();
                    MutePlayers();
                    inputReady = true;
                }
                StartMusic();
                countDown.introIsPlaying = true;
            }

        }
        else if (!startMusic && drumSource.isPlaying && !playApplause)
        {
            StopMusic();
        }

    }

    void GetToneSets()
    {
        melodicSetAmount = melodicObject.transform.childCount;
        rythmSetAmount = rythmObject.transform.childCount;

        melodicSets = new toneHolder[melodicSetAmount];
        rythmSets = new toneHolder[rythmSetAmount];

        for (int i = 0; i < melodicSetAmount; i++)
        {
            melodicSets[i] = melodicObject.transform.GetChild(i).gameObject.GetComponent<toneHolder>();
        }
        for (int i = 0; i < rythmSetAmount; i++)
        {
            rythmSets[i] = rythmObject.transform.GetChild(i).gameObject.GetComponent<toneHolder>();
        }

    }

    void AudioSetup()
    {
        drumSource = gameObject.AddComponent<AudioSource>();
        melodicSourceOneHigh = melodicObject.AddComponent<AudioSource>();
        melodicSourceOneLow = melodicObject.AddComponent<AudioSource>();
        melodicSourceTwoHigh = melodicObject.AddComponent<AudioSource>();
        melodicSourceTwoLow = melodicObject.AddComponent<AudioSource>();

        rythmSourceOneHigh = rythmObject.AddComponent<AudioSource>();
        rythmSourceOneLow = rythmObject.AddComponent<AudioSource>();
        rythmSourceTwoHigh = rythmObject.AddComponent<AudioSource>();
        rythmSourceTwoLow = rythmObject.AddComponent<AudioSource>();

        melodicSetOneHigh = melodicSets[0].high;
        melodicSetOneLow = melodicSets[0].low;

        if (melodicUseTwoSets)
        {
            melodicSetTwoHigh = melodicSets[1].high;
            melodicSetTwoLow = melodicSets[1].low;
        }
        rythmSetOneHigh = rythmSets[0].high;
        rythmSetOneLow = rythmSets[0].low;

        if (rythmUseTwoSets)
        {
            rythmSetTwoHigh = rythmSets[1].high;
            rythmSetTwoLow = rythmSets[1].low;
        }
    }

    void StartMusic()
    {

        drumSource.PlayOneShot(drumClip, volume);

        melodicSourceOneHigh.PlayOneShot(melodicSetOneHigh, volume);
        melodicSourceOneLow.PlayOneShot(melodicSetOneLow, volume);

        if (melodicUseTwoSets)
        {
            melodicSourceTwoHigh.PlayOneShot(melodicSetTwoHigh, volume);
            melodicSourceTwoLow.PlayOneShot(melodicSetTwoLow, volume);
        }

        rythmSourceOneHigh.PlayOneShot(rythmSetOneHigh, volume);
        rythmSourceOneLow.PlayOneShot(rythmSetOneLow, volume);

        if (rythmUseTwoSets)
        {
            rythmSourceTwoHigh.PlayOneShot(rythmSetTwoHigh, volume);
            rythmSourceTwoLow.PlayOneShot(rythmSetTwoLow, volume);
        }
    }

    public void MuteOthers(int setPos, UserInput userInput)
    {
        if (!userInput.isPlayer2)
        {

            //Debug.Log("Player 1");
            if (setPos == 0 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
            {
                //Debug.Log("Player 1, Set " + setPos + ", input " + userInput.userInput);

                melodicSourceOneHigh.mute = false;
                melodicSourceOneLow.mute = true;

                if (melodicUseTwoSets)
                {
                    melodicSourceTwoHigh.mute = true;
                    melodicSourceTwoLow.mute = true;
                }
            }
            else if (setPos == 0 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
            {
                // Debug.Log("Player 1, Set " + setPos + ", input " + userInput.userInput);
                melodicSourceOneHigh.mute = true;
                melodicSourceOneLow.mute = false;

                if (melodicUseTwoSets)
                {
                    melodicSourceTwoHigh.mute = true;
                    melodicSourceTwoLow.mute = true;
                }
            }
            else if (setPos == 1 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
            {
                // Debug.Log("Player 1, Set " + setPos + ", input " + userInput.userInput);
                melodicSourceOneHigh.mute = true;
                melodicSourceOneLow.mute = true;

                melodicSourceTwoHigh.mute = false;
                melodicSourceTwoLow.mute = true;

            }
            else if (setPos == 1 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
            {
                // Debug.Log("Player 1, Set " + setPos + ", input " + userInput.userInput);
                melodicSourceOneHigh.mute = true;
                melodicSourceOneLow.mute = true;

                melodicSourceTwoHigh.mute = true;
                melodicSourceTwoLow.mute = false;
            }
            else
            {
                melodicSourceOneHigh.mute = true;
                melodicSourceOneLow.mute = true;

                melodicSourceTwoHigh.mute = true;
                melodicSourceTwoLow.mute = true;
            }
        }

        if (userInput.isPlayer2)
        {
            if (setPos == 0 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
            {
                //Debug.Log("Player 2, Set " + setPos + ", input " + userInput.userInput);
                rythmSourceOneHigh.mute = false;
                rythmSourceOneLow.mute = true;

                if (rythmUseTwoSets)
                {
                    rythmSourceTwoHigh.mute = true;
                    rythmSourceTwoLow.mute = true;
                }
            }
            else if (setPos == 0 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
            {
                //Debug.Log("Player 2, Set " + setPos + ", input " + userInput.userInput);
                rythmSourceOneHigh.mute = true;
                rythmSourceOneLow.mute = false;

                if (rythmUseTwoSets)
                {
                    rythmSourceTwoHigh.mute = true;
                    rythmSourceTwoLow.mute = true;
                }
            }
            else if (setPos == 1 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
            {
                //Debug.Log("Player 2, Set " + setPos + ", input " + userInput.userInput);
                rythmSourceOneHigh.mute = true;
                rythmSourceOneLow.mute = true;

                rythmSourceTwoHigh.mute = false;
                rythmSourceTwoLow.mute = true;


            }
            else if (setPos == 1 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
            {
                //Debug.Log("Player 2, Set " + setPos + ", input " + userInput.userInput);
                rythmSourceOneHigh.mute = true;
                rythmSourceOneLow.mute = true;

                rythmSourceTwoHigh.mute = true;
                rythmSourceTwoLow.mute = false;
            }
            else
            {
                //Debug.Log("Player 2, Set " + setPos + ", input " + userInput.userInput);
                rythmSourceOneHigh.mute = true;
                rythmSourceOneLow.mute = true;

                rythmSourceTwoHigh.mute = true;
                rythmSourceTwoLow.mute = true;
            }
        }

    }

    public void MuteOthersMelodic(int setPos, UserInput userInput)
    {
        if (setPos == 0 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
        {


            melodicSourceOneHigh.mute = false;
            melodicSourceOneLow.mute = true;

            if (melodicUseTwoSets)
            {
                melodicSourceTwoHigh.mute = true;
                melodicSourceTwoLow.mute = true;
            }
        }
        else if (setPos == 0 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
        {

            melodicSourceOneHigh.mute = true;
            melodicSourceOneLow.mute = false;

            if (melodicUseTwoSets)
            {
                melodicSourceTwoHigh.mute = true;
                melodicSourceTwoLow.mute = true;
            }
        }
        else if (setPos == 1 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
        {

            melodicSourceOneHigh.mute = true;
            melodicSourceOneLow.mute = true;

            melodicSourceTwoHigh.mute = false;
            melodicSourceTwoLow.mute = true;

        }
        else if (setPos == 1 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
        {

            melodicSourceOneHigh.mute = true;
            melodicSourceOneLow.mute = true;

            melodicSourceTwoHigh.mute = true;
            melodicSourceTwoLow.mute = false;
        }
        else
        {
            melodicSourceOneHigh.mute = true;
            melodicSourceOneLow.mute = true;

            melodicSourceTwoHigh.mute = true;
            melodicSourceTwoLow.mute = true;
        }
    }

    public void MuteOthersRythm(int setPos, UserInput userInput)
    {
        if (setPos == 0 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
        {

            rythmSourceOneHigh.mute = false;
            rythmSourceOneLow.mute = true;

            if (rythmUseTwoSets)
            {
                rythmSourceTwoHigh.mute = true;
                rythmSourceTwoLow.mute = true;
            }
        }
        else if (setPos == 0 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
        {

            rythmSourceOneHigh.mute = true;
            rythmSourceOneLow.mute = false;

            if (rythmUseTwoSets)
            {
                rythmSourceTwoHigh.mute = true;
                rythmSourceTwoLow.mute = true;
            }
        }
        else if (setPos == 1 && (userInput.userInput == userInput.inputHigh || userInput.userInput == "HighBody"))
        {

            rythmSourceOneHigh.mute = true;
            rythmSourceOneLow.mute = true;

            rythmSourceTwoHigh.mute = false;
            rythmSourceTwoLow.mute = true;


        }
        else if (setPos == 1 && (userInput.userInput == userInput.inputLow || userInput.userInput == "LowBody"))
        {

            rythmSourceOneHigh.mute = true;
            rythmSourceOneLow.mute = true;

            rythmSourceTwoHigh.mute = true;
            rythmSourceTwoLow.mute = false;
        }
        else
        {

            rythmSourceOneHigh.mute = true;
            rythmSourceOneLow.mute = true;

            rythmSourceTwoHigh.mute = true;
            rythmSourceTwoLow.mute = true;
        }
    }

    public void MuteAll()
    {

        drumSource.mute = true;

        melodicSourceOneHigh.mute = true;
        melodicSourceOneLow.mute = true;
        melodicSourceTwoHigh.mute = true;
        melodicSourceTwoLow.mute = true;

        rythmSourceOneHigh.mute = true;
        rythmSourceOneLow.mute = true;
        rythmSourceTwoHigh.mute = true;
        rythmSourceTwoLow.mute = true;
    }

    public void MutePlayers()
    {
        melodicSourceOneHigh.mute = true;
        melodicSourceOneLow.mute = true;
        melodicSourceTwoHigh.mute = true;
        melodicSourceTwoLow.mute = true;

        rythmSourceOneHigh.mute = true;
        rythmSourceOneLow.mute = true;
        rythmSourceTwoHigh.mute = true;
        rythmSourceTwoLow.mute = true;
    }

    public void StopMusic()
    {

        startMusic = false;

        drumSource.Stop();

        melodicSourceOneHigh.Stop();
        melodicSourceOneLow.Stop();
        melodicSourceTwoHigh.Stop();
        melodicSourceTwoLow.Stop();

        rythmSourceOneHigh.Stop();
        rythmSourceOneLow.Stop();
        rythmSourceTwoHigh.Stop();
        rythmSourceTwoLow.Stop();

    }

    public float TimeCheck(float timer, float interval)
    {
        if (timer >= interval - timeOffset)
        {
            timer = 0;
        }
        return timer;
    }

    public void ResetTimers()
    {
        mainTrackTimer = 0;
        beatTimer = beatInterval;
        melodicBeatTimer = melodicBeatInterval;
        rythmBeatTimer = rythmBeatInterval;
    }

    public bool CheckPlayability(bool soundPlayable, float beatOnTime, float instrumentOnTime)
    {

        if (instrumentOnTime <= 0 + timeOffset)
        {
            soundPlayable = true;
        }


        return soundPlayable;
    }

    void DisableSounds()
    {
        float beatThreshold1 = melodicBeatInterval;
        float beatThreshold2 = rythmBeatInterval;

        if (melodicBeatTimer < beatThreshold1)
        {
            melodyPlayable = false;
        }
        if (rythmBeatTimer < beatThreshold2)
        {
            rythmPlayable = false;
        }

    }

    void EnableSounds()
    {
        if (melodicBeatTimer <= 0 + timeOffset)
        {
            melodyPlayable = true;
        }
        if (rythmBeatTimer <= 0 + timeOffset)
        {
            rythmPlayable = true;
        }
    }

    void ResetBeatTime()
    {
        if (beatTimer <= 0)
        {
            beatTimer = beatInterval;
        }
        if (melodicBeatTimer <= 0)
        {
            melodicBeatTimer = melodicBeatInterval;
        }
        if (rythmBeatTimer <= 0)
        {
            rythmBeatTimer = rythmBeatInterval;
        }
    }

    void ManageTime()
    {
        if (mainTrackTimer <= timeSetter.verseTimeStart + 0.5f)
        {
            //Debug.Log("Verse 1");
            melodicBeatInterval = timeSetter.melodicBeatVerse;
            rythmBeatInterval = timeSetter.rythmBeatVerse;
        }
        if (mainTrackTimer >= timeSetter.bridgeTimeStart - 0.5f && mainTrackTimer <= timeSetter.bridgeTimeStart + 0.5f)
        {
            //Debug.Log("Bridge");
        }
        if (mainTrackTimer >= timeSetter.chorusTimeStart - 1 && mainTrackTimer <= timeSetter.chorusTimeStart + 1)
        {
            //Debug.Log("Chorus");
            melodicBeatInterval = timeSetter.melodicBeatChorus;
            rythmBeatInterval = timeSetter.rythmBeatChorus;
        }
        if (mainTrackTimer >= timeSetter.bridge2TimeStart - 1 && mainTrackTimer <= timeSetter.bridge2TimeStart + 1)
        {
            //Debug.Log("Bridge 2");
            melodicBeatInterval = timeSetter.melodicBeatVerse;
            rythmBeatInterval = timeSetter.rythmBeatVerse;
        }
        if (mainTrackTimer >= timeSetter.verse2TimeStart - 1 && mainTrackTimer <= timeSetter.verse2TimeStart + 1)
        {
            //Debug.Log("Verse 2");
            melodicBeatInterval = timeSetter.melodicBeatVerse;
            rythmBeatInterval = timeSetter.rythmBeatVerse;
        }

    }

    public void PlayApplause()
    {

        drumSource.mute = false;
        drumSource.PlayOneShot(applause);
        fireworkManager.activateFireworks = true;

    }


    IEnumerator AfterEnding()
    {
        yield return new WaitForSeconds(Manager.instance.waitAtTheEnd);
        LogData.instance.WriteFile(); //Call function to save data after the final fireworks ended.
        if (SavedData.instance.song == 0)
        {
            SavedData.instance.song = 2;
        }
        else
        {
            if (SavedData.instance.song == 1)
            {
                SavedData.instance.song = 2;
            }
            else if (SavedData.instance.song == 2)
            {
                SavedData.instance.song = 1;
            }
        }
        SavedData.instance.game++;
        SavedData.instance.Save();
        SceneManager.LoadScene(3); //Fixed case where the break scene is always at position 3 in the build settings
    }
}


