using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerManager : MonoBehaviour
{

    private NetworkController network;
    private string message;

    private void Start()
    {
        network = GetComponent<NetworkController>();
    }

    public void PressClicker()
    {
        message = "message from Clicker: " + Time.time;
        network.sendString(message);
        print(message);
    }
}
