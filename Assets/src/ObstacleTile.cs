﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTile : Tile {

    private SpriteRenderer _renderer;

    private bool __shallPass = false;
    public override bool ShallPass
    {
        get
        {
            return __shallPass;
        }
    }

    // Use this for initialization
    void Start () {
        _renderer = GetComponent<SpriteRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    override public void TearDown(Action cb)
    {
        StartCoroutine(_tearDown(cb));
    }

    IEnumerator _tearDown(Action callback)
    {
        yield return new WaitForSeconds(2f);
        __shallPass = true;
        _renderer.color = Color.red;
        callback();
    }
}
