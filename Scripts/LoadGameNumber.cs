using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameNumber : MonoBehaviour {

	
	void Start () {
        GetComponent<Text>().text = SavedData.instance.game.ToString();
	}
	
}
