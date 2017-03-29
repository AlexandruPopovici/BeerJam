using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour {

    public float cooldown;
    public string name;
    private float counter;

    public Action PowerAvailableCallback;
    private bool __active = true;
    public bool Active
    {
        get
        {
            return __active;
        }
        set
        {
            __active = value;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!Active)
        {
            counter += Time.deltaTime;
            if(counter >= cooldown)
            {
                Active = true;
                if (PowerAvailableCallback != null)
                    PowerAvailableCallback();
            }
        }

	}

    public bool Use()
    {
        if (Active)
        {
            counter = 0;
            Active = false;
            return true;
        }
        return false;
    }
}
