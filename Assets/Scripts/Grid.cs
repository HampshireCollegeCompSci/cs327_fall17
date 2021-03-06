﻿// Author(s): Paul Calande, Yifeng Shi, Yixiang Xu, Wm. Josiah Erikson
// A 2-dimensional collections of Tiles.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Grid : MonoBehaviour
{
    // Invoked once when any number of squares is formed, before the clearing animation begins.
    public delegate void SquareFormedHandler(int scorePerSquare, Vector3 textPos);
    public event SquareFormedHandler SquareFormed;
    // Invoked when a square is outlined during the clearing animation,
    public delegate void SquareOutlinedHandler(int scorePerSquare, Vector3 textPos);
    public event SquareOutlinedHandler SquareOutlined;
    // Invoked when the clearing animation is finished for any number of squares.
    public delegate void SquaresClearedHandler();
    public event SquaresClearedHandler SquaresCleared;

    [SerializeField]
    [Tooltip("The width of the Grid. Populated by JSON.")]
    int width;
    [SerializeField]
    [Tooltip("The height of the Grid. Populated by JSON.")]
    int height;
    [SerializeField]
    [Tooltip("The base energy decay per turn. Populated by JSON.")]
    int baseEnergyDecayRate;
    [SerializeField]
    [Tooltip("Base energy decay rate bonus. Set by VoidEventController.")]
    int baseEnergyDecayRateBonus = 0;
    [SerializeField]
    [Tooltip("The list of additional energy decayed from vestige of each level. Populated by JSON.")]
    JSONArray decayRates;
    [SerializeField]
    [Tooltip("The maximum level a vestige can be. Populated by JSON.")]
    int vestigeMaxLevel;
    [SerializeField]
    [Tooltip("The Tile prefab to instantiate.")]
    GameObject prefabTile;
    [SerializeField]
    [Tooltip("The Space prefab to instantiate.")]
    GameObject prefabSpace;
    [SerializeField]
    [Tooltip("Reference to the BlockSpawner instance.")]
    BlockSpawner blockSpawner;
    [SerializeField]
    [Tooltip("Reference to the EnergyCounter instance.")]
    EnergyCounter energyCounter;
    [SerializeField]
    [Tooltip("Reference to the TurnCounter instance.")]
    TurnCounter turnCounter;
    [SerializeField]
    [Tooltip("Reference to the VestigeCounter instance.")]
    VestigeCounter vestigeCounter;
    [SerializeField]
    [Tooltip("Reference to the Tuning JSON.")]
    TextAsset tuningJSON;
    [SerializeField]
    [Tooltip("Energy earned per 3x3 square cleared. Populated by JSON.")]
    int energyPerSquare;
    [SerializeField]
    [Tooltip("Score earned per 3x3 square cleared. Populated by JSON.")]
    int scorePerSquare;
    [SerializeField]
    [Tooltip("Reference to the RectTransform component of this Grid.")]
    RectTransform rectTransform;
    [SerializeField]
    SubCanvas subCanvas;
    
    /*
    [SerializeField]
    [Tooltip("Reference to energy gain animator.")]
    Animator energyGainController;
    [SerializeField]
    [Tooltip("Reference to energy transfer ball animator.")]
    Animator energyTransferBallController;
    */
    [SerializeField]
    [Tooltip("Whether or not asteroids can spawn in filled cells. Populated by JSON.")]
    bool asteroidsCanSpawnInFilledCells;
    [SerializeField]
    [Tooltip("Reference to glowing outline side prefab")]
    GameObject outLinePrefab;
    [SerializeField]
    [Tooltip("Reference to energy transfer lighting prefab")]
    GameObject energyTransferPrefab;
    [SerializeField]
    [Tooltip("The underlying array of Tiles.")]
    Tile[,] tiles;
    [SerializeField]
    [Tooltip("Reference to the console grid.")]
    ConsoleGrid consoleGrid;
    [SerializeField]
    [Tooltip("Reference to the clear explosion prefab")]
    GameObject explosionPrefab;
    [SerializeField]
    [Tooltip("The number of seconds between each square highlighting animation. Populated by JSON.")]
    float secondsBetweenSquareAnimations;
    [SerializeField]
    [Tooltip("Reference to the reactor GameObject.")]
    GameObject reactor;
    [SerializeField]
    [Tooltip("How many square clearings have occurred so far. Incremented by 1 every time the player forms any number of squares in a single turn.")]
    int squareClearingsCount = 0;
    [SerializeField]
    [Tooltip("Prefabs to instantiate for vestige energy loss animations (from lowest level to highest level).")]
    GameObject[] prefabEnergyLossVestige;
    [SerializeField]
    [Tooltip("Prefab to instantiate for reactor energy loss animation.")]
    GameObject prefabEnergyLossReactor;
    [SerializeField]
    [Tooltip("Seconds between each vestige energy loss animation. Populated by JSON.")]
    float secondsBetweenVestigeEnergyLossAnimations;
    /*
    [SerializeField]
    [Tooltip("Reference to the game over controller.")]
    UIGameOver gameOver;
    */
    [SerializeField]
    [Tooltip("Reference to score blocks.")]
    ScoreBlocks scoreBlocks;
    [SerializeField]
    [Tooltip("Reference to accumulator Transform.")]
    Transform transformAccumulator;

    // The width of one Tile, calculated compared to the Grid's dimensions.
    private float tileWidth;
    // The height of one Tile, calculated compared to the Grid's dimensions.
    private float tileHeight;
    // The masks of asteroids, populated by JSON. Tier mapped to 1s and 0s.
    private Dictionary<int, int[,]> asteroidMasks = new Dictionary<int, int[,]>();

    Dictionary<Vector2, List<Space>> spaces = new Dictionary<Vector2, List<Space>>();

    List<GridBlock> gridBlocks;

    private void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        width = json["grid width"].AsInt;
        height = json["grid height"].AsInt;
        baseEnergyDecayRate = json["base energy decay rate"].AsInt;
        decayRates = json["vestige decay rates"].AsArray;
        vestigeMaxLevel = json["vestige max level"].AsInt;
        energyPerSquare = json["energy per 3x3 square cleared"].AsInt;
        scorePerSquare = json["score per 3x3 square cleared"].AsInt;
        secondsBetweenSquareAnimations = json["seconds between square animations"].AsFloat;
        asteroidsCanSpawnInFilledCells = json["asteroids can spawn in filled cells"].AsBool;
        secondsBetweenVestigeEnergyLossAnimations = json["seconds between vestige energy loss animations"].AsFloat;

        ReadAsteroidMask(json, 1);
        ReadAsteroidMask(json, 2);
        ReadAsteroidMask(json, 3);
    }

    private void ReadAsteroidMask(JSONNode json, int tier)
    {
        string jsonEntryName = "asteroid area denial " + tier;
        int[,] newMask = new int[height, width];
        JSONArray asteroidMaskJSON = json[jsonEntryName].AsArray;
        int asteroidMaskCount = asteroidMaskJSON.Count;
        for (int i = 0; i < asteroidMaskCount; ++i)
        {
            int val = asteroidMaskJSON[i].AsInt;
            int col = i % width;
            int row = i / width;
            newMask[row, col] = val;
        }
        asteroidMasks.Add(tier, newMask);
    }

    private void Awake()
    {
        consoleGrid.Init();
    }

    private void Start()
    {
        subCanvas.WideScreenSupport();
        Tune();
        //WideScreenSupport();

        tileWidth = rectTransform.rect.width / width;
        tileHeight = rectTransform.rect.height / height;

        //Instantiate tiles array
        tiles = CreateTileArray(prefabTile, transform, Vector3.zero, height, width);

        foreach (Tile t in tiles)
        {
            // Make all of the Tiles clear.
            t.Clear();
            t.SetSprite(TileData.TileType.Unoccupied);
            // Subscribe to events.
            //t.Changed += Tile_Changed;
        }

        //Instantiate spaces
        InstantiateSpaces();

        // Initialize BlockSpawner.
        blockSpawner.Init();
        //blockSpawner.Init(this);

        //Instantiate GridBlocks
        gridBlocks = new List<GridBlock>();
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetTileWidth()
    {
        return tileWidth;
    }

    public float GetTileHeight()
    {
        return tileHeight;
    }

    public bool GetIsOccupied(int row, int col)
    {
        return tiles[row, col].GetIsOccupied();
    }

    public Tile GetTileAt(int row, int col)
    {
        return tiles[row, col];
    }

    public Vector3 GetTilePosition(int row, int col)
    {
        return tiles[row, col].transform.localPosition;
    }

    public void Fill(int row, int col, TileData.TileType newType)
    {
        tiles[row, col].Fill(newType);
    }

    public void Clear(int row, int col)
    {
        tiles[row, col].Clear();
    }

    public TileData.TileType GetTileType(int row, int col)
    {
        return tiles[row, col].GetTileType();
    }

    /*
    public Tile[,] GetTiles()
    {
        return tiles;
    }
    */

    public Tile[,] CreateTileArray(GameObject prefabTile, Transform parent, Vector3 center, int height, int width)
    {
        Tile[,] result = new Tile[height, width];

        // Calculate the position of the top-left corner of the array.
        float cornerX = center.x - ((width - 1) * tileWidth * 0.5f);
        float cornerY = center.y + ((height - 1) * tileHeight * 0.5f);

        // Iterate through all the Tiles of the array.
        for (int c = 0; c < width; c++)
        {
            for (int r = 0; r < height; r++)
            {
                // Calculate the position of the Tile.
                float tileX = cornerX + c * tileWidth;
                float tileY = cornerY - r * tileHeight;
                Vector3 pos = new Vector3(tileX, tileY, 0.0f);

                GameObject currentPrefabTile = GameObject.Instantiate(prefabTile, parent, false);
                currentPrefabTile.transform.localPosition = pos;
                currentPrefabTile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileWidth, tileHeight);
                /*
                if (c == 0 && r == 0)
                {
                    // Let's figure out which Tile ends up in the (0, 0) corner...
                    currentPrefabTile.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                }
                */

                result[r, c] = currentPrefabTile.GetComponent<Tile>();
            }
        }

        return result;
    }

    public bool CanBlockFit(int row, int col, Block block)
    {
        //Assume each tile is 1x1 size.
        for (int c = 0; c < block.GetWidth(); c++)
        {
            for (int r = 0; r < block.GetHeight(); r++)
            {
                if (row + r >= GetHeight() || col + c >= GetWidth())
                {
                    if (block.GetIsOccupied(r, c))
                        return false;
                }
                else
                {
                    if (tiles[row + r, col + c].GetIsOccupied() && block.GetIsOccupied(r, c))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void CheckBlockForTutorial(int row, int col, Block block)
    {
        int width = block.GetWidth();
        int height = block.GetHeight();
        for (int r = 0; r < height; ++r)
        {
            for (int c = 0; c < width; ++c)
            {
                if (block.GetTileType(r, c) == TileData.TileType.Vestige)
                {
                    Tile v = tiles[row + r, col + c];
                    CheckVestigeForTutorial(v, row + r, col + c);
                }
            }
        }
    }

    // IMPORTANT: Don't forget about the additional WriteBlock overload below!
    // row and col correspond to the block's top-left corner on the Grid.
    public GridBlock WriteBlock(int row, int col, Block block)
    {
        //List<Coordinate> coords = new List<Coordinate>();
        for (int c = 0; c < block.GetWidth(); c++)
        {
            for (int r = 0; r < block.GetHeight(); r++)
            {
                if (block.GetIsOccupied(r, c))
                {
                    Tile theTile = tiles[row + r, col + c];
                    //theTile.Fill(block.GetTileType(r, c));
                    theTile.Duplicate(block.GetTileData(r, c));

                    //Note x is col and y is row
                    //coords.Add(new Coordinate(x + c, y + r));
                }
            }
        }
        GridBlock gb = new GridBlock(row, col, block, this);
        gridBlocks.Add(gb);

        //call LShapeCheck after each insertion
        //LShapeCheck(coords);

        CheckBlockForTutorial(row, col, block);

        return gb;
    }
    public GridBlock WriteBlock(int row, int col, DraggableBlock block)
    {
        //List<Coordinate> coords = new List<Coordinate>();
        for (int c = 0; c < block.GetWidth(); c++)
        {
            for (int r = 0; r < block.GetHeight(); r++)
            {
                if (block.GetIsOccupied(r, c))
                {
                    Tile theTile = tiles[row + r, col + c];
                    theTile.Duplicate(block.GetTileData(r, c));

                    //theTile.Fill(block.GetTileType(r, c));
                    theTile.SetSpriteAbsolute(block.GetSprite(r, c));

                    //Note x is col and y is row
                    //coords.Add(new Coordinate(x + c, y + r));
                }
            }
        }
        Block myBlock = block.GetBlock();
        GridBlock gb = new GridBlock(row, col, myBlock, this);
        gridBlocks.Add(gb);

        //call LShapeCheck after each insertion
        //LShapeCheck(coords);

        CheckBlockForTutorial(row, col, myBlock);

        return gb;
    }

    public bool SetHighlight(int row, int col, DraggableBlock block, bool on)
    {
        if (!on)
        {
            for (int c = 0; c < GetWidth(); c++)
            {
                for (int r = 0; r < GetHeight(); r++)
                {
                    //Unhighlight all tiles
                    tiles[r, c].SetNormal();
                    //tiles[r, c].SetSprite(tiles[r, c].GetTileType());
                    tiles[r, c].SetSpriteToTrueSprite();
					block.TurnBlockImageOn();
                }
            }
        }
        else
        {
            for (int c = 0; c < block.GetWidth(); c++)
            {
                for (int r = 0; r < block.GetHeight(); r++)
                {
                    if (block.GetIsOccupied(r, c))
                    {
                        //If can place here then set highlight
                        tiles[row + r, col + c].SetHighlight();
                        tiles[row + r, col + c].SetSprite(block.GetSprite(r, c));
						block.TurnBlockImageOff();
                    }
                }
            }
        }

        return on;
    }

    public void AnticipatedHighlight(int row, int col, DraggableBlock newBlock, bool on)
    {
        if (on)
        {
            TileData.TileType[,] copy = new TileData.TileType[height, width];

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    copy[r, c] = tiles[r, c].GetTileType();
                }
            }

            for (int c = 0; c < newBlock.GetWidth(); c++)
                for (int r = 0; r < newBlock.GetHeight(); r++)
                    if (newBlock.GetIsOccupied(r, c))
                        copy[row + r, col + c] = newBlock.GetTileType(r, c);
            
            List<Tile> anticipatedSquareTiles = new List<Tile>();
            List<int[]> anticipatedPotentialSquares = new List<int[]>();

            int biggestSquareSize = Mathf.Min(width, height);

            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    // Proceed only if the Tile is regular tile.
                    if (copy[r, c] == TileData.TileType.Regular ||
                        copy[r, c] == TileData.TileType.Vestige)
                    {
                        // Check for squares from length 3 upwards.
                        for (int length = 3; length <= biggestSquareSize; length++)
                        {
                            CheckForSquares(r, c, length, ref anticipatedSquareTiles, ref anticipatedPotentialSquares, copy);
                        }
                    }
                }
            }

            if (anticipatedSquareTiles.Count != 0)
            {
                List<VestigeMarker> toVestiges = new List<VestigeMarker>(); //No effect. Just match parameter.
                //Highlight anticipated vestiges
                foreach (int[] s in anticipatedPotentialSquares)
                {
                    FormVestiges(s[0], s[1], s[2], anticipatedSquareTiles, copy, ref toVestiges);
                }

                //Highlight all tiles that will form squares
                foreach (Tile t in anticipatedSquareTiles)
                {
                    t.SetAnticipatedHighlight(TileData.TileType.Regular);
                }
            }
        }
        else
        {
            for (int c = 0; c < GetWidth(); c++)
            {
                for (int r = 0; r < GetHeight(); r++)
                {
                    //Unhighlight all tiles
                    tiles[r, c].SetNormal();
                    tiles[r, c].SetSpriteToTrueSprite();
                }
            }
        }
    }

    IEnumerator BeginClearingSquares(List<int[]> squaresFormed, List<Tile> toRemove)
    {
        yield return new WaitUntil(() => !TutorialController.Instance.IsPanelOpen());

        //Turn vestiges when isPlaced is true
        List<VestigeMarker> newVestiges = new List<VestigeMarker>();
        foreach (int[] s in squaresFormed)
        {
            //Spawn a text indicating scores at the center of the cleared square
            /*
            Vector3 textPos = new Vector3();
            if (s[2] % 2 == 1)
            {
                textPos = GetTilePosition(s[0] + (s[2] - 1) / 2, s[1] + (s[2] - 1) / 2);
            }
            else
            {
                Vector3 rightPos = GetTilePosition(s[0] + (s[2] - 1) / 2 + 1, s[1] + (s[2] - 1) / 2 + 1);
                Vector3 leftPos = GetTilePosition(s[0] + (s[2] - 1) / 2, s[1] + (s[2] - 1) / 2);
                textPos = new Vector3((leftPos.x + rightPos.x) / 2, (leftPos.y + rightPos.y) / 2, (leftPos.z + rightPos.z) / 2);
            }

            // If a legal square is formed, tell the event handler.
            // This doesn't actually give points.
            OnSquareFormed(scorePerSquare, textPos);
            */

            FormVestiges(s[0], s[1], s[2], toRemove, null, ref newVestiges);
        }

        //energyCounter.PopUp("+", squaresFormed.Count * energyPerSquare);

        List<Tile> duplicatesRemoved = toRemove.Distinct().ToList();

        StartCoroutine(ClearingOutlineEffect(squaresFormed, duplicatesRemoved, newVestiges));

        if (squaresFormed.Count != 0)
        {
            squareClearingsCount += 1;
        }
    }

    public bool CheckForMatches()
    {
        bool squareFormed = false;

        // List keeping all tiles that are going to be removed.
        // We do remove after calculation because it's possible to remove
        // multiple squares together.
        List<Tile> toRemove = new List<Tile>();

        //List of all potential squares (including smaller squares inside bigger one)
        List<int[]> squaresFormed = new List<int[]>();

        //Loop through all blocks from top left.
        //Consider only the case that the current tile is the top-left corner.

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                // Proceed only if the Tile is regular tile.
                if (tiles[r, c].GetIsOccupied())
                {
                    // Check for all 3x3 squares.
                    CheckForSquares(r, c, 3, ref toRemove, ref squaresFormed, null);
                }
            }
        }

        //If toRemove is not empty, then there is at least one square formed
        if (toRemove.Count != 0)
        {
            squareFormed = true;

            TutorialController.Instance.FormedSquare();

            OnSquareFormed(scorePerSquare, Vector3.zero);

            StartCoroutine(BeginClearingSquares(squaresFormed, toRemove));
        }

        gridBlocks.Sort((y, x) => x.GetRow().CompareTo(y.GetRow()));

        // Inform all GridBlocks that matches have been checked.
        for (int i = 0; i < gridBlocks.Count; ++i)
        {
            if (gridBlocks[i].MatchesChecked())
            {
                // Step back an index if the current GridBlock has been deleted.
                --i;
            }
        }

        return squareFormed;
    }

    class VestigeMarker
    {
        Tile vestige;
        int row;
        int col;

        public VestigeMarker(Tile mvestige, int mrow, int mcol)
        {
            vestige = mvestige;
            row = mrow;
            col = mcol;
        }

        public Tile GetTile()
        {
            return vestige;
        }

        public int GetRow()
        {
            return row;
        }

        public int GetCol()
        {
            return col;
        }
    }

    private void CheckVestigeForTutorial(Tile v, int row, int col)
    {
        TutorialController.Instance.MovePanelToBlockLocation(4, row, col);
        if (v.GetVestigeLevel() == 1)
        {
            TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_WASTE);
        }
        else if (v.GetVestigeLevel() == 2)
        {
            TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_WASTE_2);
        }
        else if (v.GetVestigeLevel() == 3)
        {
            TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_WASTE_3);
        }
    }

    private void TurnTilesIntoVestiges(List<VestigeMarker> newVestiges)
    {
        //Turning and upgrading vestiges
        foreach (VestigeMarker vm in newVestiges)
        {
            Tile v = vm.GetTile();
            if (v.GetTileType() == TileData.TileType.Regular)
            {
                v.Fill(TileData.TileType.Vestige);
                v.SetVestigeLevel(1);
            }
            else if (v.GetTileType() == TileData.TileType.Vestige)
            {
                if (v.GetVestigeLevel() < vestigeMaxLevel)
                    v.SetVestigeLevel(v.GetVestigeLevel() + 1);
            }

            int row = vm.GetRow();
            int col = vm.GetCol();
            CheckVestigeForTutorial(v, row, col);
        }
        energyCounter.UpdateEnergy();
    }

    private IEnumerator ClearingOutlineEffect(List<int[]> squaresFormed, List<Tile> duplicatesRemoved,
        List<VestigeMarker> newVestiges)
    {
        Vector3 reactorPos = reactor.transform.position;

        foreach (int[] square in squaresFormed)
        {
            GameObject outline = DrawOutLine(square[0], square[1]);

            Vector3 rightPos = GetTilePosition(square[0] + (square[2] - 1) / 2 + 1, square[1] + (square[2] - 1) / 2 + 1);
            Vector3 leftPos = GetTilePosition(square[0] + (square[2] - 1) / 2, square[1] + (square[2] - 1) / 2);
            Vector3 textPos = new Vector3((leftPos.x + rightPos.x) / 2, (leftPos.y + rightPos.y) / 2, (leftPos.z + rightPos.z) / 2);

            OnSquareOutlined(scorePerSquare, textPos);

            AudioController.Instance.Outline();

            Vector3 textPosAbsolute = transform.TransformPoint(textPos);

            int energyGain = energyPerSquare;
            energyCounter.AddEnergy(energyGain, false);
            energyCounter.PopUp(energyGain, textPosAbsolute, EnergyCounter.TextColor.Gain, reactorPos);

            scoreBlocks.ScoreBlockAdded(); //Fill a score block on the top bar

            yield return new WaitForSeconds(secondsBetweenSquareAnimations);

            if (outline != null)
            {
                Destroy(outline);
            }
        }

        //Remove all tiles that form squares if isPlaced is true
        foreach (Tile t in duplicatesRemoved)
        {
            t.Clear();
        }

        //int totalEnergyGain = 0;
        foreach (int[] square in squaresFormed)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform, false);
            explosion.GetComponent<RectTransform>().sizeDelta = new Vector2(3 * GetTileWidth(), 3 * GetTileHeight());
            explosion.transform.localPosition = GetTileAt(square[0] + 1, square[1] + 1).transform.localPosition;

            DrawLightning(square[0], square[1]);

            //energyCounter.AddEnergy(energyPerSquare);
            //totalEnergyGain += energyPerSquare;
        }
        // Only play the lightning sound once so that we don't destroy the player's ears.
        AudioController.Instance.Lightning();

        OnSquaresCleared();

        TurnTilesIntoVestiges(newVestiges);
        
        //Debug.Log("Grid.ClearingOutlineEffect: Reached the end of the coroutine.");

        //energyCounter.AddEnergy(totalEnergyGain);
        //energyCounter.PopUp(totalEnergyGain, reactorPos);

        blockSpawner.UpdateAllBlocks();
        blockSpawner.ProgressQueue();
    }

    private void DrawLightning(int r, int c)
    {
        Vector3 tilePos = GetTileAt(r + 1, c + 1).transform.position;
        //Vector3 energyTransferBallPos = energyTransferBallController.transform.position;
        Vector3 energyTransferBallPos = reactor.transform.position;
        float distance = Vector3.Distance(tilePos, energyTransferBallPos) / transform.parent.transform.localScale.y;

        GameObject lighting = Instantiate(energyTransferPrefab, transform.parent.transform);
        Vector3 lightingCenter = (energyTransferBallPos + tilePos) / 2f;
        lighting.transform.position = lightingCenter;

        float scaleY = distance / lighting.GetComponent<RectTransform>().rect.height;
        float scaleX = scaleY * 0.5f;
        lighting.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        float angle = (90 - Mathf.Atan((tilePos.y - energyTransferBallPos.y) / (tilePos.x - energyTransferBallPos.x)) * 180f / Mathf.PI);
        lighting.transform.rotation = Quaternion.Euler(0, 0, -angle);

        energyCounter.DoEnergyTransferBallAnimation();
    }

    private void CheckForSquares(int r, int c, int length, ref List<Tile> toRemove, ref List<int[]> squaresFormed, TileData.TileType[,] copy)
    {
        List<Tile> processed = new List<Tile>();
        //Do not check if the square is not possible
        //to be formed at this tile
        if (r + length - 1 < height && c + length - 1 < width)
        {
            int count = 0;
            int currentRow = r;
            bool isLegal = true;
            //The max number of tiles to be checked
            while (count < length * length)
            {
                if (copy == null)
                {
                    for (int i = c; i < c + length; i++)
                    {
                        //if (!tiles[currentRow, i].GetIsOccupied())
                        if (!TileData.GetIsClearableInSquare(tiles[currentRow, i].GetTileType()))
                        {
                            isLegal = false;
                            processed.Clear();
                            break;  //exit for loop
                        }
                        //Check to avoid repeated tiles
                        if (processed.Find(t => t == tiles[currentRow, i]) == null)
                            processed.Add(tiles[currentRow, i]);
                        count++;
                    }
                    if (!isLegal)
                        break;  //exit while loop
                }
                else
                {
                    for (int i = c; i < c + length; i++)
                    {
                        //if (copy[currentRow, i] == TileData.TileType.Unoccupied)
                        if (!TileData.GetIsClearableInSquare(copy[currentRow, i]))
                        {
                            isLegal = false;
                            processed.Clear();
                            break;  //exit for loop
                        }
                        //Check to avoid repeated tiles, and 
                        //include only tiles in the original tiles array
                        if (TileData.GetIsClearableInSquare(copy[currentRow, i]) &&
                            processed.Find(t => t == tiles[currentRow, i]) == null)
                            processed.Add(tiles[currentRow, i]);
                        count++;
                    }
                    if (!isLegal)
                        break;  //exit while loop
                }
                currentRow += 1;
            }

            if (isLegal)
            {
                //and also all outside adjacent regular
                //tiles will be turned in to vestiges (Mark the certain square).
                toRemove.AddRange(processed);
                squaresFormed.Add(new int[] { r, c, length });     
            }
        }
    }

    // Shorthand for a loop used in the FormVestiges method.
    private void FormVestigesInnerLoop(List<Tile> toRemove,
        TileData.TileType[,] copy, ref List<VestigeMarker> newVestiges,
        int inRow, int inCol)
    {
        if (copy == null)
        {
            if (toRemove.Find(t => t == tiles[inRow, inCol]) == null)
            {
                if (newVestiges.Find(v => v.GetTile() == tiles[inRow, inCol]) == null)
                {
                    int trueRow = inRow;
                    int trueCol = inCol;
                    Tile trueTile = tiles[trueRow, trueCol];
                    VestigeMarker vm = new VestigeMarker(trueTile, trueRow, trueCol);
                    newVestiges.Add(vm);
                }
            }
        }
        else
        {
            if (toRemove.Find(t => t == tiles[inRow, inCol]) == null
                && TileData.GetIsClearableInSquare(copy[inRow, inCol]))
            {
                tiles[inRow, inCol].SetAnticipatedHighlight(TileData.TileType.Vestige);
            }
        }
    }

    private void FormVestiges(int row, int col, int length, List<Tile> toRemove,
        TileData.TileType[,] copy, ref List<VestigeMarker> newVestiges)
    {
        //Turn all adjacent outside regular tiles into vestiges.
        //for the specified square.

        //Left edge
        if (col > 0)
        {
            for (int r = row; r < row + length; r++)
            {
                FormVestigesInnerLoop(toRemove, copy, ref newVestiges, r, col - 1);
            }
        }
        //Right edge
        if (col + length - 1 < width - 1)
        {
            for (int r = row; r < row + length; r++)
            {
                FormVestigesInnerLoop(toRemove, copy, ref newVestiges, r, col + length);
            }
        }
        //Top edge
        if (row > 0)
        {
            for (int c = col; c < col + length; c++)
            {
                FormVestigesInnerLoop(toRemove, copy, ref newVestiges, row - 1, c);
            }
        }
        //Bottom edge
        if (row + length - 1 < height - 1)
        {
            for (int c = col; c < col + length; c++)
            {
                FormVestigesInnerLoop(toRemove, copy, ref newVestiges, row + length, c);
            }
        }

        /*
        //Left edge
        if (col > 0)
            for (int r = row; r < row + length; r++)
            {
                if (copy == null)
                {
                    if (toRemove.Find(t => t == tiles[r, col - 1]) == null)
                    {
                        if (newVestiges.Find(v => v == tiles[r, col - 1]) == null)
                        {
                            int trueRow = r;
                            int trueCol = col - 1;
                            Tile trueTile = tiles[trueRow, trueCol];
                            VestigeMarker vm = new VestigeMarker(trueTile, trueRow, trueCol);
                            newVestiges.Add(vm);
                        }
                    }
                }
                else
                {
                    if (toRemove.Find(t => t == tiles[r, col - 1]) == null && TileData.GetIsClearableInSquare(copy[r, col - 1]))
                        tiles[r, col - 1].SetAnticipatedHighlight(TileData.TileType.Vestige);
                }
            }

        //Right edge
        if (col + length - 1 < width - 1)
            for (int r = row; r < row + length; r++)
            {
                if (copy == null)
                {
                    if (toRemove.Find(t => t == tiles[r, col + length]) == null)
                    {
                        if (newVestiges.Find(v => v == tiles[r, col + length]) == null)
                            newVestiges.Add(tiles[r, col + length]);
                    }
                }
                else
                {
                    if (toRemove.Find(t => t == tiles[r, col + length]) == null && TileData.GetIsClearableInSquare(copy[r, col + length]))
                        tiles[r, col + length].SetAnticipatedHighlight(TileData.TileType.Vestige);
                }
            }

        //Top edge
        if (row > 0)
            for (int c = col; c < col + length; c++)
            {
                if (copy == null)
                {
                    if (toRemove.Find(t => t == tiles[row - 1, c]) == null)
                    {
                        if (newVestiges.Find(v => v == tiles[row - 1, c]) == null)
                            newVestiges.Add(tiles[row - 1, c]);
                    }
                }
                else
                {
                    if (toRemove.Find(t => t == tiles[row - 1, c]) == null && TileData.GetIsClearableInSquare(copy[row - 1, c]))
                        tiles[row - 1, c].SetAnticipatedHighlight(TileData.TileType.Vestige);
                }
            }

        //Bottom edge
        if (row + length - 1 < height - 1)
            for (int c = col; c < col + length; c++)
            {
                if (copy == null)
                {
                    if (toRemove.Find(t => t == tiles[row + length, c]) == null)
                    {
                        if (newVestiges.Find(v => v == tiles[row + length, c]) == null)
                            newVestiges.Add(tiles[row + length, c]);
                    }
                }
                else
                {
                    if (toRemove.Find(t => t == tiles[row + length, c]) == null && TileData.GetIsClearableInSquare(copy[row + length, c]))
                        tiles[row + length, c].SetAnticipatedHighlight(TileData.TileType.Vestige);
                }
            }
        */
    }

    private GameObject DrawOutLine(int r, int c)
    {
        GameObject outline = Instantiate(outLinePrefab, transform, false);
        outline.GetComponent<RectTransform>().sizeDelta = new Vector2(3 * GetTileWidth(), 3 * GetTileHeight());
        outline.transform.localPosition = GetTileAt(r + 1, c + 1).transform.localPosition;
        return outline;
    }

    class Outline
    {
        GameObject[] corners;
        GameObject[] sides;
        int[] location;

        public Outline(GameObject[] corners, GameObject[] sides, int[] loc)
        {
            this.corners = corners;
            this.sides = sides;
            location = loc;
        }

        public int[] GetLocation()
        {
            return location;
        }

        public void DesctroyObject()
        {
            foreach (GameObject side in sides)
                Destroy(side);
            foreach (GameObject corner in corners)
                Destroy(corner);
        } 
    }

    /*
    private void SmallerSquareExclusion(ref List<int[]> squaresFormed)
    {
        //Sort it for reducing the number of loops
        squaresFormed = squaresFormed.OrderByDescending(s => s[2]).ToList();
        int index = 0;
        
        while (index < squaresFormed.Count)
        {
            //Perform exclusion when  a 4x4 or bigger square has been formed
            if (squaresFormed[index][2] > 3)
            {
                int row = squaresFormed[index][0];
                int col = squaresFormed[index][1];
                int length = squaresFormed[index][2];
                //Exclude all squares from upper left 
                for (int r = row; r < row + length - 2; r++)
                {
                    for (int c = col; c < col +length - 2; c++)
                    {
                        //Delete all the smaller squares start at this tile
                        List<int[]> smallers = squaresFormed.FindAll(s => s[0] == r && s[1] == c && s[2] < length);
                        foreach(int[] s in smallers)
                            squaresFormed.Remove(s);
                    }
                }
            }
            index++;
        }
    }
    */

    // Instantiates all Spaces on the Grid.
    void InstantiateSpaces()
    {
        InstantiateCertainSpaces(1, 1);
    }

    // Instantiates Spaces with certain dimensions.
    void InstantiateCertainSpaces(int h, int w)
    {
        List<Space> ts = new List<Space>();

        for (int col = 0; col < width; col += w)
        {
            for (int row = 0; row < height; row += h)
            {
                //Space s = Instantiate(prefabSpace).GetComponent<Space>();
                GameObject current = Instantiate(prefabSpace, transform, false);
                Space s = current.GetComponent<Space>();
                s.Init(row, col, h, w, this);
                //s.GetComponent<RectTransform>().SetParent(canvas.transform);
                ts.Add(s);
            }
        }

        spaces.Add(new Vector2(w, h), ts);
    }


    // Returns a List of all of the Spaces of a certain width and height by fetching from the “spaces” Dictionary
    public List<Space> GetSpaces(int width, int height)
    {
        return spaces[new Vector2(width, height)];
    }

    public List<Space> GetSpacesFree(int width, int height, Block block)
    {
        List<Space> potentialSpaces = spaces[new Vector2(width, height)];
        List<Space> freeSpaces = new List<Space>();
        foreach(Space s in potentialSpaces)
        {
            if (s.CanBlockFit(block))
                freeSpaces.Add(s);
        }
        return freeSpaces;
    }

    //Check if the Block can't fit into any of the Spaces, including block after rotation
    public bool CheckIfSpacesFilled(Block block)
    {
        List<Space> localSpaces;

        // Construct a temporary block copy so that the original does not get modified.
        Block testBlock = new Block(block);

        for (int flip = 0; flip < 2; ++flip)
        {
            for (int rotate = 0; rotate < 4; ++rotate)
            {
                localSpaces = GetSpacesFree(1, 1, testBlock);
                for (int i = 0; i < localSpaces.Count; i++)
                {
                    if (localSpaces[i].CanBlockFit(testBlock))
                    {
                        return false;
                    }
                }
                testBlock.Rotate(true);
            }
            testBlock.Flip();
        }

        return true;
    }

	// Returns the number of vestiges on the grid.
    public int CountVestiges() 
    {
        int vestigeNum = 0;
		for (int r = 0; r < height; r++)
		{
			for (int c = 0; c < width; c++)
			{
				if (tiles[r, c].GetTileType() == TileData.TileType.Vestige)
				{
					vestigeNum++;
				}
			}
		}
        return vestigeNum;
    }

    // To be called by the Space class whenever a new DraggableBlock is successfully placed on the Grid.
    public void PlacedDraggableBlock(Vector3 blockCenter)
    {
        TutorialController.Instance.TriggerEvent(TutorialController.Triggers.FIRST_BLOCK);

        consoleGrid.SetDraggableBlock(null);

        //If there was not a square formed this turn, then energy will be reduced by 1 plus number of vestiges
        if (!CheckForMatches())
        {
            // Set the current number of vestiges for analytics.
            vestigeCounter.SetCurrentVestiges(CountVestiges());

            int energyChange = GetEnergyDrain();
            if (energyChange != 0)
            {
                energyCounter.RemoveEnergy(energyChange, false);
                //energyCounter.PopUp(-energyChange, blockCenter, reactor.transform.position);

                AnimateVestigeEnergyLosses();
            }

            //Update Available spaces for all draggable blocks
            blockSpawner.UpdateAllBlocks();
            blockSpawner.ProgressQueue();
        }
        
        //Update turns played - this counts as a turn
        turnCounter.PlayedTurn();
        //Count vestiges again, even if a square was formed this turn - for analytics
        vestigeCounter.SetCurrentVestiges(CountVestiges());
    }

    // Get the energy drain of the tile at (row, col).
    public int GetVestigeEnergyDrain(int row, int col)
    {
        return decayRates[tiles[row, col].GetVestigeLevel() - 1];
    }

    // Get the current level of energy drain on the Grid.
    public int GetEnergyDrain()
    {
        int energyChange = baseEnergyDecayRate + baseEnergyDecayRateBonus;
        //Calculating total energy drain
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                if (tiles[r, c].GetTileType() == TileData.TileType.Vestige)
                {
                    energyChange += decayRates[tiles[r, c].GetVestigeLevel() - 1];
                }
            }
        }

        /*
        int w = consoleGrid.GetWidth();
        int h = consoleGrid.GetHeight();
        Tile[,] consoleTiles = consoleGrid.GetTiles();
        Debug.Log(consoleTiles);
        for (int r = 0; r < h; r++)
        {
            for (int c = 0; c < w; c++)
            {
                if (consoleTiles[r, c].GetTileType() == TileData.TileType.Vestige)
                {
                    energyChange += decayRates[consoleTiles[r, c].GetVestigeLevel() - 1];
                }
            }
        }
        */

        List<TileData> consoleVestiges = consoleGrid.GetVestiges();
        if (consoleVestiges != null)
        {
            foreach (TileData t in consoleVestiges)
            {
                energyChange += decayRates[t.GetVestigeLevel() - 1];
            }
        }

        return energyChange;
    }

    private List<VestigeMarker> GetAllVestiges()
    {
        List<VestigeMarker> result = new List<VestigeMarker>();
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                Tile theTile = tiles[r, c];
                if (theTile.GetTileType() == TileData.TileType.Vestige)
                {
                    VestigeMarker vm = new VestigeMarker(theTile, r, c);
                    result.Add(vm);
                }
            }
        }
        return result;
    }

    IEnumerator AnimateVestigeEnergyLossOneByOne()
    {
        List<VestigeMarker> vestiges = GetAllVestiges();
        foreach (VestigeMarker vm in vestiges)
        {
            AnimateVestigeEnergyLoss(vm);
            Tile theTile = vm.GetTile();
            Vector3 vestigePos = theTile.transform.position;
            int levelIndex = theTile.GetVestigeLevel() - 1;
            int energyChange = decayRates[levelIndex];
            Vector3 targetPos = transformAccumulator.position;

            EnergyCounter.TextColor[] colors = {
                EnergyCounter.TextColor.Vestige1,
                EnergyCounter.TextColor.Vestige2,
                EnergyCounter.TextColor.Vestige3
            };

            energyCounter.PopUp(-energyChange, vestigePos, colors[levelIndex], targetPos);

            //gameOver.ResetGameOverWaitTime();
            yield return new WaitForSeconds(secondsBetweenVestigeEnergyLossAnimations);
        }
        //gameOver.TryStartGameOverWaitTimer();
    }

    // Instantiate the energy loss animation for all vestiges on the Grid.
    public void AnimateVestigeEnergyLosses()
    {
        /*
        List<VestigeMarker> vestiges = GetAllVestiges();
        foreach (VestigeMarker vm in vestiges)
        {
            AnimateVestigeEnergyLoss(vm);
        }
        AnimateReactorEnergyLoss();
        */

        StartCoroutine(AnimateVestigeEnergyLossOneByOne());
    }

    // Animate the vestige energy loss on the given Tile.
    public void AnimateVestigeEnergyLoss(int row, int col)
    {
        Vector3 pos = GetTilePosition(row, col);
        int levelIndex = tiles[row, col].GetVestigeLevel() - 1;
        GameObject obj = Instantiate(prefabEnergyLossVestige[levelIndex], transform, false);
        obj.transform.localPosition = pos;
    }

    private void AnimateVestigeEnergyLoss(VestigeMarker vm)
    {
        AnimateVestigeEnergyLoss(vm.GetRow(), vm.GetCol());
    }

    public void AnimateReactorEnergyLoss()
    {
        GameObject reactorAnim = Instantiate(prefabEnergyLossReactor, transform, false);
        Vector3 reactorPos = reactor.transform.position;
        reactorAnim.transform.position = reactorPos;
    }

    // Removes a GridBlock from the List of GridBlocks.
    public void RemoveGridBlock(GridBlock gb)
    {
        gridBlocks.Remove(gb);
    }

    public void SetBaseEnergyDecayRateBonus(int newVal)
    {
        baseEnergyDecayRateBonus = newVal;
    }

    public delegate bool TileCheckCondition(Tile t);
    public List<Tile> GetReferencesToTiles(TileCheckCondition condition, int[,] mask = null)
    {
        List<Tile> result = new List<Tile>();
        for (int r = 0; r < height; ++r)
        {
            for (int c = 0; c < width; ++c)
            {
                // For each part of the mask that's 1, don't return the reference.
                if (mask != null)
                {
                    if (mask[r, c] == 1)
                    {
                        continue;
                    }
                }
                Tile t = GetTileAt(r, c);
                //if (t.GetTileType() == type)
                if (condition(t))
                {
                    result.Add(t);
                }
            }
        }
        return result;
    }

    public List<Tile> GetReferencesToOccupiedTiles()
    {
        List<Tile> result = new List<Tile>();
        foreach (Tile t in tiles)
        {
            if (t.GetIsOccupied())
            {
                result.Add(t);
            }
        }
        return result;
    }

    // Randomly adds a given number of asteroids to the Grid.
    public void AddAsteroids(int asteroidCount, int tier)
    {
        int[,] asteroidMask = asteroidMasks[tier];
        int asteroidsAdded = 0;
        List<Tile> refs = GetReferencesToTiles((Tile t) => t.GetIsOccupied() == false, asteroidMask);
        // If asteroids can spawn in filled cells, add the occupied Tiles to the refs List.
        if (asteroidsCanSpawnInFilledCells)
        {
            //List<Tile> occupieds = GetReferencesToOccupiedTiles(asteroidMask);
            List<Tile> occupieds = GetReferencesToTiles((Tile t) => t.GetIsOccupied() == true, asteroidMask);
            refs.AddRange(occupieds);
        }

        while (asteroidsAdded < asteroidCount && refs.Count != 0)
        {
            int index = UnityEngine.Random.Range(0, refs.Count);
            Tile v = refs[index];
            v.Fill(TileData.TileType.Asteroid);
            refs.RemoveAt(index);
            ++asteroidsAdded;
        }

        blockSpawner.ForceUpdateSpaceInformation();
    }

    // Clear all asteroids on the Grid.
    public void ClearAllAsteroids()
    {
        foreach (Tile t in tiles)
        {
            if (t.GetTileType() == TileData.TileType.Asteroid)
            {
                t.Clear();
            }
        }

        blockSpawner.ForceUpdateSpaceInformation();
    }

    // Callback function for when a tiletype is changed.
    /*
    private void Tile_Changed(TileData.TileType newType)
    {
        //If a type is changed to Unoccupied, then add energyPerCell energy
        if (newType == TileData.TileType.Unoccupied)
        {
            energyCounter.AddEnergy(energyPerCell);
        }
    }
    */

    public int GetSquareClearingsCount()
    {
        return squareClearingsCount;
    }

    public int GetScorePerSquare()
    {
        return scorePerSquare;
    }

    private void OnSquareFormed(int scorePerSquare, Vector3 textPos)
    {
        if (SquareFormed != null)
        {
            SquareFormed(scorePerSquare, textPos);
        }
    }

    private void OnSquareOutlined(int scorePerSquare, Vector3 textPos)
    {
        if (SquareOutlined != null)
        {
            SquareOutlined(scorePerSquare, textPos);
        }
    }

    private void OnSquaresCleared()
    {
        if (SquaresCleared != null)
        {
            SquaresCleared();
        }
    }
    public void ForceOnSquaresCleared()
    {
        OnSquaresCleared();
    }
}