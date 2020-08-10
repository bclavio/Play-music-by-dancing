using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{

    /*                                 THIS SCRIPT IS NOT USED!!!!!!!!!!!!
     * MADE FOR THE PURPOSE OF A STRUCTURED TUTORIAL BUT WE CHANGED IT TO A STATE MACHINE FOR THE SAME REASON*/


    public GameObject player1InputObject, player2InputObject;

    UserInput player1Input, player2Input;

    public GameObject audioPlayer1, audioPlayer2;

    public GameObject mainTextObject;

    GameObject[] textObjects;

    toneHolder audio1, audio2;

    public float tutorialTime;
    float tutorialTimer;
    float nxtTime, prevTime;

    bool tutorialStarted, pauseTime;

    bool waitingForInput, p1HandHigh, p1HandLow, p1Forward, p1Back;
    bool p2HandHigh, p2HandLow, p2Forward, p2Back;
    bool socialInter1;

    public static bool highSession, lowSession;

    string userInput1, userInput2;

    public float standardDisplayTime, feedbackDisplayTime;
    // Use this for initialization
    void Start()
    {

        p1HandHigh = false;
        p1HandLow = false;
        p1Forward = false;
        p1Back = false;
        p2HandHigh = false;
        p2HandLow = false;
        p2Forward = false;
        p2Back = false;
        socialInter1 = false;
        highSession = false;
        lowSession = false;

        waitingForInput = false;

        GetTextObjects();

        player1Input = player1InputObject.GetComponent<UserInput>();
        player2Input = player2InputObject.GetComponent<UserInput>();

        audio1 = audioPlayer1.GetComponent<toneHolder>();
        audio2 = audioPlayer2.GetComponent<toneHolder>();

        tutorialStarted = false;
        pauseTime = false;
        tutorialTimer = 0;
        

        StartCoroutine(DisplayTutorialText());

    }

    // Update is called once per frame
    void Update()
    {

        //userInput1 = player1Input.userInput;
        //userInput2 = player2Input.userInput;

        userInput1 = PlayerIO(player1Input, userInput1);
        userInput2 = PlayerIO(player2Input, userInput2);

        Debug.Log("userI 1 = " + userInput1 + " ; p1 uin = "+ player1Input.userInput);

        if (userInput1 != player1Input.inputHigh && userInput1 != player1Input.inputLow && userInput1 != player1Input.targetForwardInput && userInput1 != player1Input.targetBackInput)
        {
            userInput1 = null;
        }
        if (userInput2 != player2Input.inputHigh && userInput2 != player2Input.inputLow && userInput2 != player2Input.targetForwardInput && userInput2 != player2Input.targetBackInput)
        {
            userInput2 = null;
        }

      // userInput1 = PlayerIO(player1Input, userInput1);
      // userInput2 = PlayerIO(player2Input, userInput2);

        //Debug.Log("Tutorial Timer = " + (int)tutorialTimer);

        if ((userInput1 != null || userInput2 != null) && !tutorialStarted)
        {
            //Debug.Log("user input taken");
            
            tutorialStarted = true;
            UserInputNull();
        }

        if (tutorialStarted && tutorialTimer != tutorialTime)
        {

            if (!pauseTime)
            {
                tutorialTimer = tutorialTimer + Time.deltaTime;
            }
            if (!waitingForInput)
            {
                //Debug.Log("NOT WAITING FOR INPUT");
                UserInputNull();
            }
            /* activateTextOnTime(tutorialTimer, tutorialStartTime, liftHandsTime, 0);
             activateTextOnTime(tutorialTimer, liftHandsTime, showHighLowTime, 1);
             activateTextOnTime(tutorialTimer, showHighLowTime, firstP1, 2);
             activateTextOnTime(tutorialTimer, firstP1, p1ReachHigh, 3);

             textWaitForInput(tutorialTimer, p1ReachHigh, p1GoodJobHigh, 4, player1Input.userInput, player1Input.inputHigh);

             activateTextOnTime(tutorialTimer, p1GoodJobHigh, p1ReachLow, 5);*/
        }
    }

    IEnumerator DisplayTutorialText()
    {

        // Waiting for input
        Debug.Log("Coroutine accessed");
        activateTextOnPos(0);
        yield return new WaitUntil(() => tutorialStarted);

        // input given, tutorial started

        activateTextOnPos(1);
        yield return new WaitForSeconds(standardDisplayTime);

        activateTextOnPos(2);
        yield return new WaitForSeconds(standardDisplayTime);

        activateTextOnPos(3);
        yield return new WaitForSeconds(standardDisplayTime);

        // Player 1 tutorial

        activateTextOnPos(4);
        yield return new WaitForSeconds(standardDisplayTime);

        highSession = true;
        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(5);

        while(!p1HandHigh)
        {
            UserInputNull();
            userInput1 = PlayerIO(player1Input, userInput1);
            textWaitForInput(player1Input, userInput1, player1Input.inputHigh, ref p1HandHigh);
            yield return null;
        }
        yield return new WaitUntil(() => p1HandHigh == true);
        highSession = false;

        activateTextOnPos(6);
        yield return new WaitForSeconds(feedbackDisplayTime);
        lowSession = true;

        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(7);

        while (!p1HandLow)
        {
            textWaitForInput(player1Input, userInput1, player1Input.inputLow, ref p1HandLow);
            yield return null;
        }
        yield return new WaitUntil(() => p1HandLow == true);
        lowSession = false;

        activateTextOnPos(8);
        yield return new WaitForSeconds(feedbackDisplayTime);

        activateTextOnPos(9);
        yield return new WaitForSeconds(standardDisplayTime);

        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(10);

        while (!p1Forward)
        {
            textWaitForInput(player1Input, userInput1, player1Input.targetForwardInput, ref p1Forward);
            yield return null;
        }
        yield return new WaitUntil(() => p1Forward == true);

        activateTextOnPos(11);
        yield return new WaitForSeconds(feedbackDisplayTime);

        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(12);

        while (!p1Back)
        {
            textWaitForInput(player1Input, userInput1, player1Input.targetBackInput, ref p1Back);
            yield return null;
        }
        yield return new WaitUntil(() => p1Back == true);

        activateTextOnPos(13);
        yield return new WaitForSeconds(feedbackDisplayTime);

        //Player 2 tutorial start

        activateTextOnPos(14);
        yield return new WaitForSeconds(standardDisplayTime);

        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(15);

        while (!p2HandHigh)
        {
            textWaitForInput(player2Input, userInput2, player2Input.inputHigh, ref p2HandHigh);
            yield return null;
        }
        yield return new WaitUntil(() => p2HandHigh == true);

        activateTextOnPos(16);
        yield return new WaitForSeconds(feedbackDisplayTime);

        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(17);

        while (!p2HandLow)
        {
            textWaitForInput(player2Input, userInput2, player2Input.inputLow, ref p2HandLow);
            yield return null;
        }
        yield return new WaitUntil(() => p2HandLow == true);

        activateTextOnPos(18);
        yield return new WaitForSeconds(feedbackDisplayTime);

        activateTextOnPos(19);
        yield return new WaitForSeconds(standardDisplayTime);

        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(20);

        while (!p2Forward)
        {
            textWaitForInput(player2Input, userInput2, player2Input.targetForwardInput, ref p2Forward);
            yield return null;
        }
        yield return new WaitUntil(() => p2Forward == true);

        activateTextOnPos(21);
        yield return new WaitForSeconds(feedbackDisplayTime);

        UserInputNull();
        waitingForInput = true;
        activateTextOnPos(22);

        while (!p2Back)
        {

            textWaitForInput(player2Input, userInput2, player2Input.targetBackInput, ref p2Back);
            yield return null;
        }
        yield return new WaitUntil(() => p2Back == true);

        activateTextOnPos(23);
        yield return new WaitForSeconds(feedbackDisplayTime);

        activateTextOnPos(24);
        yield return new WaitForSeconds(standardDisplayTime);

        activateTextOnPos(25);
        yield return new WaitForSeconds(standardDisplayTime);

    }

    string PlayerIO(UserInput playerInput, string userInput)
    {

        if (playerInput.userInput != null)
        {
            userInput = playerInput.userInput;
        }
        else
        {
            userInput = null;
        }

        return userInput;

    }

    void GetTextObjects()
    {

        int textObjectAmounts = mainTextObject.transform.childCount;

        textObjects = new GameObject[textObjectAmounts];

        for (int i = 0; i < textObjectAmounts; i++)
        {
            textObjects[i] = mainTextObject.transform.GetChild(i).gameObject;
            textObjects[i].SetActive(false);
        }

    }

    void UserInputNull()
    {
        userInput1 = null;
        userInput2 = null;

        player1Input.userInput = null;
    }

    void activateTextOnPos(int arrTextPos)
    {
        Debug.Log("Text Pos = " + arrTextPos);
        for (int k = 0; k < textObjects.Length; k++)
        {
            if (k == arrTextPos)
            {
                textObjects[k].SetActive(true);
            }
            else
            {
                textObjects[k].SetActive(false);
            }
        }
    }

    void textWaitForInput(UserInput mainInput, string userInput, string targetInput, ref bool targetBool)
    {

        userInput = mainInput.userInput;

        Debug.Log("textwfi userinput = " + userInput + " target input = " + targetInput);
        Debug.Log("mainInput = " + mainInput.userInput);

        if (userInput == targetInput)
        {
            targetBool = true;
            waitingForInput = false;
        }
        else
        {
            UserInputNull();
            return;
        }
        
    }

    void SetNextTime(float nextTimeDuration)
    {

        prevTime = nxtTime;
        nxtTime = prevTime + nextTimeDuration;

    }



}
