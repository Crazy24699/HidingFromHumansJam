using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttatchedShield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Virus"))
        {
            Destroy(GameObject.Find(collision.gameObject.name));
        }
    }
}
