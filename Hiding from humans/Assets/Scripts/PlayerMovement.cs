using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;
    public float MoveSpeedMultiplier;
    public float HorizontalMovement;

    [HideInInspector]public int FacingDirection=1;

    public Rigidbody2D RB2D;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        HorizontalMovement = Input.GetAxisRaw("Horizontal");
        RB2D.velocity = new Vector2(HorizontalMovement * MoveSpeed, RB2D.velocity.y);
        FLip();
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


}
