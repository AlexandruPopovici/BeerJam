using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    public const float NORMAL_WAIT = 0.75f;
    public const float ALERTED_WAIT = 0.35f;

    public float wait = NORMAL_WAIT;
    float waitBehaviour = 3f;
    float acc = 0;
    float accBehaviour = 0;

    System.Random random = new System.Random();
    private float[] dirProbability = new float[] { 0.25f, 0.25f, 0.25f, 0.25f };

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

    private bool __busy;
    public bool Busy
    {
        get
        {
            return __busy;
        }
        set
        {
            this.__busy = value;
        }
    }

    private bool __roaming;
    public bool Roaming
    {
        get
        {
            return __roaming;
        }
        set
        {
            this.__roaming = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Busy)
            return;
        if (Roaming)
        {
            acc += Time.deltaTime;
            accBehaviour += Time.deltaTime;
            if (acc >= wait)
            {
                UpdateMonsterRoam();
                acc -= wait;
            }
            if(accBehaviour >= waitBehaviour)
            {
                UpdateMonsterBehaviour();
                accBehaviour -= waitBehaviour;
            }
        }



	}

    void UpdateMonsterRoam()
    {
        Vector2 randomDir = Generator.GetDirection();
        Vector2 newPos = TilePosition + randomDir;
        newPos.x = Mathf.Clamp(newPos.x, 0, 64);
        newPos.y = Mathf.Clamp(newPos.y, 0, 64);
        //Debug.Log(randomDir);
        transform.position = new Vector3(newPos.x * 1.28f, newPos.y * 1.28f, 0f);
        TilePosition = new Vector2(newPos.x, newPos.y);
    }

    void UpdateMonsterBehaviour()
    {
        dirProbability[0] = (float)random.NextDouble();
        dirProbability[1] = (float)random.NextDouble();
        dirProbability[2] = (float)random.NextDouble();
        dirProbability[3] = (float)random.NextDouble();
        //Debug.Log(dirProbability[0].ToString() + "," + dirProbability[1].ToString() +"," + dirProbability[2].ToString() + "," + dirProbability[3].ToString());
    }

}
