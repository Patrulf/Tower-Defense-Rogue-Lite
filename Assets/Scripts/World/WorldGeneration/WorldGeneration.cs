using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Pool;
using UnityEngine.UI;
using System.Linq;

public class ButtonData
{
    public GameObject button;
    public Vector2 position;
    public ButtonData(GameObject p_button, Vector2 p_position) //position is not to screen coords.
    {
        button = p_button;
        position = p_position;
    }

}



[RequireComponent(typeof(Tilemap), typeof(Grid), typeof(TilemapRenderer))]
public class WorldGeneration : MonoBehaviour
{

    public GameGrid gameGrid;
    private LevelModuleGrid levelModuleGrid;

    public LevelModule[] singleModules;
    public LevelModule[] startModules;

    private Tilemap tilemap;

    public GameObject buttonPrefab;
    private ObjectPool<GameObject> buttonPool;

    private List<ButtonData> activeButtons; //maybe do a dictionary of some kind later on.

    private bool hasCameraMoved = false;

    private void Awake()
    {
        this.levelModuleGrid = new LevelModuleGrid(this.gameGrid);
        this.gameGrid.init();

        activeButtons = new List<ButtonData>();
        GameObject canvasObject = null;
        if (GetComponent<Canvas>() != null)
        {
            canvasObject = this.gameObject;
           //Create a new gameobject with a canvas here.
        } else if (GetComponentInChildren<Canvas>() != null)
        {
            canvasObject = GetComponentInChildren<Canvas>().gameObject;
        } else
        {
            Debug.LogError("Add a canvas to this gameObject");
        }

        Vector2Int dimensions = gameGrid.getDimensions();
        Vector2 isometricPosition = MathHelper.cartesianToIsometric(dimensions.x * 0.5f, dimensions.y * 0.5f);
        Camera.main.transform.position = new Vector3(isometricPosition.x, isometricPosition.y, Camera.main.transform.position.z);

        buttonPool = new ObjectPool<GameObject>( ()=>{ 
            GameObject gameObject = Instantiate(buttonPrefab);
            gameObject.transform.SetParent(canvasObject.transform);
            return gameObject;
        },
        entity => {
            entity.SetActive(true);
        },
        entity => {
            entity.SetActive(false);
            },
        entity =>
        {
            Destroy(entity);
        }
        ,true,10);

        tilemap = GetComponent<Tilemap>();
        this.generateStartPosition();


    }

    private void Start()
    {
        GameEvents.instance.onCameraMoved += onCameraMoved;
    }


    private void onRoundEnded()
    {
        List<LevelModuleInstanceData> instanceDatas = levelModuleGrid.getAvailableLevelModules();
        foreach (LevelModuleInstanceData data in instanceDatas)
        {
            Debug.Log("data gridposition: " + data.position.x + "/" + data.position.y);
            LevelModule module = data.levelModule;
            Vector2Int gridPosition = data.position;
            LevelModule currentModule = module;
            List<Direction> directions = LevelModule.getValidDirections(currentModule);


            this.generateButtons(gridPosition, directions);
        }
    }


    private void updateAvailablePositions(LevelModule newModule,Vector2Int fromGridPosition,Vector2Int toGridPosition)
    {
        if (!this.levelModuleGrid.removeAvailableLevelModule(fromGridPosition) )
        {
            Debug.LogError("Position was not removed when trying to update available positions at position: " + fromGridPosition.x + "/" +fromGridPosition.y);
        }
        this.levelModuleGrid.setOccupied(toGridPosition, true);
        this.levelModuleGrid.setCurrentlyAvailableLevelModule(toGridPosition, newModule);
    }


    public void LateUpdate()
    {
            for (int i = 0; i < this.activeButtons.Count; i++)
            {
                this.activeButtons[i].button.transform.position = Camera.main.WorldToScreenPoint(this.activeButtons[i].position);
            }
    }

    public void onCameraMoved()
    {
        hasCameraMoved = true;
    }




    private void generateButtons(Vector2Int gridPosition, List<Direction> availableDirections)
    {
        //create a button with callback to generate new submodule on click.

        for (int i = 0; i < availableDirections.Count; i++)
        {
            float xOffset = 0.0f;
            float yOffset = 0.0f;
            int nextGridPositionX = 0;
            int nextGridPositionY = 0;
            switch(availableDirections[i])
            {
                case Direction.N:
                    yOffset = LevelModuleData.height * 0.5f;
                    nextGridPositionY = 1;
                    break;
                case Direction.E:
                    xOffset = LevelModuleData.width * 0.5f;
                    nextGridPositionX = 1;
                    break;
                case Direction.S:
                    yOffset = -LevelModuleData.height * 0.5f;
                    nextGridPositionY = -1;
                    break;
                case Direction.W:
                    xOffset = -LevelModuleData.width * 0.5f;
                    nextGridPositionX = -1;
                    break;
                default:
                    Debug.LogError("invalid direction");
                    break;
            }

            if (nextGridPositionX == 0 && nextGridPositionY == 0)
            {
                Debug.LogError("no direction");
            }

            Vector2Int nextPosition = new Vector2Int(gridPosition.x + nextGridPositionX, gridPosition.y + nextGridPositionY);
            if (this.levelModuleGrid.getOccupied(nextPosition))
            {
                continue; //do not generate button.
            }

            GameObject button = this.buttonPool.Get();

            Vector2Int dimensions = gameGrid.getDimensions();

            float xPos = gridPosition.x * LevelModuleData.width;
            float yPos = gridPosition.y * LevelModuleData.height;
            Vector2 position = new Vector2(xPos, yPos );
            position.x += xOffset;
            position.y += yOffset;
            position = MathHelper.cartesianToIsometric(position);
            Vector2 basePosition = position;
            position = Camera.main.WorldToScreenPoint(position);
            button.transform.position = position;
            Vector2Int fromGridPosition = new Vector2Int(gridPosition.x, gridPosition.y);
            ButtonData buttonData = new ButtonData(button, basePosition);
            this.activeButtons.Add(buttonData);

            Button buttonComponent = button.GetComponent<Button>();
            if (buttonComponent == null)
            {
                Debug.LogError("No button component");
            }

            UnityEngine.Events.UnityAction onClick = null;
            onClick = () =>
            {
                this.activeButtons.Remove(buttonData);
                this.buttonPool.Release(button);
                GameEvents.instance.expandButtonGeneration();



                generateNewSubModule(fromGridPosition, new Vector2Int(fromGridPosition.x + nextGridPositionX, fromGridPosition.y + nextGridPositionY));
                buttonComponent.onClick.RemoveListener(onClick);
            };
            buttonComponent.onClick.AddListener(onClick);
        }
    }

    private void generateStartPosition()
    {

        int pickedIndex = Mathf.FloorToInt(Random.Range(0, startModules.Length-1));

        Vector2Int dimensions = gameGrid.getDimensions();

        Vector2 startPosition = MathHelper.cartesianToIsometric(dimensions.x*0.5f, dimensions.y*0.5f);
        Vector2Int initialGridPosition = new Vector2Int(Mathf.FloorToInt((dimensions.x * 0.5f) / LevelModuleData.width), Mathf.FloorToInt(dimensions.y * 0.5f / LevelModuleData.height));
        LevelModule currentModule = startModules[pickedIndex];
        for (int i = 0; i < startModules[pickedIndex].tiles.Length; i++)
        {
            int yIndex = Mathf.FloorToInt(dimensions.x * 0.5f) - Mathf.FloorToInt(LevelModuleData.width*0.5f) + i % LevelModuleData.width;
            int xIndex = Mathf.FloorToInt(dimensions.y * 0.5f) - Mathf.FloorToInt(LevelModuleData.height*0.5f) + Mathf.FloorToInt(i / LevelModuleData.width);
            Vector3Int index = new Vector3Int(xIndex, yIndex, 0);
            tilemap.SetTile(index, currentModule.tiles[i]);
            tilemap.RefreshTile(index);
        }
        this.gameGrid.onNewLevelModuleGenerated(currentModule, initialGridPosition);
        this.levelModuleGrid.setCurrentlyAvailableLevelModule(initialGridPosition, currentModule);
        this.levelModuleGrid.setOccupied(initialGridPosition, true);
        List<Direction> directions = LevelModule.getValidDirections(currentModule);

        this.generateButtons(initialGridPosition,directions);
    }

    private void buildLevelModule(LevelModule levelModule, Vector2Int gridPosition)
    {
        for (int i = 0; i < levelModule.tiles.Length; i++)
        {
            int yIndex = gridPosition.y * LevelModuleData.height - Mathf.FloorToInt(LevelModuleData.height * 0.5f) + i % LevelModuleData.width; //think 
            int xIndex = gridPosition.x * LevelModuleData.width - Mathf.FloorToInt(LevelModuleData.width * 0.5f) + Mathf.FloorToInt(i / LevelModuleData.width);
            Vector3Int index = new Vector3Int(xIndex, yIndex,0);
            tilemap.SetTile(index, levelModule.tiles[i]);
            tilemap.RefreshTile(index);
            
        }
        gameGrid.onNewLevelModuleGenerated(levelModule, gridPosition);
    }


    private void generateNewSubModule( Vector2Int fromGridPosition, Vector2Int toGridPosition )
    {
        float deltaX =  toGridPosition.x - fromGridPosition.x;
        float deltaY = toGridPosition.y - fromGridPosition.y;
        float epsilon = Config.epsilon;
        Direction dir = Direction.NULL;
        if (deltaX < 0 && Mathf.Abs(deltaX) >= epsilon)
        {
            dir = Direction.W;
        } else if (deltaX > 0 && Mathf.Abs(deltaX) >= epsilon)
        {
            dir = Direction.E;
        } else if (deltaY > 0 && Mathf.Abs(deltaY) >= epsilon)
        {
            dir = Direction.N;
        } else if (deltaY < 0 && Mathf.Abs(deltaY) >= epsilon)
        {
            dir = Direction.S;
        } else
        {
            Debug.LogError("Direction error when generating new submodule.");
        }

        List<ModuleConnection> allowedConnections = levelModuleGrid.GetValidConnections(dir,toGridPosition); //consider moving to levelmodulegrid.

        if (allowedConnections.Count == 0)
        {
            return;
        }

        int pickedIndex = Mathf.FloorToInt(Random.Range(0, singleModules.Length));
        while(!allowedConnections.Contains(singleModules[pickedIndex].connection))
        {
            pickedIndex = Mathf.FloorToInt(Random.Range(0, singleModules.Length));
        }
        this.buildLevelModule(singleModules[pickedIndex], toGridPosition);
        this.updateAvailablePositions(singleModules[pickedIndex], fromGridPosition,toGridPosition);




        this.onRoundEnded(); 


        this.startNewRound(toGridPosition, singleModules[pickedIndex], dir);
    }

    private void startNewRound(Vector2Int gridPosition, LevelModule levelModule, Direction directionToNewModule)
    {
        Vector2 waveStartPosition = new Vector2(gridPosition.x * LevelModuleData.width, gridPosition.y * LevelModuleData.height);
        //now offset of three tiles depending on direction of final tile. so where we came from, and direction of new tile.
        float offsetX = Mathf.Floor(LevelModuleData.width*0.5f);
        float offsetY = Mathf.Floor(LevelModuleData.height*0.5f);
        switch(levelModule.connection)
        {
            case ModuleConnection.E:
                waveStartPosition.x += offsetX; //TODO: Remove hardcoded value. half of width/height.
                break;
            case ModuleConnection.N:
                waveStartPosition.y += offsetY;
                break;
            case ModuleConnection.S:
                waveStartPosition.y -= offsetY;
                break;
            case ModuleConnection.W:
                waveStartPosition.x -= offsetX;
                break;
            case ModuleConnection.NW:
                if (directionToNewModule == Direction.E)
                {
                    waveStartPosition.y += offsetY;
                } else if (directionToNewModule == Direction.S)
                {
                    waveStartPosition.x -= offsetX;
                } else
                {
                    Debug.LogError("Error");
                }
                break;
            case ModuleConnection.NS:
                if (directionToNewModule == Direction.N)
                {
                    waveStartPosition.y += offsetY; //startpos is north, since we came from direction north, meaning south is endpoint of currentmodule.
                }
                else if (directionToNewModule == Direction.S)
                {
                    waveStartPosition.y -= offsetY;
                }
                else
                {
                    Debug.LogError("Error");
                }
                break;
            case ModuleConnection.NE:
                if (directionToNewModule == Direction.W)
                {
                    waveStartPosition.y += offsetY;
                }
                else if (directionToNewModule == Direction.S)
                {
                    waveStartPosition.x += offsetX;
                }
                else
                {
                    Debug.LogError("Error");
                }
                break;
            case ModuleConnection.SW:
                if (directionToNewModule == Direction.E)
                {
                    waveStartPosition.y -= offsetY; //going south.
                }
                else if (directionToNewModule == Direction.N)
                {
                    waveStartPosition.x -= offsetX; //going west.
                }
                else
                {
                    Debug.LogError("Error");
                }
                break;
            case ModuleConnection.SE:
                if (directionToNewModule == Direction.W)
                {
                    waveStartPosition.y -= offsetY;
                }
                else if (directionToNewModule == Direction.N)
                {
                    waveStartPosition.x += offsetX;
                }
                else
                {
                    Debug.LogError("Error");
                }
                break;
            case ModuleConnection.WE:
                if (directionToNewModule == Direction.W)
                {
                    waveStartPosition.x -= offsetX;
                }
                else if (directionToNewModule == Direction.E)
                {
                    waveStartPosition.x += offsetX;
                }
                else
                {
                    Debug.LogError("Error");
                }
                break;
            default:
                break;
        }
        waveStartPosition = MathHelper.cartesianToIsometric(waveStartPosition);

        GameEvents.instance.newRoundStarted(waveStartPosition, 1);
    }

}
