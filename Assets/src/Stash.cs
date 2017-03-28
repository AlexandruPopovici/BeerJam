using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash : MonoBehaviour {

    private Vector2 __tilePosition;
    public Vector2 TilePosition
    {
        get
        {
            return __tilePosition;
        }
        set
        {
            this.__tilePosition = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
