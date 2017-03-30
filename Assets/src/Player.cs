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

    private bool __isMoving;
    public bool IsMoving
    {
        get{
            return __isMoving;
        }
        set{
            __isMoving = value;
        }
    }

    private float __speed = 3f;
    public float Speed
    {
        get
        {
            return __speed;
        }
        set
        {
            __speed = value;
        }
    }

    private Animator animator;
    private int animationLayer;
    private AudioSource audioSource;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        SampleAnimation(1, 0f);
    }
	
	// Update is called once per frame
	void Update () {
        if (Busy)
            return;

        if (Input.GetKey(KeyCode.A))
        {
            animationLayer = 2;
            if(Move(Vector2.left))
                SetAnimationLayer(2);
        }
        if (Input.GetKey(KeyCode.W))
        {
            animationLayer = 0;
            if(Move(Vector2.up))
                SetAnimationLayer(0);
        }

        if (Input.GetKey(KeyCode.S))
        {
            animationLayer = 1;
            if(Move(Vector2.down))
                SetAnimationLayer(1);
        }

        if (Input.GetKey(KeyCode.D)){
            animationLayer = 3;
            if(Move(Vector2.right))
                SetAnimationLayer(3);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Dig();

        if (Input.GetKeyDown(KeyCode.E))
            Sniff();

        if(Input.GetKeyDown(KeyCode.Return))
            Generator.Restart();
    }

    //I'm pretty sure this is NOT how mechanim is supposed to work, but learning how it should, aint nobody got time fo that!
    private void SetAnimationLayer(int layer){
        for(int i = 0 ; i < 4; i++){
            if(i == layer)
                animator.SetLayerWeight(i, 1);
            else
                animator.SetLayerWeight(i, 0);
        }
        animator.speed = 1f;
    }

    //I'm pretty sure this is NOT how mechanim is supposed to work, but learning how it should, aint nobody got time fo that!
    private void SampleAnimation(int layer, float time){
        SetAnimationLayer(layer);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        animator.Play (stateInfo.nameHash, layer, time);
        animator.speed = 0f;
    }

    private bool Move(Vector2 direction)
    {
        bool ret = Generator.CanMove((int)TilePosition.x, (int)TilePosition.y, direction);
        if (ret)
        {   
            if(!IsMoving)
                StartCoroutine(_move(direction));
        }
        else{
            if(!IsMoving){
                SampleAnimation(animationLayer, 0f);
            }
        }
        lookDir = direction;
        return ret;
    }

    private IEnumerator _move(Vector2 direction){
        IsMoving = true;
        Vector3 start = transform.position;
        Vector3 target = transform.position + (Vector3)direction * 1.28f;
        float distance = Vector3.Distance(target, transform.position);
        float tx = 0, ty = 0;
        while(distance > 0.05f && tx <= 1.0 && ty <= 1.0f){
            //Has a weird semi-linearity to it which looks nice
            transform.Translate(direction * 1.28f * Time.deltaTime * Speed);
            if(target.x - start.x != 0)
                tx = (transform.position.x - start.x) / (target.x - start.x);
            if(target.y - start.y != 0)
                ty = (transform.position.y - start.y) / (target.y - start.y);
            distance = Vector3.Distance(target, transform.position);
            yield return new WaitForEndOfFrame();
        }
        transform.position = target;
        TilePosition += direction;
        SampleAnimation(animationLayer, 0f);
        IsMoving = false;
    }

    private void Dig()
    {
        if (Generator.TryDig())
        {
            GetComponent<AudioSource>().Play();
        }
    }

    private void Sniff()
    {
        Generator.TrySniff();
    }

    public void SetArrow(Vector3 dir)
    {
        float angle = Mathf.Acos(Vector3.Dot(Vector3.left, dir));
        StartCoroutine(animateArrow(angle * Mathf.Rad2Deg * Mathf.Sign(-dir.y)));
    }

    IEnumerator animateArrow(float angle)
    {
        targetArrow.rotation = Quaternion.Euler(0, 0, angle);
        Color color = targetArrow.GetComponent<SpriteRenderer>().color;
        while(color.a < 0.5f)
        {
            color.a += Time.deltaTime;// * 0.5f;
            targetArrow.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        while (color.a > 0.0f)
        {
            color.a -= Time.deltaTime;// * 0.5f;
            targetArrow.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
