using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour
{

    private string IP;
    private string port;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    public InputField ipText;
    public InputField portText;
    public Image statusBar;

    private static bool connectionIsEstablished;

    private void Start()
    {
        statusBar.color = new Color(217 / 255f, 56f / 255f, 56f / 255f);
    }

    private void Update()
    {
        if (connectionIsEstablished)
        {
            statusBar.color = new Color(77 / 255f, 217 / 255f, 56 / 255f);
        }
        else
        {
            statusBar.color = new Color(217 / 255f, 56 / 255f, 56 / 255f);
        }
    }

    public void EstablishConnection()
    {

        IP = ipText.text;
        port = portText.text;

        print(IP + ", " + port);

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), int.Parse(port));
        client = new UdpClient();

        connectionIsEstablished = true;

    }


    public void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

}
