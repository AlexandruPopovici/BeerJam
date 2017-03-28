using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour {

	// Use this for initialization
	void Awake () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Vector2 WordSizepx
    {
        get
        {
            return new Vector2(Screen.width, Screen.height);
        }
        set
        {
            Vector3 worldSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
            px_to_world = worldSize.x*2f / Screen.width;
            world_to_px = 1f / px_to_world;
        }
    }

    public static Vector2 WorldSize
    {
        get
        {
            return (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        }
        set
        {
            Vector3 worldSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
            px_to_world = worldSize.x*2f  / Screen.width;
            world_to_px = 1f / px_to_world;
        }
    }

    private static float px_to_world;
    private static float world_to_px;

    public static float pxToWorldPos(float pixels) {
        return px_to_world * pixels;
    }

    public static float worldToPxPos(float worldUnits)
    {
        return world_to_px * worldUnits;
    }
}
