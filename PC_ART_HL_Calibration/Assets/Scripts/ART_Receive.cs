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
    // Data
    public GameObject firstMarker;
    public Vector3 finalPosition = new Vector3(0, 0, 0);
    public Quaternion finalOrientation;
    private Matrix4x4 receivedRotationMatrix = new Matrix4x4();

    public bool firstDataReceived = false;
    public Vector3 firstPosition;
    public Quaternion firstOrientation;

    public GameObject vuforia;
    private Vector3 vuforiaPosition;
    private Matrix4x4 vuforiaRotationMatrix = new Matrix4x4();
    private Quaternion vuforiaRotation;

    Thread receiveThread;
    UdpClient client;
    public int port; // define > editor

    public void Start()
    {
        init();
    }

    private void Update()
    {
        firstMarker.transform.position = finalPosition;
        firstMarker.transform.rotation = finalOrientation;
        vuforia.transform.position = vuforiaPosition;
        vuforia.transform.rotation = vuforiaRotation;
    }

    private void OnApplicationQuit()
    {
        client.Close();
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

                ParseReceivedData(text);

                if (!firstDataReceived)
                {
                    firstPosition = finalPosition;
                    firstOrientation = finalOrientation;
                    firstDataReceived = true;
                }

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

        //print("1: " + separatedValues9D);

        print("" + 1);

        //Vuforia
        if (lines[2][3] == '2')
        {
            print("" + 2);
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

            //print("2: " + vuforia9d);
        }
        print("" + 10);
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
