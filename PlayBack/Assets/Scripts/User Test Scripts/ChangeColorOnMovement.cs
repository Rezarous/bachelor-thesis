using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOnMovement : MonoBehaviour
{

    public PlayBack playBackManager;
    public Vector3 previousPosition;
    public static int stateController = 20;
    public int numberOfUserAdjustments;


    private bool objectMoved;
    private enum objectStates { hasNotMoved, isMoving, isPlaced };
    private objectStates state;

    private Renderer thisRenderer;

    private float beginTime;
    private float endTime;
    public float timeForPlacement;

    void Start()
    {
        state = objectStates.hasNotMoved;
        numberOfUserAdjustments = -1;
        thisRenderer = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        //thisRenderer.material = playBackManager.whiteMat;
        //thisRenderer.material = playBackManager.redMat;
        thisRenderer.enabled = false;
    }

    void LateUpdate()
    {
        CheckState();
        ChangeColor();
    }


    void IncrementTime()
    {
        timeForPlacement += endTime - beginTime;
    }

    void CheckState()
    {
        if (Vector3.Distance(previousPosition, transform.position) > playBackManager.colorThreshold)
        {
            if (state != objectStates.isMoving)
            {
                beginTime = playBackManager.timeStamp;
            }
            state = objectStates.isMoving;
        }
        else if (state == objectStates.isMoving)
        {
            if (state == objectStates.isMoving)
            {
                endTime = playBackManager.timeStamp;
                IncrementTime();
            }
            state = objectStates.isPlaced;
            numberOfUserAdjustments++;
        }
        if (Vector3.Distance(previousPosition, transform.position) > 0.5)
        {
            //thisRenderer.enabled = !thisRenderer.enabled;
            thisRenderer.enabled = true;
        }
        //else
        //{
        //    if (Vector3.Distance(previousPosition, transform.position) > playBackManager.colorThreshold)
        //    {
        //        //print("isMoving: " + Vector3.Distance(previousPosition, transform.position));
        //        state = objectStates.isMoving;
        //    }
        //    else if (state == objectStates.isMoving)
        //    {
        //        //print("isPlaced: " + Vector3.Distance(previousPosition, transform.position));
        //        state = objectStates.isPlaced;
        //        numberOfUserAdjustments++;
        //    }
        //}

    }

    void ChangeColor()
    {
        //if (state == objectStates.isMoving)
        //{
        //    thisRenderer.material = playBackManager.redMat;
        //}

        //if (state == objectStates.isPlaced)
        //{
        //    thisRenderer.material = playBackManager.greenMat;
        //}
    }

}
