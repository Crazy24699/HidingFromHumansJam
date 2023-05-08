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
    //scripts


    public Tilemap ArenaMap;
    public Tilemap InteriorMap;
    public Tilemap ExteriorMap;
    public Tilemap PlatformMap;

    //tile bases
    [Header("Tiles"), Space(5)]
    public TileBase ArenaExteriorTile;
    public TileBase ArenaWallTile;
    public TileBase[] ArenaInteriorTile;
    public TileBase PlatformPoints;

    [Space(5)]
    public TilemapCollider2D ArenaCollider;


    //ints
    [Tooltip("How many times the wall will generate a tile")] public int WallTileGeneration;
    public int MinLength;
    public int MaxLength;
    [HideInInspector] public int Height;
    [HideInInspector] public int Width;
    [HideInInspector] public int ExteriorLength = 20;


    //the name of the list that will be added to the public nested list
    string ListName = "Default";


    //vectors 
    //normal vectors 
    [HideInInspector] public Vector2Int RightStartPoint;
    [HideInInspector] public Vector2Int LeftStartPoint;

    //hashsets
    public HashSet<Vector2Int> SurroundingArea = new HashSet<Vector2Int>();
    HashSet<Vector2Int> ExteriorGrowPoints = new HashSet<Vector2Int>();

    //dictionaries
    public Dictionary<string, List<Vector2Int>> ArenaExterior = new Dictionary<string, List<Vector2Int>>();
    public Dictionary<string, List<Vector2Int>> ArenaSides = new Dictionary<string, List<Vector2Int>>();

    //lists 
    protected List<Vector2Int> SideVariables = new List<Vector2Int>();
    protected List<Vector2Int> InteriorSpace = new List<Vector2Int>();
    protected List<Vector2Int> CavernRoof = new List<Vector2Int>();
    [HideInInspector] public List<Vector2Int> ArenaExteriorShell = new List<Vector2Int>();
    public List<Vector2Int> RightSide = new List<Vector2Int>();
    public List<Vector2Int> LeftSide = new List<Vector2Int>();

    public List<Vector2Int> Directions = new List<Vector2Int>()
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
        new Vector2Int(0,1),
    };


    public void Clearall()
    {
        ArenaMap.ClearAllTiles();
        InteriorMap.ClearAllTiles();
        PlatformMap.ClearAllTiles();
        ExteriorMap.ClearAllTiles();
        SurroundingArea.Clear();
        ExteriorGrowPoints.Clear();
        SideVariables.Clear();
        ArenaExterior.Clear();
        InteriorSpace.Clear();
        CavernRoof.Clear();
        ArenaExteriorShell.Clear();
        RightSide.Clear();
        RightSide.Clear();
        RightSide.Clear();
        RightSide.Clear();

        LeftSide.Clear();
    }

    public void ExecutionOrder()
    {

        HandleExterior();

        FillOuterWorld();
        HandleInterior();


    }

    public void PaintTile(Tilemap ArenaGrid, TileBase TileType, Vector2Int Position)
    {
        Vector3Int TilePositon = ArenaGrid.WorldToCell((Vector3Int)Position);
        ArenaGrid.SetTile(TilePositon, TileType);

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


    //generates the top and bottom of the cavern
    public HashSet<Vector2Int> GenerateCavernTop(bool GoingRight, int HightestYPoint, Vector2Int TileStartPoint)
    {
        //GoingRight        what direction the generation will take place.
        //HighestYPoint     the highest y point that the program needs to converge at to make the cavern roof
        //TileStartPoint        the starting cord of where the generation to the top of the cavern
        //StoredPositions        all the positions currently in the scene and the positions that will be returned 

        //All the existing positions that cant be overwritten
        HashSet<Vector2Int> Positions = new HashSet<Vector2Int>();

        List<Vector2Int> DirectionOptions = new List<Vector2Int>();



        Vector2Int PossibleNewTile;
        PossibleNewTile = TileStartPoint;


        DirectionOptions.Clear();

        //int YDistance;      //Distance from the end point on the y axis
        int XDistance;      //Distance from the end point on the x axis

        XDistance = Mathf.Abs(TileStartPoint.x);

        switch (GoingRight)
        {

            case true:

                DirectionOptions.Add(new Vector2Int(1, 0));
                DirectionOptions.Add(new Vector2Int(1, 0));
                DirectionOptions.Add(new Vector2Int(1, 1));

                break;

            case false:
                DirectionOptions.Add(new Vector2Int(-1, 0));
                DirectionOptions.Add(new Vector2Int(-1, 0));
                DirectionOptions.Add(new Vector2Int(-1, 1));

                break;
        }
        DirectionOptions.Remove(new Vector2Int(0, -1));


        int DistanceRemaining = XDistance;

        for (int i = 0; i < XDistance; i++)
        {


            int Direction = Random.Range(0, DirectionOptions.Count);
            PossibleNewTile = PossibleNewTile + DirectionOptions[Direction];

            DistanceRemaining--;
            if (PossibleNewTile.y >= HightestYPoint)
            {

                DirectionOptions.Remove(new Vector2Int(0, 1));
                DirectionOptions.Remove(new Vector2Int(1, 1));
                DirectionOptions.Remove(new Vector2Int(-1, 1));

                int HightDifference;
                HightDifference = PossibleNewTile.y - HightestYPoint;
                PossibleNewTile = new Vector2Int(PossibleNewTile.x, PossibleNewTile.y - HightDifference);

                if (DistanceRemaining > 15)
                {
                    switch (GoingRight)
                    {
                        case true:

                            DirectionOptions.Add(new Vector2Int(1, -1));
                            break;

                        case false:
                            DirectionOptions.Add(new Vector2Int(-1, -1));
                            break;
                    }
                }
            }


            if (DistanceRemaining < 15 && PossibleNewTile.y < HightestYPoint)
            {
                DirectionOptions.Remove(new Vector2Int(1, -1));
                DirectionOptions.Remove(new Vector2Int(-1, -1));

                switch (GoingRight)
                {
                    case true:

                        DirectionOptions.Add(new Vector2Int(1, 1));
                        DirectionOptions.Add(new Vector2Int(1, 1));
                        break;

                    case false:
                        DirectionOptions.Add(new Vector2Int(-1, 1));
                        DirectionOptions.Add(new Vector2Int(-1, 1));
                        break;
                }

            }



            if (PossibleNewTile.y < HightestYPoint - 3)
            {
                switch (GoingRight)
                {
                    case true:

                        DirectionOptions.Remove(new Vector2Int(1, -1));
                        break;

                    case false:
                        DirectionOptions.Remove(new Vector2Int(-1, -1));
                        break;
                }
            }


            Positions.Add(PossibleNewTile);

            ArenaExteriorShell.Add(PossibleNewTile);
            ExteriorGrowPoints.Add(new Vector2Int(PossibleNewTile.x, PossibleNewTile.y + 1));
            SideVariables.Add(new Vector2Int(PossibleNewTile.x, PossibleNewTile.y + 1));

        }

        return Positions;
    }



    //Generates the positions of the walls, cliff faces and everything is handled by this and has to be seperated to get the highest y Value for the roof
    public HashSet<Vector2Int> GenerateWallPositions(bool FavourRight)
    {
        int MinHeight = Random.Range(30, 40);

        HashSet<Vector2Int> Position = new HashSet<Vector2Int>();

        List<Vector2Int> DirectionOptions = new List<Vector2Int>();

        DirectionOptions.Clear();

        //the added value onto which the grow points on the exterior will be added, based on the bool argument, if it favours right then itll go right, left with left, etc
        int AddedValue;

        ExteriorGrowPoints.Clear();

        Vector2Int OldPosition = Vector2Int.zero;
        Vector2Int NewPosition = Vector2Int.zero;
        if (Position.Count <= 0)
        {
            for (int i = 0; i < ArenaExteriorShell.Count; i++)
            {
                Position.Add(ArenaExteriorShell[i]);
            }
        }


        switch (FavourRight)
        {
            case true:

                NewPosition = RightStartPoint;
                //why does directions not exist in the code below??????
                AddedValue = 1;
                ListName = "RightSide";
                for (int i = 0; i < 3; i++)
                {
                    DirectionOptions.Add(new Vector2Int(1, 0));
                    DirectionOptions.Add(new Vector2Int(0, 1));
                }
                break;



            case false:

                NewPosition = LeftStartPoint;
                DirectionOptions = Directions.ToList();
                AddedValue = -1;
                ListName = "LeftSide";
                for (int i = 0; i < 3; i++)
                {
                    DirectionOptions.Add(new Vector2Int(-1, 0));
                    DirectionOptions.Add(new Vector2Int(0, 1));
                }
                break;

        }


        for (int i = 0; i < WallTileGeneration; i++)
        {
            int DirectionChosen = Random.Range(0, DirectionOptions.Count);
            Vector2Int PossibleNewPositon = NewPosition + DirectionOptions[DirectionChosen];

            if (TilePositionPossible(NewPosition, PossibleNewPositon, DirectionOptions[DirectionChosen], Position))
            {

                NewPosition = PossibleNewPositon;

                Position.Add(NewPosition);
                ArenaExteriorShell.Add(NewPosition);

                if ((NewPosition.y > OldPosition.y))
                {
                    ExteriorGrowPoints.Add(new Vector2Int(NewPosition.x + AddedValue, NewPosition.y));

                    OldPosition = NewPosition;
                }




                if ((NewPosition.x > OldPosition.x && FavourRight) || (NewPosition.x < OldPosition.x && !FavourRight))
                {
                    ExteriorGrowPoints.Remove(new Vector2Int(OldPosition.x + AddedValue, OldPosition.y));
                    ExteriorGrowPoints.Add(new Vector2Int(NewPosition.x + AddedValue, NewPosition.y));

                    OldPosition = NewPosition;
                }

                PaintTile(ArenaMap, ArenaWallTile, NewPosition);

                if (FavourRight)
                {
                    RightSide.Add(NewPosition);
                }
                else
                {
                    LeftSide.Add(NewPosition);
                }

            }
            else if (!TilePositionPossible(NewPosition, PossibleNewPositon, DirectionOptions[DirectionChosen], Position))
            {
                i--;

            }
        }

        //ExternalArenaSides.Add(SideVariables);
        ArenaExterior.Add(ListName, ExteriorGrowPoints.ToList());

        return Position;

    }


    public Vector2Int FindHighestYPoint(Vector2Int FirstVector, Vector2Int SecondVector)
    {
        Vector2Int HighestYPoint = Vector2Int.zero;

        if (FirstVector.y > SecondVector.y)
        {
            HighestYPoint = FirstVector;
        }
        if (FirstVector.y < SecondVector.y)
        {
            HighestYPoint = SecondVector;
        }
        if (SecondVector.y == FirstVector.y)
        {
            HighestYPoint = SecondVector;
        }

        return HighestYPoint;
    }


    public void FillOuterWorld()
    {

        HashSet<Vector2Int> ExistingCords;

        for (int i = 0; i < ArenaExterior.Count; i++)
        {

            ExistingCords = new HashSet<Vector2Int>(ArenaExterior.ElementAt(i).Value);

            Vector2Int MovingDirection = Vector2Int.zero;

            switch (ArenaExterior.ElementAt(i).Key)
            {
                //case 0 will be moving to the Left
                case "LeftSide":
                    MovingDirection = new Vector2Int(-1, 0);
                    break;

                //case 1 will be moving right
                case "RightSide":
                    MovingDirection = new Vector2Int(1, 0);
                    break;

                //case 2 will be moving upwards
                case "CavernRoof":
                    MovingDirection = new Vector2Int(0, 1);
                    break;
            }


            foreach (var Cord in ExistingCords)
            {

                Vector2Int BaseCord = Cord;
                for (int x = 0; x < ExteriorLength; x++)
                {

                    BaseCord += MovingDirection;
                    SurroundingArea.Add(BaseCord);


                }
            }

        }

        foreach (var cord in SurroundingArea)
        {
            PaintTile(ExteriorMap, ArenaExteriorTile, cord);
        }

    }


    public void HandleExterior()
    {
        int RightLenth = Random.Range(MinLength, MaxLength);
        int LeftLength = Random.Range(-MaxLength, -MinLength);
        RightStartPoint = new Vector2Int(RightLenth, 0);
        LeftStartPoint = new Vector2Int(LeftLength, 0);

        ArenaExteriorShell.Add(RightStartPoint);
        ArenaExteriorShell.Add(LeftStartPoint);
        ArenaExteriorShell.Add(Vector2Int.zero);

        int XFillDistance;
        XFillDistance = Mathf.Abs(LeftStartPoint.x) + RightStartPoint.x;
        for (int i = 0; i < XFillDistance; i++)
        {
            Vector2Int AddedFloorCord = new Vector2Int(LeftStartPoint.x + i, LeftStartPoint.y);
            ArenaExteriorShell.Add(AddedFloorCord);
        }
        Width = Mathf.Abs(LeftLength) + RightLenth;



        GenerateWallPositions(true);
        GenerateWallPositions(false);

        Vector2Int RightPoint = RightSide[RightSide.Count - 1];
        ArenaSides.Add("RightSide", RightSide);

        Vector2Int LeftPoint = LeftSide[LeftSide.Count - 1];
        ArenaSides.Add("LeftSide", LeftSide);


        int HighestYPoint = FindHighestYPoint(RightPoint, LeftPoint).y;
        Height = HighestYPoint + 5;

        ArenaExteriorShell.Add(new Vector2Int(0, HighestYPoint + 5));

        ExteriorGrowPoints.Clear();
        List<Vector2Int> LeftCords = new List<Vector2Int>();

        LeftCords = GenerateCavernTop(true, Height, LeftPoint).ToList();
        CavernRoof = GenerateCavernTop(false, Height, RightPoint).ToList();
        ArenaExterior.Add("CavernRoof", ExteriorGrowPoints.ToList());

        CavernRoof.AddRange(LeftCords);
        ArenaSides.Add("CavernRoof", CavernRoof.ToList());

        for (int i = 0; i < ArenaExteriorShell.Count; i++)
        {
            PaintTile(ArenaMap, ArenaWallTile, ArenaExteriorShell[i]);
        }


        for (int x = 0; x < ArenaExterior.Count; x++)
        {
            foreach (var PaintCord in ArenaExterior.ElementAt(x).Value)
            {
                PaintTile(ArenaMap, ArenaExteriorTile, PaintCord);
            }
        }

       
    }


    public void HandleInterior()
    {

        //how much distance needs to be filled by locations
        int FillDistance = 0;

        //a vector 2 to get the value to 0 in a specific direction
        Vector2Int Direction = Vector2Int.zero;

        Vector2Int RoofStopPoint = Vector2Int.Min(LeftSide[LeftSide.Count - 1], RightSide[RightSide.Count - 1]);

        HashSet<Vector2Int> UsedSpace = new HashSet<Vector2Int>(ArenaExteriorShell);
        UsedSpace.UnionWith(ArenaExteriorShell);
        HashSet<Vector2Int> TotalInteriorSpace = new HashSet<Vector2Int>();


        for (int x = 0; x < ArenaSides.Count; x++)
        {
            switch (ArenaSides.ElementAt(x).Key)
            {
                case "LeftSide":
                    Direction = new Vector2Int(1, 0);
                    break;

                case "RightSide":

                    Direction = new Vector2Int(-1, 0);
                    break;

                case "CavernRoof":

                    Direction = new Vector2Int(0, -1);
                    break;
            }

            foreach (var Point in ArenaSides.ElementAt(x).Value)
            {

                if (Direction == new Vector2Int(0, -1))
                {
                    FillDistance = Mathf.Abs(Point.y - RoofStopPoint.y);
                }
                if (Direction != new Vector2Int(0, -1))
                {
                    FillDistance = Mathf.Abs(Point.x);
                }



                Vector2Int FilledCord = Vector2Int.zero;

                for (int i = 0; i < FillDistance; i++)
                {
                    switch (i)
                    {
                        case 0:
                            FilledCord = Point + Direction;
                            break;

                        case > 0:
                            FilledCord = FilledCord + Direction;
                            break;
                    }

                    if (!UsedSpace.Contains(FilledCord) && !ArenaExterior.ElementAt(0).Value.Contains(FilledCord) && !ArenaExterior.ElementAt(1).Value.Contains(FilledCord))
                    {
                        int RandomSelection = Random.Range(0, ArenaInteriorTile.Length);
                        PaintTile(InteriorMap, ArenaInteriorTile[RandomSelection], FilledCord);
                        TotalInteriorSpace.Add(FilledCord);
                    }
                    UsedSpace.Add(FilledCord);

                }

            }

        }


    }



}

//the area that the player will be confined to is loaded onto 2 alternating parts
//one part is active and while that part is active, the previouse part will be cleared
//eg: if part 1 is active, part 2 will be cleared
