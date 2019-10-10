using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDP_Sender : MonoBehaviour
{
#if UNITY_EDITOR
    private static int localPort;

    //HoloLens
    public string targetIP;
    public int targetPort = 11110;

    public string message;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    void Start()
    {
        print("Max Value: " + System.Int32.MaxValue);
        print("The code: " + BitConverter.ToInt32(new byte[4], 0));
        print("Size of int: " + sizeof(int));
        init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("clicked");
            sendString(message);
        }
    }

    public void init()
    {
        print("UDPSend.init()");

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
        client = new UdpClient();

        print("Sending to " + targetIP + " : " + targetPort);
        print("Testing: nc -lu " + targetIP + " : " + targetPort);

    }

    private void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
            print("sent");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
#endif
}

