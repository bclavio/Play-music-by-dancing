using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayer : MonoBehaviour {

    public GameObject playerReplaceObject;

    UserInput playerInput;

    public float timeTillGenerate;

    public bool useAutoPlayer;

    float generationTime;

    float randomNumber, decisionNumber;

    string userInput;

	// Use this for initialization
	void Start () {

        generationTime = timeTillGenerate;

        playerInput = playerReplaceObject.GetComponent<UserInput>();

	}
	
	// Update is called once per frame
	void Update () {

        generationTime = generationTime - Time.deltaTime;

        if(generationTime <= 0)
        {

            randomNumber = Random.Range(1, 20);

           // Debug.Log("Random = " + randomNumber);

            generationTime = timeTillGenerate;
        }

        if(randomNumber < 10)
        {
            decisionNumber = 0;
        }
        else if(randomNumber >= 11)
        {
            decisionNumber = 1;
        }

        if (useAutoPlayer)
        {
            if (decisionNumber == 0)
            {
                playerInput.userInput = "low";

               // print(playerInput.userInput);
            }
            else if (decisionNumber == 1)
            {
                playerInput.userInput = "high";
               // print(playerInput.userInput);
            }
        }
        

	}
}
