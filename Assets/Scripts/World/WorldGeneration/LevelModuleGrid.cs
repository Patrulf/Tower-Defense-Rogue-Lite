using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelModuleInstanceData
{
    public Vector2Int position { get; private set; }
    public LevelModule levelModule { get; private set; }
    public LevelModuleInstanceData(Vector2Int p_position,LevelModule p_levelModule)
    {
        position = new Vector2Int(p_position.x,p_position.y);
        levelModule = p_levelModule;
    }
} 
public class LevelModuleGrid
{
    public GameGrid referenceGrid;



    private Dictionary<Vector2Int, LevelModule> currentlyAvailable;
    private Dictionary<Vector2Int, bool> occupiedList;

    public LevelModuleGrid(GameGrid referenceGrid)
    {
        this.referenceGrid = referenceGrid;
        this.currentlyAvailable = new Dictionary<Vector2Int, LevelModule>();
        this.occupiedList = new Dictionary<Vector2Int, bool>();
    }

    private Vector2 convertToRealCoordinates(Vector2Int gridPosition)
    {
        Vector2Int dimensions = referenceGrid.getDimensions();
        return new Vector2(gridPosition.x * (dimensions.x / LevelModuleData.width) - LevelModuleData.width * 0.5f, gridPosition.y * (dimensions.x / LevelModuleData.height) - LevelModuleData.height * 0.5f);
    }

    public void setCurrentlyAvailableLevelModule(Vector2Int gridPosition, LevelModule levelModule)
    {
        Debug.Log("AvailablePosition set at: " + gridPosition.x + "/" + gridPosition.y + "Total Elements: " + this.currentlyAvailable.Count);
        currentlyAvailable.Add( gridPosition, levelModule );
    }

    public bool removeAvailableLevelModule(Vector2Int gridPosition)
    {
        Debug.Log("AvailablePosition Removed at: " + gridPosition.x + "/" + gridPosition.y + "Total Elements: " + this.currentlyAvailable.Count);
        return this.currentlyAvailable.Remove(gridPosition);
    }

    public List<LevelModuleInstanceData> getAvailableLevelModules()
    {
        List<LevelModuleInstanceData> data = new List<LevelModuleInstanceData>();
        foreach (KeyValuePair<Vector2Int, LevelModule> keyValuePair in this.currentlyAvailable)
        {
            LevelModuleInstanceData instance = new LevelModuleInstanceData(keyValuePair.Key, keyValuePair.Value);
            data.Add(instance);
        }
        return data;
    }

    public void setOccupied(Vector2Int gridPosition,bool state)
    {
        Vector2Int dimensions = referenceGrid.getDimensions();
        Vector2 realPosition = convertToRealCoordinates(gridPosition);
        if (realPosition.x < 0 || realPosition.y < 0 || realPosition.x > dimensions.x || realPosition.y > dimensions.y)
        {
            Debug.LogError("Action may not be performed. Outside of bounds.");
        }
        occupiedList[gridPosition] = state;
    }

    public bool getOccupied(Vector2Int gridPosition)
    {
        Vector2Int dimensions = referenceGrid.getDimensions();
        Vector2 realPosition = convertToRealCoordinates(gridPosition);
        if (realPosition.x < 0 ||realPosition.y < 0 || realPosition.x > dimensions.x || realPosition.y > dimensions.y )
        {
            return true;
        }
        if (!occupiedList.ContainsKey(gridPosition))
        {
            return false;
        } else
        {
            return occupiedList[gridPosition];
        }
    }



    
    public List<ModuleConnection> GetValidConnections(Direction direction, Vector2Int atPosition)
    {
        List<ModuleConnection> connections = new List<ModuleConnection>();
        Vector2Int southPosition = new Vector2Int(atPosition.x, atPosition.y - 1);
        Vector2Int northPosition = new Vector2Int(atPosition.x, atPosition.y + 1);
        Vector2Int westPosition = new Vector2Int(atPosition.x - 1, atPosition.y );
        Vector2Int eastPosition = new Vector2Int(atPosition.x + 1, atPosition.y);


        switch (direction)
        {
            case Direction.N: //we will skip returning n for n and s for s etc, these are not really useful for our purposes ever.
                if (!this.getOccupied(westPosition) )
                {
                    connections.Add(ModuleConnection.SW);
                }
                if (!this.getOccupied(eastPosition))
                {
                    connections.Add(ModuleConnection.SE);
                }
                if (!this.getOccupied(northPosition))
                {
                    connections.Add(ModuleConnection.NS);
                }
                break;
            case Direction.W:
                if (!this.getOccupied(southPosition))
                {
                    connections.Add(ModuleConnection.SE);
                }
                if (!this.getOccupied(westPosition))
                {
                    connections.Add(ModuleConnection.WE);
                }
                if (!this.getOccupied(northPosition))
                {
                    connections.Add(ModuleConnection.NE);
                }
                break;
            case Direction.S:
                if (!this.getOccupied(southPosition))
                {
                    connections.Add(ModuleConnection.NS);
                }
                if (!this.getOccupied(eastPosition))
                {
                    connections.Add(ModuleConnection.NE);
                }
                if (!this.getOccupied(westPosition))
                {
                    connections.Add(ModuleConnection.NW);
                }
                break;
            case Direction.E:
                if (!this.getOccupied(southPosition))
                {
                    connections.Add(ModuleConnection.SW);
                }
                if (!this.getOccupied(eastPosition))
                {
                    connections.Add(ModuleConnection.WE);
                }
                if (!this.getOccupied(northPosition))
                {
                    connections.Add(ModuleConnection.NW);
                }
                break;

            default:
                Debug.LogError("No such Module Connection exists.");
                break;
        }


        if (connections.Count == 0)
        {
            Debug.LogError("NO CONNECTIONS AVAILABLE");
        }
        return connections;

    }




}
