using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World_Fixer : MonoBehaviour {

    public float x = 0;
    public float y = 0;
    public float z = 0;
    public float qx = 0;
    public float qy = 0;
    public float qz = 0;
    public float qw = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 move = new Vector3(x, y, z);
        Quaternion rotate = new Quaternion(qx, qy, qz, qw);
        gameObject.transform.position = move;
        gameObject.transform.rotation *= rotate;
	}
}
