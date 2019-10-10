using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This class determines if the user is moving or not
public class CameraStateDetector : MonoBehaviour {

    public bool cameraIsStatic = false;
    public float speedThreshold;
    public int waitingNumberOfFrames;
    public Image movementIndicator;
    public Material redMaterial;
    public Material greenMaterial;

    private int waitedFrames;
    private Vector3 previousPosition;
    private float positionalDistance;

	// Use this for initialization
	void Start () {
        previousPosition = transform.position;
        waitedFrames = 0;
    }
	
	// Update is called once per frame
	void Update () {

        positionalDistance = Vector3.Distance(previousPosition, transform.position);

        if (waitedFrames == 0)
        {
            if (positionalDistance > speedThreshold)
            {
                setMovementState();
            }
            else
            {
                setStaticState();
            }
        }
        else
        {
            if (waitedFrames < waitingNumberOfFrames)
            {
                waitedFrames++;
            }

            if (waitedFrames == waitingNumberOfFrames)
            {
                waitedFrames = 0;
            }
        }

        previousPosition = transform.position;
    }


    void setStaticState() // user is not moving ==> Green
    {
        cameraIsStatic = true;
        //cubeMovementIndicator.GetComponent<Renderer>().material = greenMaterial;
        movementIndicator.color = Color.green;
        waitedFrames = 0;
    }

    void setMovementState() // user is moving ==> Red
    {
        cameraIsStatic = false;
        //cubeMovementIndicator.GetComponent<Renderer>().material = redMaterial;
        movementIndicator.color = Color.red;
        waitedFrames++;
    }
}
