using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;

    private float fallMultiplier = 2.5f;

    private float lowJumpMultiplier = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_rigidbody2D.velocity.y < 0)
        {
            _rigidbody2D.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rigidbody2D.velocity.y > 0)
        {
            _rigidbody2D.velocity += Vector2.up
                                     * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}