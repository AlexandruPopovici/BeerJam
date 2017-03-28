using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 goalPos = target.position;
        //goalPos.y = transform.position.y;
        Vector3 pos = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        transform.position = new Vector3(pos.x, pos.y, -1f);

    }
}
