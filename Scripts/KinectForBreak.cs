using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class KinectForBreak : MonoBehaviour {


    public static KinectForBreak instance;

    Kinect.KinectSensor sensor;
    Kinect.MultiSourceFrameReader reader;
    IList<Kinect.Body> bodies;
    [HideInInspector]
    public List<ulong> playersIdBreak;       //Remove the public. Did only for observation.
    [HideInInspector]
    public ulong[] IDsBreak; //Temporare save of IDs every frame.
    bool foundId;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        sensor = Kinect.KinectSensor.GetDefault();

        if (sensor != null)
        {
            sensor.Open();
        }

        reader = sensor.OpenMultiSourceFrameReader(Kinect.FrameSourceTypes.Body | Kinect.FrameSourceTypes.Color | Kinect.FrameSourceTypes.Depth | Kinect.FrameSourceTypes.Infrared);
        reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
        playersIdBreak = new List<ulong>();
        IDsBreak = new ulong[6];
        playersIdBreak.Clear();
    }

    private void Reader_MultiSourceFrameArrived(object sender, Kinect.MultiSourceFrameArrivedEventArgs e)
    {
        var reference = e.FrameReference.AcquireFrame();

        //Body
        using (var frame = reference.BodyFrameReference.AcquireFrame())
        {
            if (frame != null)
            {
                bodies = new Kinect.Body[frame.BodyFrameSource.BodyCount];  //BodyCount is always 6. I guess they have it on 6 because it is the maximum of the players that they can track per frame.
                frame.GetAndRefreshBodyData(bodies);
                var p = 0; //p counts the bodies so it counts the players
                for (int h = 0; h < IDsBreak.Length; h++)
                {
                    IDsBreak[h] = 0;
                }

                foreach (var body in bodies)
                {
                    foundId = false;
                    if (body != null)
                    {
                        if (body.IsTracked)
                        {
                            IDsBreak[p] = body.TrackingId;

                            if (playersIdBreak.Count == 0)
                            {
                                playersIdBreak.Add(body.TrackingId);
                            }

                            foreach (ulong id in playersIdBreak)
                            {
                                if (body.TrackingId == id)
                                {
                                    foundId = true;
                                }
                            }

                            if (foundId == false)
                            {
                                playersIdBreak.Add(body.TrackingId);
                            }
                        }
                    }
                    p++;
                }
            }
        }
    }


    void Update()
    {
        for (int i = 0; i < playersIdBreak.Count; i++)
        {
            bool playerFound = false;
            for (int j = 0; j < IDsBreak.Length; j++)
            {
                if (playersIdBreak[i] == IDsBreak[j])
                {
                    playerFound = true;
                }
            }

            if (playerFound == false)
            {
                playersIdBreak.RemoveAt(i);
            }
        }
    }


    public int CheckPlayersAfterGame()
    {
        if (playersIdBreak.Count == 0) //All the players left the game. Go to welcome screen/tutorial
        {
            TutorialPlayers.instance.timeToStartTemp = TutorialPlayers.instance.timeToStart;
            return 0; /////////Fixed case where tutorial scene is always scene 0.
        }
        else //They are some still remaining to play
        {
            TutorialPlayers.instance.timeToStartTemp = 0;

            if (playersIdBreak.Count == 1)
            {
                bool foundPlayer = false;
                for (int i = 0; i < TutorialPlayers.instance.playersPlayedTut.Count; i++)
                {
                    if (playersIdBreak[0] == TutorialPlayers.instance.playersPlayedTut[i])
                    {
                        foundPlayer = true;
                    }
                }

                if (foundPlayer) //P1 had already passed the tutorial
                {
                    return 1;
                }
                else //New player1
                {
                    return 0;   //Remaining P1 hasn't passed the tutorial. Needs to see it.
                }
            }
            else
            {
                bool[] foundPlayer = new bool[2];
                for (int j = 0; j < 2; j++) ////////Runs only for P1 and P2
                {
                    for (int i = 0; i < TutorialPlayers.instance.playersPlayedTut.Count; i++) //if only one finished the tutorial then they will see it again
                    {
                        if (playersIdBreak[j] == TutorialPlayers.instance.playersPlayedTut[i])
                        {
                            foundPlayer[j] = true;
                        }
                    }

                }

                if (foundPlayer[0] == true && foundPlayer[1] == true)    //Fixed case for two players. both they have passed the tutorial.
                {
                    return 1;
                }
                else
                {
                    return 0; //One of the two hasn't seen the tutorial.
                }
            }
        }
    }
}
