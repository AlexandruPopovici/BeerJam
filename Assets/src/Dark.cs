using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Dark : ImageEffectBase
{
    public Texture2D bitemaskTop;
    public Texture2D maskBottom;
    public float bite = 0.6f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //bite = Mathf.Lerp(-0.05f, 0.25f, Mathf.PingPong(Time.time * 0.5f, 1));
        //bite = Mathf.Clamp(bite, -0.25f, 0.25f);
        material.SetFloat("_Bite", bite);
        material.SetTexture("_MaskTop", bitemaskTop);
        material.SetTexture("_MaskBottom", maskBottom);
        //material.SetFloat("_RampOffset", rampOffset);
        Graphics.Blit(source, destination, material);
    }
}
