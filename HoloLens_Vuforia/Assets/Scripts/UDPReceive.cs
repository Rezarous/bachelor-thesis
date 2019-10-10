using UnityEngine;
using System.Text;
using System.Linq;
using System;
using System.IO;

#if !WINDOWS_UWP
using System.Net;
using System.Net.Sockets;
using System.Threading;


#else
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;
#endif

public class UDPReceive : MonoBehaviour
{

    public static string IPAddress = "192.168.1.140";
    public static int port = 11110;

    private bool initialized = false;
    private bool connected = false;

    public TextMesh textMesh;

    public GameObject cam;
    public GameObject marker;

    string data = "";

#if WINDOWS_UWP

    private bool receivedSomething;
    private string receivedMessage;

    private DatagramSocket socket;
    
    private int UDPPingReplyLength = Encoding.UTF8.GetBytes("UDPPingReply").Length + 4;

    void initUDPReceiver() {
        Debug.Log("Waiting for a connection...");

        socket = new DatagramSocket();
        socket.MessageReceived += Socket_MessageReceived;
        HostNameType hostNameType = HostNameType.Ipv4;
        HostName IP = null;
        try {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            IP = Windows.Networking.Connectivity.NetworkInformation.GetHostNames()
             .FirstOrDefault(
                    hn =>
                        hn.Type == hostNameType &&
                        hn.IPInformation?.NetworkAdapter != null &&
                        hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);

            _ = socket.BindEndpointAsync(IP, port.ToString());

            initialized = true;
            Debug.Log("Hololens IP is " + IP.CanonicalName);
            textMesh.color = Color.green;
        }
        catch(Exception e) {
            Debug.Log("Hier gecrasht");
            Debug.Log(e.ToString());
            Debug.Log(SocketError.GetStatus(e.HResult).ToString());
            textMesh.color = Color.red;
            return;
        }

        Debug.Log("exit start");
    }

    public async void sendUDPMessage(byte[] message) {
        try {
            Windows.Networking.HostName hnip = new Windows.Networking.HostName(IPAddress);
            Debug.Log("Send message to IPAddress " + hnip.DisplayName + " on Port " + port.ToString());
            using(var stream = await socket.GetOutputStreamAsync(hnip, port.ToString())) {
                using(var writer = new Windows.Storage.Streams.DataWriter(stream)) {
                    writer.WriteBytes(message);
                    await writer.StoreAsync();
                }
            }
        } catch (Exception e)
        {
            Debug.Log("cant send from Hololens");
        }
    }



    private async void Socket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender,
    Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args) {
        try
        {
            connected = true;
            Debug.Log("message received");
            Stream streamIn = args.GetDataStream().AsStreamForRead();
            Debug.Log("1");
            byte[] byteLength = new byte[4];
            Debug.Log("2");
            await streamIn.ReadAsync(byteLength, 0, 4);
            Debug.Log("3");
            //int length = BitConverter.ToInt32(byteLength, 0);
            int length = 2048;
            Debug.Log("Length is: " + length);
            Debug.Log("4");
            byte[] messageBytes = new byte[length];
            Debug.Log("5");
            System.Buffer.BlockCopy(byteLength, 0, messageBytes, 0, 4);
            Debug.Log("6");
            await streamIn.ReadAsync(messageBytes, 4, length - 4);
            Debug.Log("7");
            //Debug.Log(Encoding.UTF8.GetString(messageBytes, sizeof(int), length - 4));
            Debug.Log(Encoding.UTF8.GetString(messageBytes, 0, length - 4));
            Debug.Log("8");

            Debug.Log("Going into void.");
            PrintUDP(messageBytes, length);

        } catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void PrintUDP(byte[] messageBytes, int length)
    {
        Debug.Log("In void!");
        receivedMessage = Encoding.UTF8.GetString(messageBytes, 0, length - 4);
        Debug.Log("Message: " + receivedMessage);
        receivedSomething = true;
    }

    public void Start() {
        textMesh.color = Color.cyan;
        initUDPReceiver();
    }

    void SendTransformationData(){
        data =
                cam.transform.position.x + "," +
                cam.transform.position.y + "," +
                cam.transform.position.z + "," +
                cam.transform.rotation.x + "," +
                cam.transform.rotation.y + "," +
                cam.transform.rotation.z + "," +
                cam.transform.rotation.w + "," +
                marker.transform.position.x + "," +
                marker.transform.position.y + "," +
                marker.transform.position.z + "," +
                marker.transform.rotation.x + "," +
                marker.transform.rotation.y + "," +
                marker.transform.rotation.z + "," +
                marker.transform.rotation.w;

        sendUDPMessage(Encoding.UTF8.GetBytes(data));
    }
    
    void Update() {
        //if (connected == false)
        //    initUDPReceiver();

        // SendTransformationData();
        
        if (receivedSomething)
        {
            textMesh.color = Color.yellow;
            textMesh.text = receivedMessage;

            receivedSomething = false;
        }
    }


#endif

}