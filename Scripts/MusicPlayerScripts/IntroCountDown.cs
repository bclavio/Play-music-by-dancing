using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCountDown : MonoBehaviour {

    public GameObject introTextObject;
    public GameObject[] countDownObjects;

    public List<AudioClip> feedback = new List<AudioClip>();
    AudioSource feedbackSrc;

    Text introText;
    Shadow introTextShadow;

    public float introTime;
    float timer;

    [HideInInspector]
    public bool introIsPlaying;

    bool play1, play2, play3, play4, play5;

    // Use this for initialization
    void Awake()
    {
        introIsPlaying = false;
    }

    void Start () {

        feedbackSrc = gameObject.AddComponent<AudioSource>();

        introText = introTextObject.GetComponent<Text>();
        introTextShadow = introTextObject.GetComponent<Shadow>();

        InitializeCountDownText();
        DeactivateAll();

        play1 = false;
        play2 = false;
        play3 = false;
        play4 = false;
        play5 = false;

        timer = introTime;

	}
	
	// Update is called once per frame
	void Update () {

        if (introIsPlaying && timer != -1)
        {
            timer = timer - Time.deltaTime;

            if(timer > 3)
            {
                if (!play1)
                {
                    ActivateText(true);
                    play1 = true;
                }
                
            }
            else if(timer <= 3 && timer > 2)
            {
                if(!play2)
                {
                    ActivateText(false);
                    ActivateCountDown(3, true);
                    play2 = true;
                }                
            }
            else if(timer <= 2 && timer > 1)
            {
                if (!play3)
                {
                    ActivateCountDown(3, false);
                    ActivateCountDown(2, true);
                    play3 = true;
                }
            }
            else if(timer <= 1 && timer > 0)
            {
                if (!play4)
                {
                    ActivateCountDown(2, false);
                    ActivateCountDown(1, true);
                    play4 = true;
                }
            }
            else if(timer <= 0 && timer > -1)
            {
                if (!play5)
                {
                    ActivateCountDown(1, false);
                    ActivateCountDown(0, true);
                    play5 = true;
                }
            }
            else
            {
                DeactivateAll();
                //print("Now I am playing");
                timer = -1;
            }
        }


	}

    void InitializeCountDownText()
    {
        int textObjectAmount = introTextObject.transform.childCount;

        countDownObjects = new GameObject[textObjectAmount];

        for(int i = 0; i < textObjectAmount; i++)
        {
            countDownObjects[i] = introTextObject.transform.GetChild(i).gameObject;
        }


    }

    void DeactivateAll()
    {


        for(int i = 0; i < countDownObjects.Length; i++)
        {
            countDownObjects[i].SetActive(false);
        }

        introText.enabled = false;
        introTextShadow.enabled = false;

    }

    void ActivateText(bool setActive)
    {
        introText.enabled = setActive;
        introTextShadow.enabled = setActive;

        if (setActive)
        {
            feedbackSrc.PlayOneShot(feedback[4]);
        }
        

    }

    void ActivateCountDown(int numberToActivate, bool setActive)
    {
        countDownObjects[numberToActivate].SetActive(setActive);

        if (setActive)
        {
            feedbackSrc.PlayOneShot(feedback[numberToActivate]);
        }
       

    }

}
