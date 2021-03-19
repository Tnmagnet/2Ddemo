using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private Vector2 playerSize;

    [Header("Layers")]
    public LayerMask layerMask;

    [Space] 
    [HideInInspector]
    
    public bool onGround;
    
    public bool onWall;

    public bool onLeftWall;

    public bool onRightWall;

    public int wallSide;

    [Space] 
    [Header("Collision")] 
    
    public float collisionRadius = 0.25f;

    public float downBoxPosition, leftBoxPosition, rightBoxPosition;
    // Start is called before the first frame update
    void Start()
    {
        playerSize = GetComponent<SpriteRenderer>().bounds.size;

    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2) transform.position + Vector2.down * (playerSize.y * downBoxPosition), collisionRadius, layerMask);
        onWall = Physics2D.OverlapCircle((Vector2) transform.position + Vector2.left * (playerSize.x * leftBoxPosition),
                     collisionRadius, layerMask) ||
                 Physics2D.OverlapCircle(
                     (Vector2) transform.position + Vector2.right * (playerSize.x * rightBoxPosition), collisionRadius,
                     layerMask);
        onLeftWall = Physics2D.OverlapCircle(
            (Vector2) transform.position + Vector2.left * (playerSize.x * leftBoxPosition),
            collisionRadius, layerMask);
        onRightWall = Physics2D.OverlapCircle(
            (Vector2) transform.position + Vector2.right * (playerSize.x * rightBoxPosition), collisionRadius,
            layerMask);
        wallSide = onRightWall ? -1 : 1;
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
        Gizmos.DrawWireSphere((Vector2) transform.position + Vector2.down * (playerSize.y * downBoxPosition),collisionRadius);
        if (onLeftWall)
        {
            Gizmos.color=Color.red;
        }
        else
        {
            Gizmos.color=Color.green;
        }
        Gizmos.DrawWireSphere((Vector2) transform.position + Vector2.left * (playerSize.x * leftBoxPosition),collisionRadius);
        if (onRightWall)
        {
            Gizmos.color=Color.red;
        }
        else
        {
            Gizmos.color=Color.green;
        }
        Gizmos.DrawWireSphere((Vector2) transform.position + Vector2.right * (playerSize.x * rightBoxPosition),collisionRadius);
    }
}
