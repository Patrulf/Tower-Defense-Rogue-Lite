using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor;
namespace Alice
{
    public struct TileData
    {
        public Tile[] tiles;
        public Dictionary<string, int> nameToIndex; //name of tile, tileIndex. 

        public TileData(Tile[] p_tiles)
        {
            tiles = p_tiles;
            nameToIndex = new Dictionary<string, int>();
            for (int i = 0; i < tiles.Length; i++)
            {
                nameToIndex.Add(tiles[i].name, i);
            }
        }
    }


    [RequireComponent(typeof(Tilemap), typeof(Grid), typeof(TilemapRenderer) )]
    public class LevelModuleEditor : MonoBehaviour, LevelEditorControls.ILevelEditorMapActions
    {
        public LevelModule[] startModules;
        public LevelModule[] singleModules; 
        public TMP_Dropdown startModulesDropDown;
        public TMP_Dropdown singleModulesDropDown;



        public GameTile[] tiles;
        //public Tile[] unPathableTiles;

        public TMP_Dropdown tilesDropDown;

        private Tilemap tilemap;

        private LevelModule _currentLevelModule;

        private GameTile _currentTileSelection;
        private LevelEditorControls controls;


        private void Awake()
        {
            if (controls == null)
            {
                controls = new LevelEditorControls();
                controls.LevelEditorMap.SetCallbacks(this);
            }
            controls.LevelEditorMap.Enable();
        }

        private void Start()
        {
            _currentTileSelection = null;
            _currentLevelModule = null;
            Vector2 cameraPos = MathHelper.cartesianToIsometric(new Vector2(LevelModuleData.width * 0.5f, LevelModuleData.height * 0.5f) );
            Camera.main.transform.position = new Vector3(cameraPos.x,cameraPos.y,Camera.main.transform.position.z);

            //tileData = new TileData(tiles); //unsure purpose of this one.

            tilemap = GetComponent<Tilemap>();


            this.initDropDownMenus();

            this.handleStartModules(0);
            //this.handleSingleModules(0);
            this.handleTileSelection(0);
        }

        private void initDropDownMenus()
        {
            this.initStartModulesDropDown();

            this.initSinglesModulesDropDown();
            this.initTilesDropDown();
        }

        private void initSinglesModulesDropDown()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < singleModules.Length; i++)
            {
                names.Add(singleModules[i].name);
            }
            singleModulesDropDown.ClearOptions();
            singleModulesDropDown.AddOptions(names);
        }

        private void initStartModulesDropDown()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < startModules.Length; i++)
            {
                names.Add(startModules[i].name);
            }
            startModulesDropDown.ClearOptions();
            startModulesDropDown.AddOptions(names);
        }
        private void initTilesDropDown()
        {
            List<string> tileNames = new List<string>();
            for (int i = 0; i < tiles.Length; i++)
            {
                tileNames.Add(tiles[i].name);
            }
            tilesDropDown.ClearOptions();
            tilesDropDown.AddOptions(tileNames);
        }



        private void buildModuleTiles(LevelModule currentModule)
        {
            for (int i = 0; i < LevelModuleData.height * LevelModuleData.width; i++)
            {
                TileBase defaultTile;
                if (!currentModule.tiles[i])
                {
                    //currentModule.tiles = new GameTile[];
                    defaultTile = tiles[0];
                    currentModule.tiles[i] = tiles[0];
                } else
                {
                    defaultTile = currentModule.tiles[i];
                }

                int xPos = Mathf.FloorToInt(i / LevelModuleData.width);
                int yPos = i % LevelModuleData.width;
                Vector3Int currentPosition = new Vector3Int(xPos, yPos, 0);

                tilemap.SetTile(currentPosition, defaultTile);
                tilemap.RefreshTile(currentPosition);
            }
        }

        private void onLevelModuleSelected(LevelModule currentSelection)
        {
            this._currentLevelModule = currentSelection;
            this.buildModuleTiles(currentSelection);
        }

        public void handleSingleModules(int val)
        {
            this.onLevelModuleSelected(singleModules[val]);
        }
    
        public void handleStartModules(int val)
        {
            this.onLevelModuleSelected(startModules[val]);
        }

        private void onTileSelected(GameTile tile)
        {
            this._currentTileSelection = tile;

        }

        public void handleTileSelection(int val)
        {
            this.onTileSelected(tiles[val]);
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!_currentTileSelection)
                {
                    Debug.LogError("No tile selected");
                    return;
                }
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector2Int tileIndex = Vector2Int.FloorToInt(MathHelper.isometricToCartesian(mousePos));
                Vector3Int tilePosition = new Vector3Int(tileIndex.x, tileIndex.y, 0);

                if (tileIndex.x >= 0 && tileIndex.x < LevelModuleData.width &&
                    tileIndex.y >= 0 && tileIndex.y < LevelModuleData.height)
                {
                    this._currentLevelModule.tiles[tileIndex.y + tileIndex.x * LevelModuleData.height] = this._currentTileSelection;
                    EditorUtility.SetDirty(this._currentLevelModule); //to notify scriptableobject has been updated.
                    tilemap.SetTile(tilePosition, this._currentTileSelection);
                    tilemap.RefreshTile(tilePosition);
                }
            }
        }

        public void onSave()
        {
            var modules = this.startModules.Concat(singleModules.ToArray());
            foreach(LevelModule module in modules)
            {
                SaveSystem.saveLevelModule(module);
            }
        }

        public void OnDeselect(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

