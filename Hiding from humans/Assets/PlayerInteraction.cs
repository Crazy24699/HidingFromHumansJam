using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Virus"))
        {
            Debug.Log("stop");
            Time.timeScale = 0;
        }
    }

    

}
