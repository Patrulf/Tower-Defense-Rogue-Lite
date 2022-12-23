using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OccupiedState
{
    //Open,
    //Closed,

    //TODO: split Closed into buildable and unbuildable?



    //Problems with this?
    //Idk let's just do it this way.

    Path,          //PATH. //Is unbuildable but for pathfinding considered as 'open'
    Buildable,     //BUILDABLE. //is buildable but for pathfinding considered as 'closed'.
    UnBuildable,   //UNBUILDABLE. // isUnBuildable and for pathfinding considered as 'closed'.




}

//TODO: look into making the data into a scriptable object.
[CreateAssetMenu(fileName = "GameGrid", menuName = "Grid")]
public class GameGrid : ScriptableObject
{
    //[SerializeField]
    //private GameGridData data;

    [SerializeField]
    private float xSize;
    [SerializeField]
    private float ySize;
    [SerializeField]
    private int xCells;
    [SerializeField]
    private int yCells;

    private OccupiedState[] tileStates;


    public void init()
    {
        tileStates = new OccupiedState[xCells * yCells];

        for (int i = 0; i < xCells * yCells; i++)
        {
            tileStates[i] = OccupiedState.UnBuildable;
        }
    }

    //Todo: could perhaps be an event? for now we'll just keep as is though.
    public void onNewLevelModuleGenerated(LevelModule levelModule, Vector2Int gridPosition)
    {
        /*if (tileStates == null)
        {
            tileStates = new OccupiedState[xCells * yCells];
        }*/

        for (int i = 0; i < levelModule.tiles.Length; i++)
        {
            int yIndex = gridPosition.y * LevelModuleData.height - Mathf.FloorToInt(LevelModuleData.height * 0.5f) + i % LevelModuleData.width; //think 
            int xIndex = gridPosition.x * LevelModuleData.width - Mathf.FloorToInt(LevelModuleData.width * 0.5f) + Mathf.FloorToInt(i / LevelModuleData.width);
            if (!levelModule.tiles[i].isPathable)
            {
                this.SetTileOccupationCartesian(xIndex, yIndex, OccupiedState.Buildable);
            } else
            {
                this.SetTileOccupationCartesian(xIndex, yIndex, OccupiedState.Path);
            }
        }


    }

    //fake isometric conversion.
    public Vector2 cartesianToIsometric(Vector2 p)
    {
        //move on x positive.
        //(add 1x to x.
        //add 0.5 to y.) ---x

        //move on y positive.
        //(subtract 1x to x.
        //add 0.5 to y.) ---y

        //therefore xIso = 1x - 1y
        //and therefore yIso = 0.5x + 0.5y.
        return new Vector2(p.x - p.y, (p.x + p.y)*0.5f);
    }

    public Vector2 isometricToCartesian(Vector2 p)
    {
        //move on x positive.
        //xIso = xCart - yCart
        //yIso = (xCart + yCart)*0.5

        //Final values.
        //xCart = xIso/2 + yIso
        //yCart = yIso - xIso/2

        return new Vector2(p.x*0.5f + p.y, p.y-p.x*0.5f);
    }


    public OccupiedState getTileOccupationIsometric(float x, float y)
    {
        Vector2Int cartesian = Vector2Int.FloorToInt(isometricToCartesian(new Vector2(x, y)));
        return tileStates[cartesian.x + xCells * cartesian.y];
    }
    public OccupiedState getTileOccupationIsometric(Vector2 p_position)
    {
        return getTileOccupationIsometric(p_position.x, p_position.y);
    }
    public OccupiedState getTileOccupation(int x, int y) //converting from isometric coords to cartesian.
    {
        return tileStates[x + xCells * y];
    }
    public OccupiedState getTileOccupation(Vector2Int p_position)
    {
        return getTileOccupation(p_position.x, p_position.y);
    }

    public void SetTileOccupationIsometric(int x, int y, OccupiedState p_state)
    {
        Vector2Int cartesian = Vector2Int.FloorToInt(isometricToCartesian(new Vector2(x, y)));

        tileStates[cartesian.x + xCells * cartesian.y] = p_state;
    }

    public void SetTileOccupationCartesian(int x, int y, OccupiedState p_state)
    {
        tileStates[x + xCells * y] = p_state;
    }



    public bool isValidTilePositionIsometric(float x, float y)
    {
        Vector2Int cartesian = Vector2Int.FloorToInt(isometricToCartesian(new Vector2(x, y)));
        bool isWithinXBounds = cartesian.x >= 0 && cartesian.x < this.xCells ? true : false;
        bool isWithinYBounds = cartesian.y >= 0 && cartesian.y < this.yCells ? true : false;
        return isWithinXBounds && isWithinYBounds;
    }
    public bool isValidTilePositionIsometric(Vector2 p_position)
    {
        return isValidTilePositionIsometric(p_position.x, p_position.y);
    }

    public bool isValidTilePosition(int x, int y) //Used cartesian coords.
    {
        bool isWithinXBounds = x >= 0 && x < this.xCells ? true : false;
        bool isWithinYBounds = y >= 0 && y < this.yCells ? true : false;
        return (isWithinXBounds && isWithinYBounds);
    }

    public bool isValidTilePosition(Vector2Int p_position)
    {
        return isValidTilePosition(p_position.x, p_position.y);
    }

    public bool isValidTilePosition(Vector2 p_position)
    {
        Vector2Int position = Vector2Int.FloorToInt(p_position);
        return isValidTilePosition(position.x, position.y);
    }

    public Vector2Int getDimensions()
    {
        return new Vector2Int(xCells, yCells);
    }
}

