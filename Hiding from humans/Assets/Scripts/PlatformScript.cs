using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class PlatformScript : MonoBehaviour
{
    public int JumpValue = 5;

    public GenerateArea GenerateAreaScript;

    public List<Vector2Int> WallCordinates = new List<Vector2Int>();

    public int BoundsHeight;
    public int PlatformCount;

    public GameObject PlatformPrefab;

    public int PlatformLength = 6;

    public List<Vector2Int> PlatformList;

    public void PlatformStart()
    {
        GenerateAreaScript = GameObject.FindObjectOfType<GenerateArea>();
        WallCordinates = new List<Vector2Int>(GenerateAreaScript.Walls);
        BoundsHeight = GenerateAreaScript.Height;

        PlatformCount = BoundsHeight / JumpValue;
    }

    public void HandleSpawningLogic()
    {
        Vector2Int CurrentPlatformPosition;
        Vector2Int LastPlatformPositon;
        

        switch (PlatformList.Count)
        {
            default:
            case 0:
                CurrentPlatformPosition = Vector2Int.zero;
                LastPlatformPositon = new Vector2Int(0,1);
                break;

            case > 1:
                LastPlatformPositon = PlatformList[PlatformList.Count - 1];
                break;

        }
        for (int i = 0; i < PlatformCount; i++)
        {
            if (LastPlatformPositon.y+7>BoundsHeight)
            {
                Debug.Log("as i break");
                i = PlatformCount;
            }
            
            switch (LastPlatformPositon.x)
            {
                default:
                case 0:

                    Vector2Int[] DirectionChoice = new Vector2Int[2]
                    {
                        Vector2Int.left,
                        Vector2Int.right,
                    };
                    CurrentPlatformPosition = DirectionChoice[Random.Range(0, DirectionChoice.Length)];

                    break;

                case < 0:
                    CurrentPlatformPosition = new Vector2Int(+ 9, LastPlatformPositon.y + 5);
                    break;

                case > 0:
                    CurrentPlatformPosition = new Vector2Int( - 9, LastPlatformPositon.y + 5);

                    break;
            }
            SpawnPlatforms(CurrentPlatformPosition);
            PlatformList.Add(CurrentPlatformPosition);
            LastPlatformPositon = CurrentPlatformPosition;
        }
    }

    public void GetEnclosingCells()
    {
        List<Vector2Int> MatchingYCords = new List<Vector2Int>();
        MatchingYCords = WallCordinates.FindAll(i => i.y == 3);

        int LeftValue = MatchingYCords.Min(L => L.x);
        int RightValue = MatchingYCords.Max(R => R.x);



    }

    public void SpawnPlatforms(Vector2 Position)
    {
        
        GameObject PlatformRef;
        //Vector3 WorldConversions;


        PlatformRef = Instantiate(PlatformPrefab, Position.ConvertTo<Vector3>() + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        PlatformRef.name = "Platform" + PlatformList.Count.ToString();
        PlatformRef.transform.localScale = new Vector3Int(5, 1, -10);

    }
    
}
