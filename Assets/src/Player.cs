using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Transform targetArrow;

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

    private Vector2 __lookDir;
    public Vector2 lookDir
    {
        get
        {
            return __lookDir;
        }
        set
        {
            __lookDir = value;
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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Busy)
            return;

        if (Input.GetKeyDown(KeyCode.A))
            Move(Vector2.left);

        if (Input.GetKeyDown(KeyCode.W))
            Move(Vector2.up);

        if (Input.GetKeyDown(KeyCode.S))
            Move(Vector2.down);

        if (Input.GetKeyDown(KeyCode.D))
            Move(Vector2.right);

        if (Input.GetKeyDown(KeyCode.Space))
            Dig();

        if (Input.GetKeyDown(KeyCode.E))
            Sniff();
    }

    private void Move(Vector2 direction)
    {
        Debug.Log("Move");
        if (Generator.CanMove((int)TilePosition.x, (int)TilePosition.y, direction))
        {
            transform.Translate(direction * 1.28f);
            TilePosition += direction;
        }
        lookDir = direction;
    }

    private void Dig()
    {
        Generator.TryDig();
    }

    private void Sniff()
    {
        Generator.TrySniff();
    }

    public void SetArrow(Vector3 dir)
    {
        float angle = Mathf.Acos(Vector3.Dot(Vector3.left, dir));
        //Debug.Log(dir);
        //Debug.Log(angle * Mathf.Rad2Deg);
        StartCoroutine(animateArrow(angle * Mathf.Rad2Deg * Mathf.Sign(-dir.y)));
    }

    IEnumerator animateArrow(float angle)
    {
        targetArrow.rotation = Quaternion.Euler(0, 0, angle);
        Color color = targetArrow.GetComponent<SpriteRenderer>().color;
        while(color.a < 0.8f)
        {
            color.a += Time.deltaTime * 0.5f;
            targetArrow.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        while (color.a > 0.0f)
        {
            color.a -= Time.deltaTime * 0.5f;
            targetArrow.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
