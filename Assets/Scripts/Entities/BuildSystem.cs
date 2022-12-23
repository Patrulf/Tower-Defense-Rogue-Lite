using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BuildSystem : MonoBehaviour
{

    private bool _isAllowedToBuild;

    public Canvas canvas;

    public BuildingData[] buildings;

    public GameObject buttonPrefab;

    private List<GameObject> _buttons;

    private BuildingData _currentSelection;
    private GameObject _currentSelectionGameObject;
    
    private Sprite _currentSelectionSprite;


    private Coroutine buildingPlacementCoroutine;

    public GameGrid gameGrid;



    public GameObject buildingPrefab;
    private ObjectPool<GameObject> _buildingPrefabPool;


    private void Awake()
    {
        this._buildingPrefabPool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate<GameObject>(buildingPrefab);
        },(entity) =>
        {
            SpriteRenderer spriteRenderer = buildingPrefab.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.enabled = true;
        },(entity) => {


            SpriteRenderer spriteRenderer = buildingPrefab.GetComponentInChildren<SpriteRenderer>();
            
            spriteRenderer.enabled = false;
        });

        this._currentSelectionGameObject = new GameObject();
        this._currentSelectionGameObject.AddComponent<Transform>();

        GameObject selectionChild = new GameObject();
        selectionChild.AddComponent<Transform>();
        selectionChild.AddComponent<SpriteRenderer>();
        selectionChild.GetComponent<Transform>().SetParent(this._currentSelectionGameObject.transform);
        selectionChild.transform.position = gameGrid.cartesianToIsometric(new Vector2(0.5f, 0.5f)); //Just to offset sprites.


        _buttons = new List<GameObject>();

        this._isAllowedToBuild = false;

        this._currentSelection = null;
        this._currentSelectionSprite = null;

        this.initBuildingButtons();

    }
    void Start()
    {
        GameEvents.instance.onMouseMoved += this.onMouseMoved;
        GameEvents.instance.onSelect += this.onBuildingPlacement;
    }


    private void initBuildingButtons()
    {
        GameObject canvasObject = canvas.gameObject;
        if (canvasObject == null)
        {
            Debug.LogError("Add a canvas to this gameObject");
        }
        for (int i = 0; i < buildings.Length; i++)
        {

            BuildingData currentBuildingData = buildings[i];

            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(canvasObject.transform);
            Button buttonComponent = button.GetComponent<Button>();
            UnityEngine.Events.UnityAction onClick = null;
            onClick = () =>
            {
                _currentSelection = currentBuildingData;
                _currentSelectionSprite = _currentSelection.sprite; 
                this._currentSelectionGameObject.GetComponentInChildren<SpriteRenderer>().sprite = _currentSelectionSprite;
            };
            //TODO: add slider for buttons.
            buttonComponent.onClick.AddListener(onClick);
            buttonComponent.image.sprite = currentBuildingData.sprite;
            buttonComponent.transform.position -= new Vector3(0.0f,(64.0f + 20.0f) * i,0.0f);
        }
    }



    //TODO: Should also be called when moving camera.
    public void onMouseMoved(Vector2 mousePosition)
    {

        Vector2 worldCoordinates = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 cartesian = gameGrid.isometricToCartesian(worldCoordinates);

        Vector2 dimensions = this.gameGrid.getDimensions();

        Vector2 clampedCoords = gameGrid.cartesianToIsometric(new Vector2(Mathf.Floor(cartesian.x  ) , Mathf.Floor(cartesian.y) ) );

        bool isBuilding = this._currentSelection != null ? true:false;
        if (isBuilding)
        {
            this._currentSelectionGameObject.transform.position = new Vector3(clampedCoords.x, clampedCoords.y, this._currentSelectionGameObject.transform.position.z);
        }
    }

    public void onBuildingPlacement(Vector2 mousePosition)
    {
        if (!_currentSelection)
        {
            return;
        }
        Vector2 worldCoordinates = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2Int cartesian = Vector2Int.FloorToInt(gameGrid.isometricToCartesian(worldCoordinates)) ;
        Vector2 clampedIsometric = gameGrid.cartesianToIsometric(cartesian);

        if (gameGrid.getTileOccupation(cartesian.x,cartesian.y) == OccupiedState.Buildable)
        {
            GameObject buildingPrefab = this._buildingPrefabPool.Get();
            buildingPrefab.GetComponentInChildren<SpriteRenderer>().sprite = this._currentSelectionSprite;
            buildingPrefab.transform.position = new Vector3(clampedIsometric.x,clampedIsometric.y,buildingPrefab.transform.position.z);
            gameGrid.SetTileOccupationCartesian(cartesian.x,cartesian.y, OccupiedState.UnBuildable);
        }

    }

    public void startBuildSession()
    {
        this._isAllowedToBuild = true;
    }

    public void disableBuildSession()
    {
        this._isAllowedToBuild = false;
    }
    
}
