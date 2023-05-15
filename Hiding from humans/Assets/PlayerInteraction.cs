using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject ShieldRef;
    private bool AttackReady = true;

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
            Debug.Log("stop");
            Time.timeScale = 0;
        }
    }

    
    public IEnumerator AttackCooldown()
    {
        AttackReady = false;
        yield return new WaitForSeconds(0.5f);
        AttackReady = true;
    }
}
