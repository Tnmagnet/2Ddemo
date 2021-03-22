using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    private Collision _collision;
    [HideInInspector]
    public Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private float x, y, xRaw, yRaw;
    private Vector2 direction;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    private bool dash;
    private bool jump;
    private bool wallJump;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collision = GetComponent<Collision>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
        direction=new Vector2(x,y);
        //落地重置冲刺次数
        if (_collision.onGround && !isDashing)
        {
            hasDashed = false;
            //TODO:开启更自然的跳跃
        }
        //抓墙成功
        if (Input.GetButton("Fire3") && _collision.onWall && canMove)
        {
            wallGrab = true;
            wallSlide = false;
        }
        //抓墙不成功
        if (Input.GetButton("Fire3") || _collision.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }
        //墙上滑
        if (_collision.onWall && !_collision.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
            }
        }
        //滑行停止
        if (!_collision.onWall || _collision.onGround)
        {
            wallSlide = false;
        }

        if (Input.GetButton("Fire1") && !hasDashed)
        {
            dash = true;
        }
        //墙边跳及普通跳跃
        if (Input.GetButton("Jump"))
        {
            if (_collision.onGround)
            {
                jump = true;
            }

            if (_collision.onWall)
            {
                wallJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    void Run(Vector2 direction)
    {
        if(!canMove)
            return;
        if(wallGrab)
            return;
        if (!wallJumped)
        {
            _rigidbody2D.velocity=new Vector2(direction.x*speed,_rigidbody2D.velocity.y);
        }
        else
        {
            _rigidbody2D.velocity = Vector2.Lerp(_rigidbody2D.velocity,
                (new Vector2(direction.x * speed, _rigidbody2D.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    void Jump(Vector2 direction, bool onWall)
    {
        var velocity = _rigidbody2D.velocity;
        velocity=new Vector2(velocity.x,0);//y方向速度清零
        velocity += direction * jumpForce;
        _rigidbody2D.velocity = velocity;
    }

    void Dash(Vector2 direction)
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.velocity += direction * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        _rigidbody2D.gravityScale = 0;
        yield return new WaitForSeconds(0.3f);
        _rigidbody2D.gravityScale = 3;
    }
}