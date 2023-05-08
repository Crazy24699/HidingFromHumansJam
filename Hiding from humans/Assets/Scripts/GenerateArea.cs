using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class GenerateArea : MonoBehaviour
{
    public Tilemap WallMap1;
    public Tilemap WallMap2;

    public TileBase WallTile;

    public HashSet<Vector2Int> AreaTileCords = new HashSet<Vector2Int>();       //Gets the current cords of all the tiles in that area being generated

    public Vector2Int EndingPoint;
    public List<Vector2> GenerationDirections = new List<Vector2>()
    {
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
    };

    public int ChosenDirection;         //gets the value of the chosen direction 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void GenerateFloor()
    {

    }

    public void PaintTile(Tilemap Map, TileBase TileRef, Vector2Int Position)
    {
        Vector3Int TilePosition = Map.WorldToCell((Vector3Int)Position);
        Map.SetTile(TilePosition, TileRef);
    }

    public bool PossibleTilePosition(Vector2Int CurrentPosition, Vector2Int PossiblePosition, Vector2Int ChosenDirection, HashSet<Vector2Int> AllPositions)
    {
        int CheckXValue = ChosenDirection.x;
        int CheckYValue = ChosenDirection.y;

        bool ValidPosition = false;

        bool EmptyTile = !AllPositions.Contains(CurrentPosition);

        bool RightClear;
        bool LeftClear;
        bool TopClear;

        Vector2Int PositivePositionCheck;
        Vector2Int NegativePositionCheck;

        switch (CheckXValue)
        {
            case -1:
                PositivePositionCheck = new Vector2Int(PossiblePosition.x, PossiblePosition.y + 1);
                NegativePositionCheck = new Vector2Int(PossiblePosition.x, PossiblePosition.y - 1);

                LeftClear = EmptyTile && (!AllPositions.Contains(PositivePositionCheck) && !AllPositions.Contains(NegativePositionCheck));
                ValidPosition = LeftClear;
                break;

            case 1:

                RightClear = EmptyTile && (!AllPositions.Contains(new Vector2Int(PossiblePosition.x, PossiblePosition.y - 1)) && !AllPositions.Contains(new Vector2Int(PossiblePosition.x, PossiblePosition.y + 1)));
                ValidPosition = RightClear;
                break;
        }

        switch (CheckYValue)
        {
            case 1:
                TopClear = EmptyTile && (!AllPositions.Contains(new Vector2Int(PossiblePosition.x - 1, PossiblePosition.y + 1)) && !AllPositions.Contains(new Vector2Int(PossiblePosition.x + 1, PossiblePosition.y + 1)));
                ValidPosition = TopClear;
                break;
        }

        return ValidPosition;
    }

    public void GenerateNewChunk()
    {
            
    }

    public void GenerateChunk(Vector2Int LeftEndPoint,Vector2Int RightEndPoint)
    {
        Vector2Int Ral=new Vector2Int(0,0);
        switch (ChosenDirection)
        {
            case new Vector2Int(0,0):
                break;
        }
    }

    

}

//the area that the player will be confined to is loaded onto 2 alternating parts
//one part is active and while that part is active, the previouse part will be cleared
//eg: if part 1 is active, part 2 will be cleared
