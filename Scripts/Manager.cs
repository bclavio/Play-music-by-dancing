using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager instance;


    Kinect.KinectSensor sensor;
    Kinect.MultiSourceFrameReader reader;
    IList<Kinect.Body> bodies;

    // ALL OF THESE VARIABLES THEY COULD BE IN A NEW CLASS AND IN THIS SCRIPT JUST CREATE TWO VARIABLES (P1,P2) WHICH WOULD INHERIT THESE VARIABLES BY CALLING THE CONSTRUCTOR!!!!!!!!!!!

    public float[,] jointsPos;
    public float[,] jointPosWorld; //Store the world position of every joint in a body. 3 columns for X, Y, Z
    public List<float[,]> bodyJoints;
    public List<float[,]> bodyJointsWorld; //Store the world position of every joint per player
    [HideInInspector]
    public List<float[]> playersJointsHeight; // Heights of the joints per player
    public List<float[,]> playersMinMaxHeight; // Min-Max height of joints per player
    public List<ulong> playersId;       //Remove the public. Did only for observation.
    public List<float[,]> playersMinMaxHeightInPixels;
    [HideInInspector]
    public bool[] initializeMidPos, zoneChanged;
    [HideInInspector]
    public ulong[] IDs;
    [HideInInspector]
    public float[] jointsHeight;       //With the order of the prefJoints. (heights of the joints in general) (the y variable nto the actual y in pixels)
    public float[,] minMaxHeight;
    public float[,] minMaxHeightInPixels;
    [HideInInspector]
    public List<float> spineMidHeightInPixels;   //The spineMid position of each player in pixels
    [HideInInspector]
    public List<float> spineMidPos, spineMidPosThreshold;
    [HideInInspector]
    public List<Texture> playersMidHighLowMat;     //Counts how many bodies are active at the same frame.
    [HideInInspector]
    public bool foundId, moveHands, ableToHigh5Left, ableToHigh5Right;
    [HideInInspector]
    public bool didHigh5, playTones, playTonesBody, playTonesBodyHands, clipEndedP1, clipEndedP2, tutorialRaisedHandsDown, p1MovedFrw, p1MovedBack, p2MovedFrw, p2MovedBack;
    public List<Texture> playersMat;
    public List<Texture> player1_2LineMat;
    public List<Texture> LowHighMats; //2 cells per player.
    public List<Texture> MidLowHighMats; ////2 cells per player.
    public List<Kinect.JointType> prefJoints;       //Assign in the inspector the pref joints you want to detect. ALWAYS first the main body.
    public int textureWidth, textureHeight, midTextureWidthP1, midTextureHeightP1, midTextureWidthP2, midTextureHeightP2, lineTextureWidth, lineTextureHeight, high5Distance;
    public float bodyDistanceThreshold, waitAtTheEnd;
    public GameObject player1, player2;
    public UserInput p1UInput, p2UInput;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
        playersId = new List<ulong>();
        playersId.Clear();
        bodyJoints = new List<float[,]>();
        bodyJointsWorld = new List<float[,]>();
        playersJointsHeight = new List<float[]>();
        playersMinMaxHeight = new List<float[,]>();
        playersMinMaxHeightInPixels = new List<float[,]>();
        spineMidHeightInPixels = new List<float>();
        spineMidPos = new List<float>();
        spineMidPosThreshold = new List<float>();
        playersMidHighLowMat = new List<Texture>();
        IDs = new ulong[6];
        initializeMidPos = new bool[6];  //maximum 6 players
        zoneChanged = new bool[6];  //maximum 6 players
        moveHands = false;
        ableToHigh5Left = true;
        ableToHigh5Right = true;
        didHigh5 = false;
        playTones = false;
        playTonesBody = false;
        playTonesBodyHands = false;
        clipEndedP1 = true;
        clipEndedP2 = true;
        p1MovedBack = false;
        p1MovedFrw = false;
        p2MovedBack = false;
        p2MovedFrw = false;
        tutorialRaisedHandsDown = false;

        p1UInput = player1.GetComponent<UserInput>();
        p2UInput = player2.GetComponent<UserInput>();

    }

    private void Reader_MultiSourceFrameArrived(object sender, Kinect.MultiSourceFrameArrivedEventArgs e)
    {
        var reference = e.FrameReference.AcquireFrame();

        //Body
        using (var frame = reference.BodyFrameReference.AcquireFrame())
        {
            if (frame != null)
            {
                bodies = new Kinect.Body[frame.BodyFrameSource.BodyCount];  //BodyCount is always 6.
                frame.GetAndRefreshBodyData(bodies);
                var p = 0; //p counts the bodies so it counts the players
                for (int h = 0; h < IDs.Length; h++)
                {
                    IDs[h] = 0;
                }

                foreach (var body in bodies)
                {
                    var i = 0;
                    jointsPos = new float[25, 2];
                    jointPosWorld = new float[prefJoints.Count, 3];
                    jointsHeight = new float[prefJoints.Count];
                    minMaxHeight = new float[prefJoints.Count, 2];
                    minMaxHeightInPixels = new float[prefJoints.Count, 2];
                    for (int j = 0; j < prefJoints.Count; j++)
                    {
                        minMaxHeight[j, 0] = 1;
                        minMaxHeight[j, 1] = -1;
                        minMaxHeightInPixels[j, 0] = 1080;
                        minMaxHeightInPixels[j, 1] = 1080;
                    }

                    if (body != null)
                    {
                        if (body.IsTracked)
                        {
                            IDs[p] = body.TrackingId;

                            if (playersId.Count == 0)
                            {
                                playersId.Add(body.TrackingId);
                                spineMidHeightInPixels.Add(0);
                                spineMidPos.Add(0);
                                spineMidPosThreshold.Add(0);
                                bodyJoints.Add(null);
                                bodyJointsWorld.Add(null);
                                playersJointsHeight.Add(null);
                                playersMidHighLowMat.Add(null);
                                playersMinMaxHeight.Add(minMaxHeight);
                                playersMinMaxHeightInPixels.Add(minMaxHeightInPixels);
                            }
                            else
                            {
                                foundId = false;

                                foreach (ulong id in playersId)
                                {
                                    if (body.TrackingId == id)
                                    {
                                        foundId = true;
                                    }
                                }

                                if (foundId == false)
                                {
                                    playersId.Add(body.TrackingId);
                                    spineMidHeightInPixels.Add(0);
                                    spineMidPos.Add(0);
                                    spineMidPosThreshold.Add(0);
                                    bodyJoints.Add(null);
                                    bodyJointsWorld.Add(null);
                                    playersJointsHeight.Add(null);
                                    playersMidHighLowMat.Add(null);
                                    playersMinMaxHeight.Add(minMaxHeight);
                                    playersMinMaxHeightInPixels.Add(minMaxHeightInPixels);
                                }
                            }

                            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
                            {

                                //3d coordinates in meters
                                Kinect.CameraSpacePoint skeletonPoint = body.Joints[jt].Position;

                                Kinect.ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(skeletonPoint);

                                var k = 0;  //k counts the which joint in the jointHeight matrix corresponding to the amount of the prefJoints
                                foreach (Kinect.JointType joint in prefJoints)
                                {
                                    if (jt == joint)
                                    {
                                        jointsHeight[k] = body.Joints[jt].Position.Y;
                                        jointPosWorld[k, 0] = skeletonPoint.X;
                                        jointPosWorld[k, 1] = skeletonPoint.Y;
                                        jointPosWorld[k, 2] = skeletonPoint.Z;
                                        for (int j = 0; j < playersId.Count; j++)       //j counts whos is the player
                                        {
                                            if (body.TrackingId == playersId[j])
                                            {
                                                if (k == 0)
                                                {
                                                    spineMidHeightInPixels[j] = colorPoint.Y;
                                                    if (initializeMidPos[j] == false)
                                                    {
                                                        spineMidPos[j] = Mathf.Floor(body.Joints[jt].Position.Z); //spineMidPos is used for the zone control with the integer
                                                        spineMidPosThreshold[j] = body.Joints[jt].Position.Z;
                                                        initializeMidPos[j] = true;
                                                    }
                                                    else
                                                    {
                                                        if (body.Joints[jt].Position.Z > spineMidPosThreshold[j] + bodyDistanceThreshold || body.Joints[jt].Position.Z < spineMidPosThreshold[j] - bodyDistanceThreshold)
                                                        {
                                                            //Recalibration time
                                                            for (int l = 1; l < prefJoints.Count; l++)
                                                            {
                                                                playersMinMaxHeight[j][l, 0] = 1;
                                                                playersMinMaxHeight[j][l, 1] = -1;
                                                                playersMinMaxHeightInPixels[j][l, 0] = 1080;
                                                                playersMinMaxHeightInPixels[j][l, 1] = 1080;
                                                            }

                                                            zoneChanged[j] = true;
                                                            if (body.Joints[jt].Position.Z > spineMidPosThreshold[j] + bodyDistanceThreshold)
                                                            {
                                                                //further away from kinect (Low)
                                                                if (j == 0)
                                                                {
                                                                    p1UInput.userInput = p1UInput.targetBackInput;
                                                                    p1MovedBack = true;
                                                                    playersMidHighLowMat[j] = MidLowHighMats[0];
                                                                    clipEndedP1 = false;
                                                                }
                                                                else if (j == 1)
                                                                {
                                                                    p2UInput.userInput = p2UInput.targetBackInput;
                                                                    p2MovedBack = true;
                                                                    playersMidHighLowMat[j] = MidLowHighMats[2];
                                                                    clipEndedP2 = false;
                                                                }
                                                            }
                                                            else if (body.Joints[jt].Position.Z < spineMidPosThreshold[j] - bodyDistanceThreshold)
                                                            {
                                                                //closer to kinect (High)
                                                                if (j == 0)
                                                                {
                                                                    p1UInput.userInput = p1UInput.targetForwardInput;
                                                                    p1MovedFrw = true;
                                                                    playersMidHighLowMat[j] = MidLowHighMats[1];
                                                                    clipEndedP1 = false;
                                                                }
                                                                else if (j == 1)
                                                                {
                                                                    p2UInput.userInput = p2UInput.targetForwardInput;
                                                                    p2MovedFrw = true;
                                                                    playersMidHighLowMat[j] = MidLowHighMats[3];
                                                                    clipEndedP2 = false;
                                                                }
                                                            }

                                                            if (clipEndedP1 == false && clipEndedP2 == false)
                                                            {
                                                                playTonesBody = true;
                                                            }

                                                            spineMidPosThreshold[j] = body.Joints[jt].Position.Z;
                                                        }
                                                    }
                                                }
                                                SupportFunctions.instance.SortMinMax(body.Joints[jt].Position.Y, j, k);
                                                SupportFunctions.instance.SortLine(colorPoint.Y, j, k);
                                            }

                                        }
                                    }
                                    k++;
                                }


                                jointsPos[i, 0] = colorPoint.X;
                                jointsPos[i, 1] = colorPoint.Y;
                                i++;

                                if (jt == Kinect.JointType.ThumbRight)
                                {
                                    for (int j = 0; j < playersId.Count; j++)
                                    {
                                        if (body.TrackingId == playersId[j])
                                        {
                                            bodyJoints[j] = jointsPos;
                                            bodyJointsWorld[j] = jointPosWorld;
                                            playersJointsHeight[j] = jointsHeight;
                                        }
                                    }
                                }
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
        SupportFunctions.instance.CheckActivePlayers();
        SupportFunctions.instance.AddRowToData();
        SupportFunctions.instance.CheckHighFive();
        SupportFunctions.instance.CheckHandnBodyForNotes();
        SupportFunctions.instance.TimeBeingActive();
        SupportFunctions.instance.TimeSpentAlone();
        SupportFunctions.instance.PlayAlone();
        SupportFunctions.instance.HowPlayTogether();
        SupportFunctions.instance.CheckHowPlayMusic();
        if (playersId.Count < 2)
        {
            playTones = false;
            playTonesBody = false;
            playTonesBodyHands = false;
        }
    }

    void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
        {
            if (playersId.Count != 0)
            {
                for (int k = 0; k < playersId.Count; k++)       // K counts players
                {
                    var i = -1;     //i Counts the joints. All of them. 25 in sum

                    for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
                    {
                        i++;
                        var p = 0;      //p counts the pref joints we chose.
                        foreach (Kinect.JointType joint in prefJoints)
                        {
                            if (jt == joint)
                            {
                                try
                                {
                                    if (bodyJoints[k][i, 0] != 0 && bodyJoints[k][i, 1] != 0)
                                    {
                                        if (jt == prefJoints[0]) //the base bodyjoint like spinemid needs to be at the first place ALWAYS!!!!!!
                                        {
                                            if (zoneChanged[k] == false && k < 2)
                                            {
                                                if (k == 0)
                                                {
                                                    Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - midTextureWidthP1 / 2, bodyJoints[k][i, 1] - midTextureHeightP1 / 2, midTextureWidthP1, midTextureHeightP1), playersMat[k]);
                                                }
                                                else
                                                {
                                                    Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - midTextureWidthP2 / 2, bodyJoints[k][i, 1] - midTextureHeightP2 / 2, midTextureWidthP2, midTextureHeightP2), playersMat[k]);
                                                }
                                            }
                                            else if (zoneChanged[k] == true && k < 2)
                                            {
                                                if (clipEndedP1 == true)
                                                {
                                                    if (p1MovedBack == true)
                                                    {
                                                        p1MovedBack = false;
                                                    }
                                                    else if (p1MovedFrw == true)
                                                    {
                                                        p1MovedFrw = false;
                                                    }

                                                    if (k == 0)
                                                    {
                                                        Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - midTextureWidthP1 / 2, bodyJoints[k][i, 1] - midTextureHeightP1 / 2, midTextureWidthP1, midTextureHeightP1), playersMat[k]);
                                                    }
                                                }
                                                else
                                                {
                                                    if (k == 0)
                                                    {
                                                        if (playersMidHighLowMat[k] != null)
                                                        {
                                                            Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - midTextureWidthP1 / 2, bodyJoints[k][i, 1] - midTextureHeightP1 / 2, midTextureWidthP1, midTextureHeightP1), playersMidHighLowMat[k]);
                                                        }
                                                    }
                                                }

                                                if (clipEndedP2 == true)
                                                {
                                                    if (p2MovedBack == true)
                                                    {
                                                        p2MovedBack = false;
                                                    }
                                                    else if (p2MovedFrw == true)
                                                    {
                                                        p2MovedFrw = false;
                                                    }

                                                    if (k == 1)
                                                    {
                                                        Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - midTextureWidthP2 / 2, bodyJoints[k][i, 1] - midTextureHeightP2 / 2, midTextureWidthP2, midTextureHeightP2), playersMat[k]);
                                                    }
                                                }
                                                else
                                                {
                                                    if (k == 1)
                                                    {
                                                        if (playersMidHighLowMat[k] != null)
                                                        {
                                                            Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - midTextureWidthP2 / 2, bodyJoints[k][i, 1] - midTextureHeightP2 / 2, midTextureWidthP2, midTextureHeightP2), playersMidHighLowMat[k]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            if (playersJointsHeight[k][p] > playersJointsHeight[k][0] && k < 2)         //K will be restricted to 2 for only two players.
                                            {
                                                moveHands = true;

                                                var range = playersMinMaxHeightInPixels[k][p, 0] - playersMinMaxHeightInPixels[k][p, 1];
                                                var distribution = range / 3;

                                                Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - lineTextureWidth / 2, (playersMinMaxHeightInPixels[k][p, 0] - (2 * distribution)) - lineTextureHeight / 2, lineTextureWidth, lineTextureHeight), player1_2LineMat[k]);

                                                var range2 = playersMinMaxHeight[k][p, 1] - playersMinMaxHeight[k][p, 0];
                                                var distribution2 = range2 / 3;

                                                if (playersJointsHeight[k][p] > (playersMinMaxHeight[k][p, 0] + (2 * distribution2)))
                                                {
                                                    ////////////////////////////////////  HERE IS THE HIGH INPUT////////////////////////////////////

                                                    if (k == 0)
                                                    {
                                                        p1UInput.userInput = "high";

                                                    }
                                                    else if (k == 1)
                                                    {
                                                        p2UInput.userInput = "high";
                                                    }

                                                    switch (k)
                                                    {
                                                        case 0:
                                                            Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - textureWidth / 2, bodyJoints[k][i, 1] - textureHeight / 2, textureWidth, textureHeight), LowHighMats[1]);
                                                            break;
                                                        case 1:
                                                            Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - textureWidth / 2, bodyJoints[k][i, 1] - textureHeight / 2, textureWidth, textureHeight), LowHighMats[3]);
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    ////////////////////////////////////  HERE IS THE LOW INPUT////////////////////////////////////

                                                    if (k == 0)
                                                    {
                                                        p1UInput.userInput = "low";
                                                    }
                                                    else if (k == 1)
                                                    {
                                                        p2UInput.userInput = "low";
                                                    }

                                                    switch (k)
                                                    {
                                                        case 0:
                                                            Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - textureWidth / 2, bodyJoints[k][i, 1] - textureHeight / 2, textureWidth, textureHeight), LowHighMats[0]);
                                                            break;
                                                        case 1:
                                                            Graphics.DrawTexture(new Rect(bodyJoints[k][i, 0] - textureWidth / 2, bodyJoints[k][i, 1] - textureHeight / 2, textureWidth, textureHeight), LowHighMats[2]);
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    //print("Something wrong with the bodyJoints of someone player that is being registered into Kinect");
                                }
                            }
                            p++;
                        }
                    }

                }
            }
        }
    }
}

