using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GameObject Platform;
    // Start is called before the first frame update
    void Start()
    {
        Platform.transform.position = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
