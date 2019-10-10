using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

public class MultiMarkerReceiver : MonoBehaviour {

    // Global Control
    public int expectedNumberOfMarkers;
    private bool correctNumberOfMarkersDetected;
    public GameObject warningPlane;

    //Networking
    Thread receiveThread;
    UdpClient client;
    public int port; // define > editor
    public bool firstDataReceived = false;

    //Markers
    public GameObject[] markers;
    public Vector3[] markerPositions;
    public Quaternion[] markerRotations;
    private Matrix4x4[] receivedRotationMatrices;

    //Update
    int signalIndex = 0;
    public int numberOfSamples;
    Vector3[] temporaryMarkerPositions;
    public Vector3[] finalMarkerPositions;
    public Quaternion[] finalMarkerRotations;

    void Start ()
    {
        temporaryMarkerPositions = new Vector3[expectedNumberOfMarkers];
        markerPositions = new Vector3[expectedNumberOfMarkers];
        finalMarkerPositions = new Vector3[expectedNumberOfMarkers];

        markerRotations = new Quaternion[expectedNumberOfMarkers];
        finalMarkerRotations = new Quaternion[expectedNumberOfMarkers];

        receivedRotationMatrices = new Matrix4x4[expectedNumberOfMarkers];
        Init();
    }

    private void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }

    private void Init()
    {
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

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

        if((int.Parse(lines[2].Split('[')[1][0].ToString()) == 0) && (int.Parse(lines[2].Split('[')[4][0].ToString()) == 1))
        {
            correctNumberOfMarkersDetected = true;
            for (int i = 0; i < int.Parse(lines[2][3].ToString()); i++)
            {
                FindTheRightMarkerToAssign(i, lines, i);
            }
        } else
        {
            correctNumberOfMarkersDetected = false;
        }
    }

    void FindTheRightMarkerToAssign(int dataIndex, string[] lines, int markerIndex)
    {
        if (int.Parse(lines[2].Split('[')[dataIndex * 3 + 1][0].ToString()) == markerIndex)
        {
            AssignValuesToMaker(dataIndex, lines, markerIndex);
        }
        else
        {
            markerIndex++;
            FindTheRightMarkerToAssign(dataIndex, lines, markerIndex);
        }
        //print("Position: " + index + ": " + markerPositions[index]);
        //print("Rotation: " + index + ": " + markerRotations[index]);
    }

    void AssignValuesToMaker(int index, string[] lines, int markerIndex)
    {
        string separatedValues6D = lines[2].Split('[')[index * 3 + 2];
        string[] v6D = separatedValues6D.Remove(separatedValues6D.Length - 1).Split(' ');
        Vector3 tempPos = new Vector3(float.Parse(v6D[0]), float.Parse(v6D[1]), float.Parse(v6D[2]));
        markerPositions[markerIndex] = ScaleAndSwapVector(tempPos);

        string separatedValues9D = lines[2].Split('[')[index * 3 + 3];
        separatedValues9D = separatedValues9D.Remove(separatedValues9D.Length - 3, 3);
        string[] v9D = separatedValues9D.Split(' ');
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                receivedRotationMatrices[markerIndex][i, j] = float.Parse(v9D[(3 * j) + i]);
            }
        }
        receivedRotationMatrices[markerIndex][3, 3] = 1f;
        markerRotations[markerIndex] = ConvertCoordinateSystem(QuaternionFromMatrix(receivedRotationMatrices[markerIndex]));
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

    void Update()
    {
        if (correctNumberOfMarkersDetected) warningPlane.SetActive(false); else warningPlane.SetActive(true);
        MeanAverageSignal(numberOfSamples);
        for (int i = 0; i < expectedNumberOfMarkers; i++)
        {
            markers[i].transform.position = markerPositions[i];
            markers[i].transform.rotation = markerRotations[i];
        }
    }

    void MeanAverageSignal(int num)
    {

        for (int i = 0; i < expectedNumberOfMarkers; i++)
        {
            temporaryMarkerPositions[i] += markerPositions[i];
            if (signalIndex % num == 0)
            {
                finalMarkerPositions[i] = temporaryMarkerPositions[i] / num;
                finalMarkerRotations[i] = markerRotations[i];
                temporaryMarkerPositions[i] = Vector3.zero;
            }
        }
        signalIndex++;
    }
}
