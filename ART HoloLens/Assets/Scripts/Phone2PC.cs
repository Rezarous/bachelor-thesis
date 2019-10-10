using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

public class Phone2PC : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port;
    public float[] receivedValues;

    public void Start()
    {
        receivedValues = new float[7];
        init();
    }

    private void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
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
                //ParseReceivedData(text);
                ParseMessage(text);

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    void ParseMessage(string message)
    {
        string[] numbersAsString = message.Split(',');

        for(int i=0; i< receivedValues.Length; i++)
        {
            receivedValues[i] = float.Parse(numbersAsString[i]);
        }
    }
}
