using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;
    public float HorizontalMovement;
    public float GroundRadius;

    public float JumpMultiplyer;
    public float JumpForce;

    [HideInInspector]public int FacingDirection=1;

    public bool IsGrounded = false;

    public Rigidbody2D RB2D;
    public LayerMask GroundLayer;
    public Transform GroundPoint; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GroundPoint.position, GroundRadius);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        IsGrounded=Physics2D.OverlapCircle(GroundPoint.transform.position, GroundRadius,GroundLayer);

        HorizontalMovement = Input.GetAxisRaw("Horizontal");
        RB2D.velocity = new Vector2(HorizontalMovement * MoveSpeed, RB2D.velocity.y);
        FLip();

        if(IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void FLip()
    {
        switch (HorizontalMovement)
        {
            case -1:
                FacingDirection = -1;
                break;

            case 1:
                FacingDirection = 1;
                break;
        }

        Vector3 LocalScale = transform.localScale;
        LocalScale.x = FacingDirection;
        transform.localScale=LocalScale;
    }

    public void Jump()
    {
        RB2D.AddForce(new Vector2(0, JumpForce * JumpMultiplyer), ForceMode2D.Impulse);
    }

}
