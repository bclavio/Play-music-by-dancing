using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SavedData : MonoBehaviour {

    public static SavedData instance;

    [HideInInspector]
    public int song, game; ////song is fixed to take 1 or 2 depending on the two fixed scene songs we have. game increases by how many games were fully ended.

	// Use this for initialization
	void Start () {
	
        if(instance == null)
        {
            instance = this;
            game = 1;
            Load();
        }
        	
	}

    //SAVE-LOAD
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/GameplayProgress.dat"); /////////Not dynamic saves. it is only one save file. doesn't matter. we would need more if we would like to have different save files

        PlayerData data = new PlayerData();
        data.song = song;
        data.game = game;


        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath + "/GameplayProgress.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/GameplayProgress.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            song = data.song;
            game = data.game;

        }

    }
}

[Serializable]
class PlayerData
{

    public int song;
    public int game;

}