using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GenerateArea GenerateArenaScirpt;
    public PlatformScript PlatformScriptRef;

    public int CurrentLevel;

    public GameObject EnemyPrefab;

    public List<GameObject> EnemiesInScene;
    public List<GameObject> PlatformsInScene = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        GenerateArenaScirpt.AreaGenerationStart();

        if(GenerateArenaScirpt.Walls.Count < 10)
        {
            GenerateArenaScirpt.AreaGenerationStart();
            Debug.LogError("Had to re generate");
        }

        PlatformScriptRef.PlatformStart();

        PlatformScriptRef.HandleSpawningLogic();

        IncreaseVirusLevel();
        SpawnEnemies();
        //InvokeRepeating("IncreaseVirusLevel", 5, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies()
    {
        int Counter = 0;
        foreach (var PlatformPoint in PlatformsInScene)
        {
            Counter++;
            GameObject EnemyRef;
            EnemyRef = Instantiate(EnemyPrefab, new Vector2(PlatformPoint.transform.position.x, PlatformPoint.transform.position.y + 2), Quaternion.identity);
            EnemyRef.name = "Enemy" + Counter;


        }
    }

    public void IncreaseVirusLevel()
    {
        GenerateArenaScirpt.StartCoroutine(GenerateArenaScirpt.IncrimentVirus(GenerateArenaScirpt.CurrentVirusLevel));
    }

   
}
