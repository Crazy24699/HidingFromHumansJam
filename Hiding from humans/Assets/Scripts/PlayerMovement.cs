using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;
    public float HorizontalMovement;
    public float GroundRadius;

    public float JumpMultiplyer;
    public float JumpForce;

    [HideInInspector]public int FacingDirection;

    public bool IsGrounded = false;
    private bool isPaused = false;


    public Rigidbody2D RB2D;
    public LayerMask GroundLayer;
    public Transform GroundPoint; 
    public Animator PlayerAnimations;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void Move()
    {
        IsGrounded=Physics2D.OverlapCircle(GroundPoint.transform.position, GroundRadius,GroundLayer);

        HorizontalMovement = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        RB2D.velocity = new Vector2(HorizontalMovement, RB2D.velocity.y);
        FLip();

        PlayerAnimations.SetFloat("Current Speed", Mathf.Abs(HorizontalMovement));



        if(IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void FLip()
    {
        Vector3 LocalScale = transform.localScale;
        
        switch (HorizontalMovement)
        {
            default:
            case 0:
                FacingDirection = Mathf.FloorToInt(LocalScale.x);
                Debug.Log(FacingDirection);
                break;

            case < 0:
                //FacingDirection = Mathf.FloorToInt(LocalScale.x) * -1;
                FacingDirection = -2;
                
                break;

            case > 0:
                //FacingDirection = Mathf.FloorToInt(LocalScale.x) * 1;
                FacingDirection = 2;
                break;
        }


        LocalScale.x = FacingDirection;
        transform.localScale=LocalScale;
    }

    public void Jump()
    {
        RB2D.AddForce(new Vector2(0, JumpForce * JumpMultiplyer), ForceMode2D.Impulse);
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Set time scale to 0 to pause the game
        Debug.Log("Game paused");
        // Add any other pause-related actions or UI display here
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // Set time scale back to 1 to resume the game
        Debug.Log("Game resumed");
        // Add any other resume-related actions or UI hiding here
    }

}
