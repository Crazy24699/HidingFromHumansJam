using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GenerateArea GenerateArenaScirpt;
    public PlatformScript PlatformScriptRef;

    public int CurrentLevel;

    public GameObject Platform;
    // Start is called before the first frame update
    void Start()
    {
        Platform.transform.position = new Vector2(0, 0);

        GenerateArenaScirpt.AreaGenerationStart();

        PlatformScriptRef.PlatformStart();

        PlatformScriptRef.HandleSpawningLogic();

        InvokeRepeating("IncreaseVirusLevel", 5, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseVirusLevel()
    {
        GenerateArenaScirpt.StartCoroutine(GenerateArenaScirpt.IncrimentVirus(GenerateArenaScirpt.CurrentVirusLevel));
    }
}
