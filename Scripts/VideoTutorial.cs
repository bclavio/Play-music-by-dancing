using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoTutorial : MonoBehaviour
{

    public List<Material> videoMats = new List<Material>();
    public GameObject canvas;
    int counter, countReps;
    Renderer rend;
    MovieTexture video;
    public int repeatVideo;
    bool videoStarted, readyToPlay, tutorialStarted;
    float countToStart;
    AudioSource aud;
    bool registered;
    float countReset;
    public float countUntilReset;

    void Start()
    {
        countReset = 0;
        canvas.SetActive(true);
        counter = 0;
        countReps = 0;
        countToStart = 0;
        rend = GetComponent<Renderer>();
        videoStarted = false;
        tutorialStarted = false;
        readyToPlay = true;
        aud = GetComponent<AudioSource>();
        registered = false;
    }

    void Update()
    {
        if (KinectForTutorial.instance.playersIdTut.Count > 0)
        {
            registered = true;
            countReset = 0;
            if (!tutorialStarted)
            {
                countToStart += Time.deltaTime;
                if (countToStart >= TutorialPlayers.instance.timeToStartTemp)
                {
                    canvas.SetActive(false);
                    tutorialStarted = true;
                }
            }
        }
        else
        {
            countReset += Time.deltaTime;
            if (countReset >= countUntilReset)
            {
                countReset = 0;
                countToStart = 0;
                tutorialStarted = false;
                canvas.SetActive(true);
                counter = 0;
                countReps = 0;
                videoStarted = false;
                readyToPlay = true;
                rend.material = videoMats[counter];
                video = (MovieTexture)rend.material.mainTexture;
                video.Stop();
                aud.Stop();
                if (registered == true)
                {
                    SceneManager.LoadScene(0);  //Load again the tutorial scene
                }
            }
        }

        if (counter < videoMats.Count)
        {
            if (!canvas.activeInHierarchy)
            {
                if (readyToPlay)
                {

                    rend.material = videoMats[counter];
                    video = (MovieTexture)rend.material.mainTexture;

                    video.Play();
                    aud.Play();
                    videoStarted = true;
                    readyToPlay = false;
                }

                if (videoStarted)
                {
                    if (!video.isPlaying)
                    {
                        video.Stop();
                        aud.Stop();
                        countReps++;
                        if (countReps == repeatVideo)
                        {
                            counter++;
                            countReps = 0;
                        }
                        videoStarted = false;
                        readyToPlay = true;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < KinectForTutorial.instance.playersIdTut.Count; i++)
            {
                TutorialPlayers.instance.playersPlayedTut.Add(KinectForTutorial.instance.playersIdTut[i]);
            }
            if (SavedData.instance.song == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(SavedData.instance.song);  //Fixed to have only two gameplay scenes into the game.
            }
        }
    }
}
