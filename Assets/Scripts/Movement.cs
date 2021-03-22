using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
    public Vector2 velocity;

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
    
    private bool groundTouch;
    private bool hasDashed;
    private bool hasJumped;

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
        velocity = _rigidbody2D.velocity;
        //落地重置冲刺次数
        if (_collision.onGround && !isDashing)
        {
            hasDashed = false;
            GetComponent<BetterJump>().enabled = true;
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

        if (Input.GetButton("Fire1") && !hasDashed&&(xRaw!=0||yRaw!=0))
        {
            dash = true;
        }
        //墙边跳及普通跳跃
        if (Input.GetButton("Jump"))
        {
            if (_collision.onGround&&!hasJumped)
            {
                jump = true;
            }

            if (_collision.onWall)
            {
                wallJump = true;
            }
        }
        //判断墙的方向
        if (x > 0)
        {
            side = 1;
            _rigidbody2D.transform.eulerAngles=new Vector3(0f,0f,0f);
        }

        if (x < 0)
        {
            side = -1;
            _rigidbody2D.transform.eulerAngles=new Vector3(0f,180f,0f);
        }
    }

    private void FixedUpdate()
    {
        if (_collision.onGround)
        {
            _animator.SetBool("grounded",true);
        }
        Run(direction);
        if (jump)
        {
            Jump(Vector2.up);
        }

        if (dash)
        {
            Dash(xRaw,yRaw);
        }

        if (wallSlide)
        {
            WallSilde();
        }
    }

    void Run(Vector2 direction)
    {
        if(!canMove)
            return;
        if(wallGrab)
            return;
        _animator.SetBool("run",true);
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

    void Jump(Vector2 direction)
    {
        _animator.SetBool("jump",true);
        var velocity = _rigidbody2D.velocity;
        velocity=new Vector2(velocity.x,0);//y方向速度清零
        velocity += direction * jumpForce;
        _rigidbody2D.velocity = velocity;
    }

    void Dash(float x,float y)
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.velocity += new Vector2(x,y) * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        _rigidbody2D.gravityScale = 0;
        GetComponent<BetterJump>().enabled = false;
        yield return new WaitForSeconds(0.3f);
        _rigidbody2D.gravityScale = 3;
        GetComponent<BetterJump>().enabled = true;
    }

    void WallJump()
    {
        if ((side == 1 && _collision.onRightWall) || (side == -1 && _collision.onLeftWall))
        {
            side *= -1;
            _rigidbody2D.transform.eulerAngles=new Vector3(0f,180f,0f);
        }
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(0.1f));
        Vector2 wallDir = _collision.onRightWall ? Vector2.left : Vector2.right;
        Jump(Vector2.up/1.5f+wallDir/1.5f);
        wallJumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void WallSilde()
    {
        if(_collision.wallSide != side)
            _collision.transform.eulerAngles=new Vector3(0,180f,0f);

        if (!canMove)
            return;

        var velocity = _rigidbody2D.velocity;
        bool pushingWall = (velocity.x > 0 && _collision.onRightWall) || (velocity.x < 0 && _collision.onLeftWall);
        float push = pushingWall ? 0 : velocity.x;

        velocity = new Vector2(push, -slideSpeed);
        _rigidbody2D.velocity = velocity;
    }
}