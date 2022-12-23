using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ModuleConnection {
    N,
    W,
    E,
    S,
    NW,
    NE,
    NS,
    SE,
    SW,
    WE, //these are single connection, i guess we'll have to do double connections too.
}

public enum Direction
{
    NULL,
    N,
    W,
    S,
    E,
}


[CreateAssetMenu(fileName = "Level", menuName = "LevelModule")]
[System.Serializable]
public class LevelModule : ScriptableObject
{

    [SerializeField] private const int width = LevelModuleData.width;
    [SerializeField] private const int height = LevelModuleData.height;

    //[HideInInspector]
    [SerializeField] public GameTile[] tiles = new GameTile[width*height];

    [SerializeField] public ModuleConnection connection;


    public static List<Direction> getValidDirections(LevelModule module)
    {
        List<Direction> directions = new List<Direction>();
        //Todo: will need a grid for our world generation submodules too. will need to check if those are occupied or not.

        switch(module.connection)
        {
            case ModuleConnection.N:
                directions.Add(Direction.N);
                break;
            case ModuleConnection.S:
                directions.Add(Direction.S);
                break;
            case ModuleConnection.E:
                directions.Add(Direction.E);
                break;
            case ModuleConnection.W:
                directions.Add(Direction.W);
                break;
            case ModuleConnection.NE:
                directions.Add(Direction.E);
                directions.Add(Direction.N);
                break;
            case ModuleConnection.NW:
                directions.Add(Direction.W);
                directions.Add(Direction.N);
                break;
            case ModuleConnection.NS:
                directions.Add(Direction.N);
                directions.Add(Direction.S);
                break;
            case ModuleConnection.SW:
                directions.Add(Direction.S);
                directions.Add(Direction.W);
                break;
            case ModuleConnection.SE:
                directions.Add(Direction.S);
                directions.Add(Direction.E);
                break;
            case ModuleConnection.WE:
                directions.Add(Direction.W);
                directions.Add(Direction.E);
                break;
            default:
                Debug.LogError("No valid direction.");
                break;
        }
        return directions;

    }



}
