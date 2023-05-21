using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class ShieldProjection : MonoBehaviour
{
    protected Rigidbody2D RB2D;
    protected Animator ShieldAnimation;
    protected Transform Player;

    public void Awake()
    {
        StartCoroutine(Despawn());
        RB2D = GetComponent<Rigidbody2D>();

        Player = GameObject.FindGameObjectWithTag("Player").transform;

        int Direction;
        switch (Player.transform.lossyScale.x)
        {
            default:
            case > 0:
                Direction = 1;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;

            case < 0:
                Direction = -1;
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }

        RB2D.velocity = new Vector2(Direction * 22, RB2D.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Virus"))
        {
            Destroy(GameObject.Find(collision.name));
            Destroy(this.gameObject);
        }
    }

    public IEnumerator Despawn()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(this.gameObject);
    }
}
