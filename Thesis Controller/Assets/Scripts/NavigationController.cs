using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{
    public GameObject main;
    public GameObject calibration;
    public GameObject clicker;

    public void LoadCalibrationScene()
    {
        main.SetActive(false);
        clicker.SetActive(false);
        calibration.SetActive(true);
    }

    public void LoadClickerScene()
    {
        main.SetActive(false);
        calibration.SetActive(false);
        clicker.SetActive(true);
    }

    public void LoadHomeScene()
    {
        clicker.SetActive(false);
        calibration.SetActive(false);
        main.SetActive(true);
        GetComponent<CalibrationController>().SetSendingToFalse();
    }
}
