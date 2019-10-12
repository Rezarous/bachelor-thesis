using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationController : MonoBehaviour
{

    public Slider x;
    public Slider y;
    public Slider z;
    public Slider q1;
    public Slider q2;
    public Slider q3;
    public Slider q4;

    public Text xText;
    public Text yText;
    public Text zText;
    public Text qXText;
    public Text qYText;
    public Text qZText;
    public Text qWText;

    private NetworkController network;
    private string message;
    private bool startsending;

    // Start is called before the first frame update
    void Start()
    {
        network = GetComponent<NetworkController>();
        StartCoroutine("SendCalibrationData");
    }

    // Update is called once per frame
    void Update()
    {
        xText.text = "X: " + x.value;
        yText.text = "Y: " + y.value;
        zText.text = "Z: " + z.value;
        qXText.text = "QX: " + q1.value;
        qYText.text = "QY: " + q2.value;
        qZText.text = "QZ: " + q3.value;
        qWText.text = "QW: " + q4.value;
    }

    public void SetSendingToTrue()
    {
        startsending = true;
        print(startsending);
    }

    public void SetSendingToFalse()
    {
        startsending = false;
        print(startsending);
    }

    public void ToggleSending()
    {
        startsending = !startsending;
        print(startsending);
    }

    IEnumerator SendCalibrationData()
    {
        yield return new WaitForSeconds(0.1f);
        print("trying to send: " + startsending);
        if (startsending)
        {
            message = x.value + "," + y.value + "," + z.value + "," + q1.value + "," + q2.value + "," + q3.value + "," + q4.value;
            network.sendString(message);
            print(message);
        }
        StartCoroutine("SendCalibrationData");
    }

}
