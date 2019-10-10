using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class HL_Receive : MonoBehaviour
{
#if UNITY_EDITOR

    Thread receiveThread;
    UdpClient client;

    // public
    // public string IP = "127.0.0.1"; default local
    public int port = 11110; // define > init

    // infos
    public string lastReceivedUDPPacket = "";

    public GameObject hololens;
    Vector3 hololensPosition;
    Quaternion hololensRotation;
    public GameObject marker;
    Vector3 markerPosition;
    Quaternion markerRotation;

    public void Start()
    {
        init();
    }

    private void Update()
    {
        hololens.transform.position = hololensPosition;
        hololens.transform.rotation = hololensRotation;
        marker.transform.position = markerPosition;
        marker.transform.rotation = markerRotation;
    }

    private void OnApplicationQuit()
    {
        client.Close();
        receiveThread.Abort();
    }

    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -u 127.0.0.1 : " + port + " \n"
                    + "\nLast Packet: \n" + lastReceivedUDPPacket
                , style);
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
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                // Den abgerufenen Text anzeigen.
                //print(">> " + text);

                // latest UDPpacket
                lastReceivedUDPPacket = text;
                ParseData(lastReceivedUDPPacket);

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    void ParseData(string data)
    {
        string[] commaSeparatedValues = data.Split(',');
        hololensPosition = new Vector3
            (
            float.Parse(commaSeparatedValues[0]),
            float.Parse(commaSeparatedValues[1]),
            float.Parse(commaSeparatedValues[2])
            );
        hololensRotation = new Quaternion
            (
            float.Parse(commaSeparatedValues[3]),
            float.Parse(commaSeparatedValues[4]),
            float.Parse(commaSeparatedValues[5]),
            float.Parse(commaSeparatedValues[6])
            );
        markerPosition = new Vector3
            (
            float.Parse(commaSeparatedValues[7]),
            float.Parse(commaSeparatedValues[8]),
            float.Parse(commaSeparatedValues[9])
            );
        markerRotation = new Quaternion
            (
            float.Parse(commaSeparatedValues[10]),
            float.Parse(commaSeparatedValues[11]),
            float.Parse(commaSeparatedValues[12]),
            float.Parse(commaSeparatedValues[13])
            );
    }

#endif
}