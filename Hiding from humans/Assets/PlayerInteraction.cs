using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject ShieldRef;
    private bool AttackReady = true;
    public GameObject DeathScreen;
    public GameObject WinScreen;
    public void Attack()
    {
        Instantiate(ShieldRef, new Vector2((this.transform.position.x + 1.5f) * (this.transform.lossyScale.x / this.transform.lossyScale.x), this.transform.position.y), Quaternion.identity);
        
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && AttackReady)
        {
            Attack();
            StartCoroutine(AttackCooldown());
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Virus"))
        {
            PlayerMovement PlayerMoveScript = GameObject.FindObjectOfType<PlayerMovement>();
            PlayerMoveScript.PauseGame();
            DeathScreen.SetActive(true);
        }


    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Escape"))
        {
            PlayerMovement PlayerMoveScript = GameObject.FindObjectOfType<PlayerMovement>();
            PlayerMoveScript.PauseGame();
            WinScreen.SetActive(true);
        }
    }

    public IEnumerator AttackCooldown()
    {
        AttackReady = false;
        yield return new WaitForSeconds(0.5f);
        AttackReady = true;
    }
}
