using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTestInput : MonoBehaviour
{

    public GameObject player1InputObject, player2InputObject;
    UserInput player1Input, player2Input;

    public string p1handInput1, p1handInput2, p1forward, p1back, p2handInput1, p2handInput2, p2forward, p2back;

    // Use this for initialization
    void Start()
    {

        player1Input = player1InputObject.GetComponent<UserInput>();
        player2Input = player2InputObject.GetComponent<UserInput>();

    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(Input.inputString);

        if (Input.inputString == p1handInput1)
        {
            player1Input.userInput = player1Input.inputHigh;
            
        }
        if (Input.inputString == p1handInput2)
        {
            
            player1Input.userInput = player1Input.inputLow;
        }
        if (Input.inputString == p1forward)
        {
            
            player1Input.userInput = player1Input.targetForwardInput;
        }
        if (Input.inputString == p1back)
        {
            
            player1Input.userInput = player1Input.targetBackInput;
        }

        if (Input.inputString == p2handInput1)
        {
            
            player2Input.userInput = player2Input.inputHigh; ;
        }
        
        if (Input.inputString == p2handInput2)
        {
            
            player2Input.userInput = player2Input.inputLow; ;
        }
        if (Input.inputString == p2forward)
        {
            
            player2Input.userInput = player2Input.targetForwardInput; ;
        }
        if (Input.inputString == p2back)
        {
            
            player2Input.userInput = player2Input.targetBackInput; ;
        }
    }
}
