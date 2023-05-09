using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class GenerateArea : MonoBehaviour
{
    public Tilemap AreaMap;
    public TileBase WallTiles;

    [SerializeField]protected int MinWidth;
    [SerializeField]protected int MaxWidth;

    public int Height=80;
    public int DifficultyChosen = 1;

    public int AreaWidth;

    protected int TotalUnitsOver = 5;       //How many units the program is allowed to go over on the x axis 
    public int CurrentUnitsOver;

    protected List<Vector2Int> Directions = new List<Vector2Int>()
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

    public void Start()
    {
        if (MinWidth == 0)
        {
            MinWidth = 30;
        }
        if(MaxWidth == 0)
        {
            MaxWidth = 50;
        }
        PaintTile(AreaMap, WallTiles, new Vector2Int(-40,0));
        PaintTile(AreaMap, WallTiles, new Vector2Int(-20,0));
        PaintTile(AreaMap, WallTiles, new Vector2Int(40,0));

        GenerateFloor();
        GenerateWallPositions();
    }

    public void PaintTile(Tilemap ArenaGrid, TileBase TileType, Vector2Int Position)
    {
        Vector3Int TilePositon = ArenaGrid.WorldToCell((Vector3Int)Position);
        ArenaGrid.SetTile(TilePositon, TileType);

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

    public bool ValidPosition()
    {

        return false;
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
            //case -1:
            //    BottomClear = PositionClear && (!AllPositions.Contains(new Vector2Int(PossiblePosition.x - 1, PossiblePosition.y - 1)) || !AllPositions.Contains(new Vector2Int(PossiblePosition.x + 1, PossiblePosition.y - 1)));
            //    CanGenerateHere = BottomClear;
            //    break;

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

        for (int y = 0; y < Height; y++)
        {


            if (y == 0)
            {
                CurrentPosition = new Vector2Int(AreaWidth, 0);
            }

            Vector2Int ChosenDirection = Directions[Random.Range(0, Directions.Count)];
            

            NextPosition = CurrentPosition + ChosenDirection;
            CurrentPosition = NextPosition;
            Walls.Add(NextPosition);
            //Walls.Add(new Vector2Int(-NextPosition.x, NextPosition.y));
            Debug.Log(Walls.Count);

            if (CurrentPosition.x > AreaWidth)
            {
                CurrentUnitsOver = CurrentPosition.x - AreaWidth;
                switch (CurrentUnitsOver)
                {
                    default:
                        case 0:
                        Directions.Remove(new Vector2Int(-1, 0));
                        Directions.Add(new Vector2Int(1, 0));
                    break;

                    case 2:
                        //Directions.Remove(new Vector2Int(1, 0));
                        Directions.Add(new Vector2Int(-1, 0));
                        //Directions.Add(new Vector2Int(1, 0));
                    break;

                    case 5:
                        Directions.Remove(new Vector2Int(-1, 0));
                        Directions.Add(new Vector2Int(1, 0));
                   break;
                }
            }

            if(LastPosition.y==CurrentPosition.y)
            {
                y--;
            }
            y++;
            LastPosition = CurrentPosition;
        }


        foreach (var Tile in Walls)
        {
            PaintTile(AreaMap, WallTiles, Tile);
        }

    }

}

//the area that the player will be confined to is loaded onto 2 alternating parts
//one part is active and while that part is active, the previouse part will be cleared
//eg: if part 1 is active, part 2 will be cleared
