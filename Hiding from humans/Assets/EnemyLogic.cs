using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float playerDetectionRange = 5f;
    protected Transform Player;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isFacingRight = true;
    private bool isGrounded;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Check if the Player is within the detection range
        float distanceToPlayer = Mathf.Abs(Player.position.x - transform.position.x);
        bool isPlayerInRange = distanceToPlayer <= playerDetectionRange;

        if (isPlayerInRange)
        {
            // Move towards the Player
            float moveDirection = Mathf.Sign(Player.position.x - transform.position.x);
            rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);

            // Flip the enemy based on the Player's position
            if (moveDirection > 0 && !isFacingRight || moveDirection < 0 && isFacingRight)
            {
                Flip();
            }
        }
        else if (!isGrounded)
        {
            // Flip when reaching the edge of a platform
            Flip();
        }

        // Move in the current direction
        float patrolDirection = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveSpeed * patrolDirection, rb.velocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(Vector3.up, 180f);
    }


    public void Die()
    {
        Destroy(this.gameObject);
    }

}
