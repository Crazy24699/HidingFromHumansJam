using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;
using UnityEngine.UIElements;


public class GenerateArea : MonoBehaviour
{
    public Tilemap AreaMap;
    public Tilemap VirusMap;

    public TileBase WallTiles;
    public TileBase VirusTiles;

    [SerializeField]protected int MinWidth;
    [SerializeField]protected int MaxWidth;

    public int Height=80;
    public int DifficultyChosen = 1;

    public int AreaWidth;

    public int CurrentVirusLevel;

    protected int TotalUnitsOver = 5;       //How many units the program is allowed to go over on the x axis 
    public int CurrentUnitsOver;

    public GameObject CPUEscapeRef;

    [SerializeField]protected List<Vector2Int> Directions = new List<Vector2Int>()
    {
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,1),
    };

    public Vector2Int LeftStartingLocation;
    public Vector2Int RightStartingLocation;

    //public Dictionary<string, HashSet<Vector2Int>> WireBounds = new Dictionary<string, HashSet<Vector2Int>>()
    //{
    //    {"LeftWall",new HashSet<Vector2Int>()},
    //    {"RightWall",new HashSet<Vector2Int>()}
    ////};

    public HashSet<Vector2Int> Walls = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> Floor = new HashSet<Vector2Int>();

    public void AreaGenerationStart()
    {
        if (MinWidth == 0)
        {
            MinWidth = 15;
        }
        if(MaxWidth == 0)
        {
            MaxWidth = 25;
        }

        PaintTile(AreaMap, WallTiles, new Vector2Int(-40,0));
        PaintTile(AreaMap, WallTiles, new Vector2Int(0,0));
        PaintTile(AreaMap, WallTiles, new Vector2Int(40,0));
        PaintTile(AreaMap, WallTiles, new Vector2Int(40,Height));

        GenerateFloor();
        GenerateWallPositions();


        //List<Vector2Int> MatchingYCords = new List<Vector2Int>(Walls);

        //for (int y = 1; y < 6; y++)
        //{
        //    List<Vector2Int> UseableVectors = new List<Vector2Int>();
        //    UseableVectors.AddRange(MatchingYCords.FindAll(i => i.y == y));

        //    int LeftValue = UseableVectors.Min(L => L.x);
        //    int RightValue = UseableVectors.Max(R => R.x);

        //    PaintTile(AreaMap, VirusTiles, new Vector2Int(RightValue, y));
        //    PaintTile(AreaMap, VirusTiles, new Vector2Int(LeftValue, y));
        //}

    }

    public void PaintTile(Tilemap ArenaGrid, TileBase TileType, Vector2Int Position)
    {
        Vector3Int TilePositon = ArenaGrid.WorldToCell((Vector3Int)Position);
        ArenaGrid.SetTile(TilePositon, TileType);

    }

    //spawn platforms in either 6 or 7's

    //Spawn a platform in specific numbers. Every 3-4 level platform if there is time for that

    public void SpawnMidPoint()
    {
        List<Vector2Int> MatchingYCords = new List<Vector2Int>(Walls);


        Vector2Int SpawnLocation = new Vector2Int(0, Height / 2);

        List<Vector2Int> UseableVectors = new List<Vector2Int>();
        //UseableVectors.AddRange(MatchingYCords.FindAll(i => i.y == y));

        int LeftValue = UseableVectors.Min(L => L.x);
        int RightValue = UseableVectors.Max(R => R.x);

    }

    public void GenerateFloor()
    {
        int ChosenWidth = Random.Range(MinWidth, MaxWidth);
        AreaWidth = ChosenWidth;
        RightStartingLocation.x = ChosenWidth;
        LeftStartingLocation.x = ChosenWidth * -1;

        Vector2Int GenerationDirection; 

        for (int y = 0; y < 1; y++)
        {
            switch (y)
            {
                default:
                case 0:
                    GenerationDirection = new Vector2Int(1, 0);
                    break;
                case 1:
                    GenerationDirection = new Vector2Int(-1, 0);

                    break;
            }
            for (int i = 0; i < ChosenWidth; i++)
            {
                Floor.Add(new Vector2Int(LeftStartingLocation.x + i,0));
                Floor.Add(new Vector2Int(RightStartingLocation.x - i,0));
            }
        }
        foreach (var FloorTile in Floor)
        {
            PaintTile(AreaMap, WallTiles, FloorTile);
        }
    }

    public IEnumerator IncrimentVirus(int Level)
    {
        //List<Vector2Int> MatchingYCords = new List<Vector2Int>();
        //MatchingYCords = Walls.ToList();

        yield return new WaitForSeconds(3.5f);

        for (int i = 0; i < 5; i++)
        {
            List<Vector2Int> WallsRef = new List<Vector2Int>(Walls);
            List<Vector2Int> MatchingYCords = new List<Vector2Int>();


            MatchingYCords = WallsRef.FindAll(i => i.y == CurrentVirusLevel);
            int LeftValue = MatchingYCords.Min(L => L.x);
            int RightValue = MatchingYCords.Max(R => R.x);

            yield return new WaitForSeconds(0.05f);

            int FillAmount = (Mathf.Abs(LeftValue) + Mathf.Abs(RightValue)+1);

            Vector2Int StartCord = new Vector2Int(LeftValue-1, Level);
            Vector2Int CurrentCord = StartCord ;

            for (int y = 0; y < FillAmount; y++)
            {
                if (CurrentCord.x >= RightValue)
                {
                    break;
                }

                CurrentCord += Vector2Int.right;
                PaintTile(VirusMap, VirusTiles, CurrentCord);
            }
            CurrentVirusLevel++;
            Level++;
        }

        if (CurrentVirusLevel < Height)
        {
            StartCoroutine(IncrimentVirus(CurrentVirusLevel));

        }
    }

    public bool TilePositionPossible(Vector2Int CurrentPosition, Vector2Int PossiblePosition, Vector2Int ChosenDirection, HashSet<Vector2Int> AllPositions)
    {
        int XValue = ChosenDirection.x;
        int YValue = ChosenDirection.y;
        bool CanGenerateHere = false;

        bool PositionClear = !AllPositions.Contains(PossiblePosition);

        //the TilePositiniong thing is causing a memory leak

        bool LeftClear;
        bool RightClear;

        //to check if the top or bottom is clear, the chosen vector will have to be passed into the bool method

        bool TopClear;
        //bool BottomClear;

        Vector2Int PositivePositionCheck;
        Vector2Int NegativePositionCheck;

        //Debug.Log(ChosenDirection+"   "+PossiblePosition+"   "+(PossiblePosition.x+ChosenDirection.x,PossiblePosition.y+ChosenDirection.y)+"    "+ AllPositions.Contains(new Vector2Int(PossiblePosition.x + ChosenDirection.x, PossiblePosition.y + ChosenDirection.y)));


        switch (XValue)
        {
            case -1:
                PositivePositionCheck = new Vector2Int(PossiblePosition.x, PossiblePosition.y + 1);
                NegativePositionCheck = new Vector2Int(PossiblePosition.x, PossiblePosition.y - 1);

                LeftClear = PositionClear && (!AllPositions.Contains(PositivePositionCheck) && !AllPositions.Contains(NegativePositionCheck));
                CanGenerateHere = LeftClear;
                break;

            case 1:

                RightClear = PositionClear && (!AllPositions.Contains(new Vector2Int(PossiblePosition.x, PossiblePosition.y - 1)) && !AllPositions.Contains(new Vector2Int(PossiblePosition.x, PossiblePosition.y + 1)));
                CanGenerateHere = RightClear;
                break;
        }


        switch (YValue)
        {
            case 1:
                TopClear = PositionClear && (!AllPositions.Contains(new Vector2Int(PossiblePosition.x - 1, PossiblePosition.y + 1)) && !AllPositions.Contains(new Vector2Int(PossiblePosition.x + 1, PossiblePosition.y + 1)));
                CanGenerateHere = TopClear;
                break;
        }

        return CanGenerateHere;
    }


    //remove return null
    public void GenerateWallPositions()
    {
        
        Vector2Int LastPosition = Vector2Int.zero;
        Vector2Int CurrentPosition = Vector2Int.zero;
        Vector2Int NextPosition = Vector2Int.zero;
        HashSet<Vector2Int> StoredPositions = new HashSet<Vector2Int>();
        Vector2Int StartingPoint = Vector2Int.zero;
        Debug.Log(Height);
        for (int y = 0; y < Height;)
        {


            if (y == 0)
            {
                CurrentPosition = new Vector2Int(AreaWidth, 0);
            }

            Vector2Int ChosenDirection = Directions[Random.Range(0, Directions.Count)];
            Vector2Int PossibleNewPosition = CurrentPosition + ChosenDirection;

            if (TilePositionPossible(NextPosition, PossibleNewPosition, ChosenDirection, Walls))
            {
                NextPosition = PossibleNewPosition;

                Walls.Add(NextPosition);
                Walls.Add(new Vector2Int((NextPosition.x * -1)-1, NextPosition.y));

                CurrentPosition = NextPosition;

                if (CurrentPosition.x > AreaWidth || CurrentPosition.x < AreaWidth) 
                {
                    Debug.Log("Warning warning warning");
                    CurrentUnitsOver = CurrentPosition.x - AreaWidth;
                    switch (CurrentUnitsOver)
                    {
                        default:
                        case 0:

                            if(!Directions.Contains(new Vector2Int(-1, 0)))
                            {
                                Directions.Add(new Vector2Int(-1, 0));
                            }
                            if(!Directions.Contains(new Vector2Int(1, 0)))
                            {
                                Directions.Add(new Vector2Int(1, 0));
                            }

                            Debug.Log("Half wolf");
                            break;

                        case 2:
                            //Directions.Remove(new Vector2Int(1, 0));
                            if(!Directions.Contains(new Vector2Int(-1, 0)))
                            {
                                Directions.Add(new Vector2Int(-1, 0));
                            }

                            Debug.Log("Half awake");
                            //Directions.Add(new Vector2Int(1, 0));
                            break;

                        case 5:
                            Directions.Remove(new Vector2Int(1, 0));
                            if (!Directions.Contains(new Vector2Int(-1, 0)))
                            {
                                Directions.Add(new Vector2Int(-1, 0));
                            }

                            Debug.Log("Half afraid");
                            break;

                        case -5:
                            Directions.Remove((new Vector2Int(-1, 0)));
                            if (!Directions.Contains(new Vector2Int(1, 0)))
                            {
                                Directions.Add(new Vector2Int(1, 0));
                            }
                            break;
                    }
                }

                if (CurrentPosition.y < Height)
                {
                    Debug.Log("know me for what i am ");

                    if (LastPosition.y <= CurrentPosition.y)
                    {
                        y--;
                        Debug.Log("the same");
                    }
                }
                else if(CurrentPosition.y >= Height)
                {
                    Debug.Log("die out");
                    y = Height - 1;
                }

                
                LastPosition = CurrentPosition;
            }

            NextPosition = CurrentPosition + ChosenDirection;


            //Walls.Add(new Vector2Int(-NextPosition.x, NextPosition.y));

            y++;
        }


        foreach (var Tile in Walls)
        {
            PaintTile(AreaMap, WallTiles, Tile);
        }

        Debug.Log(Walls.Count);

        Instantiate(CPUEscapeRef, new Vector2(0, Height), Quaternion.identity);

    }

}

//the area that the Player will be confined to is loaded onto 2 alternating parts
//one part is active and while that part is active, the previouse part will be cleared
//eg: if part 1 is active, part 2 will be cleared
