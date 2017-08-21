using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    [HideInInspector]
    public float moveCamToX;

    // Use this for initialization
    void Start () {
        moveCamToX = transform.position.x;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 moveToVector = new Vector3(moveCamToX,  transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, moveToVector, 0.01f);
    }
}
