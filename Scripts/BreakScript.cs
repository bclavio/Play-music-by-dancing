using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakScript : MonoBehaviour
{

    public float baseWaitingTime, extraWaitingTime;
    float countTime;
    int scene;
    bool reachedTime;


    void Start()
    {
        countTime = 0;
        scene = -1;
        reachedTime = false;
    }


    void Update()
    {
        countTime += Time.deltaTime;

        if (countTime >= baseWaitingTime && !reachedTime)
        {
            scene = GetComponent<KinectForBreak>().CheckPlayersAfterGame();
            reachedTime = true;
            
            if(scene == 1) //Means has to play again the game
            {
                SceneManager.LoadScene(SavedData.instance.song);
            }
            else if(scene == 0)
            {
                StartCoroutine(WaitBitMore(scene));
            }
        }
    }

    IEnumerator WaitBitMore(int scene)
    {
        yield return new WaitForSeconds(extraWaitingTime);
        TutorialPlayers.instance.playersPlayedTut.Clear();
        SceneManager.LoadScene(scene);
    }
}
