using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    //人物平移速度
    public float speed = 5f;
    //人物位置
    private float x;
    private float y;
    //人物跳跃速度
    [Range(0, 10)] public float jumpVelocity = 5;
    //下降系数
    public float fallMultiplier = 2.5f;
    //小跳
    public float lowJumpMultiplier = 2;
    //墙边
    public float grabWallMultiplier = 0.8f;

    public LayerMask mask;
    //碰撞箱
    public float downBoxHeight=0.5f;

    private Vector2 playerSize;

    private Vector2 downBoxSize;
    private Vector2 lrBoxSize;

    private Vector2 leftBoxCenter;
    private Vector2 rightBoxCenter;
    private Vector2 downBoxCenter;
    //跳跃以及墙边判定
    private bool jumpRequest;

    private bool onGround;
    private bool onWall;

    private Rigidbody2D _rigidbody2D;

    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        playerSize = GetComponent<SpriteRenderer>().bounds.size;
        
        downBoxSize=new Vector2(playerSize.x*0.6f,downBoxHeight);
        lrBoxSize=new Vector2(playerSize.x*0.1f,playerSize.y*0.6f);
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        if (x > 0)
        {
            _rigidbody2D.transform.eulerAngles=new Vector3(0,0,0);
            _animator.SetBool("run", true);
        }
        if (x < 0)
        {
            _rigidbody2D.transform.eulerAngles=new Vector3(0,180,0);
            _animator.SetBool("run",true);
        }

        if (x < 0.001 && x > -0.001)
        {
            _animator.SetBool("run",false);
        }
        Run();
        if (Input.GetButtonDown("Jump") && onGround)
        {
            jumpRequest = true;
        }
        
    }

    private void FixedUpdate()
    {
        //进行人物脚下以及两侧的碰撞判定，用于平台判定和墙边判定
        if (jumpRequest)
        {
            _rigidbody2D.AddForce(Vector2.up*jumpVelocity,ForceMode2D.Impulse);
            jumpRequest = false;
            onGround = false;
            _animator.SetBool("grounded",false);
        }
        else
        {
            downBoxCenter = (Vector2) transform.position + Vector2.down * (playerSize.y * 0.5f);
            leftBoxCenter = (Vector2) transform.position + Vector2.left * (playerSize.x * 0.3f);
            rightBoxCenter = (Vector2) transform.position + Vector2.right * (playerSize.x * 0.3f);
            if (Physics2D.OverlapBox(downBoxCenter, downBoxSize, 0, mask)==null)
            {
                onGround = false;
                _animator.SetBool("grounded",false);
            }
            else
            {
                onGround = true;
                _animator.SetBool("grounded",true);
            }

            if (Physics2D.OverlapBox(leftBoxCenter, lrBoxSize, 0, mask) ||
                Physics2D.OverlapBox(rightBoxCenter, lrBoxSize, 0, mask))
            {
                onWall = true;
            }
            else
            {
                onWall = false;
            }
        }
        //添加跳跃时上升与下降的速度系数，使得跳跃更加真实
        if (_rigidbody2D.velocity.y < 0)
        {
            _rigidbody2D.gravityScale = fallMultiplier;
        }else if (_rigidbody2D.velocity.y > 0&& !Input.GetButton("Jump"))
        {
            _rigidbody2D.gravityScale = lowJumpMultiplier;
        }
        else
        {
            _rigidbody2D.gravityScale = 1;
        }
        //根据人物是否在墙边改变下落速度
        if (onWall == true && onGround == false)
        {
            _rigidbody2D.gravityScale = grabWallMultiplier;
        }
    }
    private void Run()//人物移动
    {
        Vector3 movement = new Vector3(x, y, 0);
        _rigidbody2D.transform.position += movement * (speed * Time.deltaTime);
    }
    private void OnDrawGizmos()
    {
        if (onGround)
        {
            Gizmos.color=Color.red;
        }
        else
        {
            Gizmos.color=Color.green;
        }
        Gizmos.DrawWireCube(downBoxCenter,downBoxSize);
        if (onWall)
        {
            Gizmos.color=Color.red;
        }
        else
        {
            Gizmos.color=Color.green;
        }
        Gizmos.DrawWireCube(leftBoxCenter,lrBoxSize);
        Gizmos.DrawWireCube(rightBoxCenter,lrBoxSize);
    }
}
