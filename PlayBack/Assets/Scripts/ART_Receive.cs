using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

public class ART_Receive : MonoBehaviour
{
    // Global Control
    public int expectedNumberOfMarkers;
    private bool correctNumberOfMarkersDetected;
    public GameObject warningPlane;

    //Networking
    Thread receiveThread;
    UdpClient client;
    public int port; // define > editor
    //public bool firstDataReceived = false;


    // Data
    public GameObject firstMarker;
    public Vector3 finalPosition = new Vector3(0, 0, 0);
    public Quaternion finalOrientation;
    private Matrix4x4 receivedRotationMatrix = new Matrix4x4();

    
    //public Vector3 firstPosition;
    //public Quaternion firstOrientation;

    public GameObject vuforia;
    private Vector3 vuforiaPosition;
    private Matrix4x4 vuforiaRotationMatrix = new Matrix4x4();
    private Quaternion vuforiaRotation;

    public GameObject secondMarker;
    private Vector3 secondMarkerPosition;
    private Matrix4x4 secondMarkerRotationMatrix = new Matrix4x4();
    private Quaternion secondMarkerRotation;

    

    public void Start()
    {
        init();
    }

    Vector3 tempFirstMarkerPositionSignal;
    Vector3 tempSecondMarkerPositionSignal;
    int signalIndex = 0;
    public int numberOfSamples;

    public Vector3 finalFirstMarkerPositionSignal;
    public Vector3 finalSecondMarkerPositionSignal;

    private void Update()
    {
        if (correctNumberOfMarkersDetected)
        {
            warningPlane.SetActive(false);
        }
        else
        {
            warningPlane.SetActive(true);
        }

        MeanAverageSignal(numberOfSamples);
        //firstMarker.transform.position = finalPosition;
        firstMarker.transform.position = finalFirstMarkerPositionSignal;
        //firstMarker.transform.rotation = finalOrientation;

        //vuforia.transform.position = vuforiaPosition;
        vuforia.transform.position = finalSecondMarkerPositionSignal;
        //vuforia.transform.rotation = vuforiaRotation;

        secondMarker.transform.position = secondMarkerPosition;
        secondMarker.transform.rotation = secondMarkerRotation;
    }

    

    void MeanAverageSignal(int num)
    {
        tempFirstMarkerPositionSignal += finalPosition;
        tempSecondMarkerPositionSignal += vuforiaPosition;
        if (signalIndex % num == 0)
        {
            //print("apply mean average");
            finalFirstMarkerPositionSignal = tempFirstMarkerPositionSignal / num;
            firstMarker.transform.rotation = finalOrientation;

            finalSecondMarkerPositionSignal = tempSecondMarkerPositionSignal / num;
            vuforia.transform.rotation = vuforiaRotation;

            tempFirstMarkerPositionSignal = Vector3.zero;
            tempSecondMarkerPositionSignal = Vector3.zero;
            signalIndex = 0;
        }
        signalIndex++;
    }

    private void OnApplicationQuit()
    {
        //client.Close();
        receiveThread.Abort();
    }

    // init
    private void init()
    {
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);
                //print(text);
                ParseReceivedData(text);

                //if (!firstDataReceived)
                //{
                //    firstPosition = finalPosition;
                //    firstOrientation = finalOrientation;
                //    firstDataReceived = true;
                //}

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    private void ParseReceivedData(string text)
    {
        string[] lines = text.Split('\n');
        string separatedValues6D = lines[2].Split('[')[2];
        string[] values6D = separatedValues6D.Remove(separatedValues6D.Length - 1).Split(' ');

        Vector3 pos = new Vector3(float.Parse(values6D[0]), float.Parse(values6D[1]), float.Parse(values6D[2]));

        string separatedValues9D = lines[2].Split('[')[3];
        separatedValues9D = separatedValues9D.Remove(separatedValues9D.Length - 3, 3);
        string[] values9D = separatedValues9D.Split(' ');

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                receivedRotationMatrix[i, j] = float.Parse(values9D[(3 * j) + i]);
            }
        }
        receivedRotationMatrix[3, 3] = 1f;
        //print(receivedRotationMatrix);

        finalOrientation = ConvertCoordinateSystem(QuaternionFromMatrix(receivedRotationMatrix));
        finalPosition = ScaleAndSwapVector(pos);

        if (int.Parse(lines[2][3].ToString()) == expectedNumberOfMarkers)
        {

            correctNumberOfMarkersDetected = true;

            if (lines[2][3] == '2')
            {
                HandleTheFirstMarker(lines);
            }

            if (lines[2][3] == '3')
            {
                HandleTheFirstMarker(lines);
                HandleTheSecondMarker(lines);
            }
        }
        else
        {
            correctNumberOfMarkersDetected = false;
        }
    }

    void HandleTheFirstMarker(string[] lines)
    {
        string vuforia6d = lines[2].Split('[')[5];
        string[] v6D = vuforia6d.Remove(vuforia6d.Length - 1).Split(' ');
        Vector3 tempPos = new Vector3(float.Parse(v6D[0]), float.Parse(v6D[1]), float.Parse(v6D[2]));
        vuforiaPosition = ScaleAndSwapVector(tempPos);

        string vuforia9d = lines[2].Split('[')[6];
        vuforia9d = vuforia9d.Remove(vuforia9d.Length - 3, 3);
        string[] v9D = vuforia9d.Split(' ');
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                vuforiaRotationMatrix[i, j] = float.Parse(v9D[(3 * j) + i]);
            }
        }
        vuforiaRotationMatrix[3, 3] = 1f;
        vuforiaRotation = ConvertCoordinateSystem(QuaternionFromMatrix(vuforiaRotationMatrix));
    }

    void HandleTheSecondMarker(string[] lines)
    {
        string vuforia6d = lines[2].Split('[')[8];
        string[] v6D = vuforia6d.Remove(vuforia6d.Length - 1).Split(' ');
        Vector3 tempPos = new Vector3(float.Parse(v6D[0]), float.Parse(v6D[1]), float.Parse(v6D[2]));
        secondMarkerPosition = ScaleAndSwapVector(tempPos);

        string vuforia9d = lines[2].Split('[')[9];
        vuforia9d = vuforia9d.Remove(vuforia9d.Length - 3, 3);
        string[] v9D = vuforia9d.Split(' ');
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                secondMarkerRotationMatrix[i, j] = float.Parse(v9D[(3 * j) + i]);
            }
        }
        secondMarkerRotationMatrix[3, 3] = 1f;
        secondMarkerRotation = ConvertCoordinateSystem(QuaternionFromMatrix(secondMarkerRotationMatrix));

        // print("3rd Position: " + secondMarkerPosition);
        // print("3rd Rotation: " + secondMarkerRotation);
    }

    private Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        Vector3 forward = new Vector3(m.GetColumn(2).x, m.GetColumn(2).y, m.GetColumn(2).z);
        Vector3 upwards = new Vector3(m.GetColumn(1).x, m.GetColumn(1).y, m.GetColumn(1).z);
        return Quaternion.LookRotation(forward, upwards);
    }



    private Quaternion ConvertCoordinateSystem(Quaternion rightHandedQuaternion)
    {
        return new Quaternion(-rightHandedQuaternion.x,
            -rightHandedQuaternion.z,
            -rightHandedQuaternion.y,
            rightHandedQuaternion.w);
    }

    private Vector3 ScaleAndSwapVector(Vector3 vectorIn)
    {
        float x = vectorIn.x;
        float y = vectorIn.z;
        float z = vectorIn.y;
        vectorIn = new Vector3(x, y, z);
        return vectorIn / 1000;
    }
}
