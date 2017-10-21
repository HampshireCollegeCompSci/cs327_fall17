// Author(s): Paul Calande, Yifeng Shi, Yixiang Xu, Wm. Josiah Erikson
// A 2-dimensional collections of Tiles

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Grid : MonoBehaviour
{
    public delegate void SquareFormedHandler(int size, Vector3 textPos);
    public event SquareFormedHandler SquareFormed;

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
    [Tooltip("Energy earned per Tile cleared. Populated by JSON.")]
    int energyPerCell = 1;
    [SerializeField]
    [Tooltip("Reference to the RectTransform component of this Grid.")]
    RectTransform rectTransform;
    [SerializeField]
    [Tooltip("Reference to energy gain animator.")]
    Animator energyGainController;
    [SerializeField]
    [Tooltip("Whether or not asteroids can spawn in filled cells.")]
    bool asteroidsCanSpawnInFilledCells;
    [SerializeField]
    [Tooltip("Placeholder sprite for square outline")]
    GameObject outLinePrefab;
    [SerializeField]
    [Tooltip("The underlying array of Tiles.")]
    Tile[,] tiles;

    // The width of one Tile, calculated compared to the Grid's dimensions.
    private float tileWidth;
    // The height of one Tile, calculated compared to the Grid's dimensions.
    private float tileHeight;

    Dictionary<Vector2, List<Space>> spaces = new Dictionary<Vector2, List<Space>>();

    List<GridBlock> gridBlocks;

    List<Outline> outlines;

    SnapLocation prevSnapLocation;

    //Four lists storing lists of four direction of L-shapes respectively.
    //List<LShape> topLeft;
    //List<LShape> topRight;
    //List<LShape> bottomLeft;
    //List<LShape> bottomRight;

    class Outline
    {
        Vector3[] vertices = new Vector3[4];
        int[] location = new int[3];
        GameObject[] outlineObj;
        int[] nextPos;
        float?[] prevDistance;

        public Outline(GameObject[] obj, int[] loc, Vector3[] ver, int[] nPos)
        {
            outlineObj = obj;
            location = loc;
            vertices = ver;
            nextPos = nPos;

            for (int i = 0; i < 4; i++)
            {
                outlineObj[i].transform.localPosition = vertices[i];
            }

            prevDistance = new float?[4]{ null, null, null, null };
        }

        public void ChangeTarget(int i)
        {
            if (nextPos[i] == 3)
                nextPos[i] = 0;
            else
                nextPos[i]++;      
        }

        public GameObject[] GetOutlineObject()
        {
            return outlineObj;
        }

        public int[] GetLocation()
        {
            return location;
        }

        public Vector3[] GetVertices()
        {
            return vertices;
        }

        public int[] NextPos()
        {
            return nextPos;
        }

        public void DesctroyObject()
        {
            foreach (GameObject obj in outlineObj)
                Destroy(obj);
        }

        public float?[] PrevDistance()
        {
            return prevDistance;
        }
    }

    private void Tune()
    {
        var json = JSON.Parse(tuningJSON.ToString());
        width = json["grid width"].AsInt;
        height = json["grid height"].AsInt;
        baseEnergyDecayRate = json["base energy decay rate"].AsInt;
        decayRates = json["vestige decay rates"].AsArray;
        vestigeMaxLevel = json["vestige max level"].AsInt;
        energyPerCell = json["energy per cell cleared"].AsInt;
        asteroidsCanSpawnInFilledCells = json["asteroids can spawn in filled cells"].AsBool;
    }

    private void Start()
    {
        Tune();

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
            t.Changed += Tile_Changed;
        }

        //Instantiate spaces
        InstantiateSpaces();
        /*
        spaces = new Space[width, height];
		for (int c = 0; c < width; c++)
		{
			for (int r = 0; r < height; r++)
			{
				//Need to be changed after knowing specific positions
				GameObject currentPrefabSpace = Instantiate(prefabSpace);
                spaces[r, c] = currentPrefabSpace.GetComponent<Space>();
                //Need to be changed after knowing specific positions.
                spaces[r, c].Init(0, 0, this);
			}
		}
        */

        // Initialize BlockSpawner.
        blockSpawner.Init();
        //blockSpawner.Init(this);

        //Instantiate GridBlocks
        gridBlocks = new List<GridBlock>();

        outlines = new List<Outline>();
    }

    private void Update()
    {
        if (outlines.Count > 0)
        {
            for (int i = 0; i < outlines.Count; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    //GameObject obj = outlines[i].GetOutlineObject()[j];
                    Vector3 currentPos = outlines[i].GetOutlineObject()[j].transform.localPosition;
                    Vector3 nextPos = outlines[i].GetVertices()[outlines[i].NextPos()[j]];

                    //Checking if the distance from current position to the next position actually
                    //getting large. If this is true, we fix the position and switch to next target.
                    if (outlines[i].PrevDistance()[j] == null)
                        outlines[i].PrevDistance()[j] = Vector3.Distance(currentPos, nextPos);

                    if (outlines[i].PrevDistance()[j] < Vector3.Distance(currentPos, nextPos))
                    {
                        outlines[i].GetOutlineObject()[j].transform.localPosition = nextPos;
                        outlines[i].ChangeTarget(j);
                    }

                    outlines[i].PrevDistance()[j] = Vector3.Distance(currentPos, nextPos);
                    Vector3 direction = Vector3.right;
                    switch (outlines[i].NextPos()[j])
                    {
                        case 0:
                            direction = Vector3.up;
                            break;
                        case 1:
                            direction = Vector3.right;
                            break;
                        case 2:
                            direction = Vector3.down;
                            break;
                        case 3:
                            direction = Vector3.left;
                            break;
                    }
                    outlines[i].GetOutlineObject()[j].transform.Translate(direction * Time.deltaTime * 700);
                }           
            }
        }
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

    /*
    private class Coordinate
    {
        int x, y;
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }
    }
    */

    // IMPORTANT: Don't forget about the additional WriteBlock overload below!
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
        GridBlock gb = new GridBlock(row, col, block.GetBlock(), this);
        gridBlocks.Add(gb);

        //call LShapeCheck after each insertion
        //LShapeCheck(coords);

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

    public void ClearOutline()
    {
        outlines.ForEach(o => o.DesctroyObject());
        outlines.Clear();
    }

    public void AnticipatedHighlight(int row, int col, DraggableBlock newBlock, bool on, SnapLocation snapLocation)
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
                //Exclude smaller squares
                SmallerSquareExclusion(ref anticipatedPotentialSquares);

                List<Tile> toVestiges = new List<Tile>(); //No effect. Just match parameter.
                //Highligh anticipated vestiges
                foreach (int[] s in anticipatedPotentialSquares)
                    FormVestiges(s[0], s[1], s[2], anticipatedSquareTiles, copy, ref toVestiges);

                if (prevSnapLocation == null)
                {
                    foreach (int[] s in anticipatedPotentialSquares)
                        DrawOutLine(s[0], s[1], s[2]);

                    prevSnapLocation = snapLocation;
                }
                else
                {
                    if (prevSnapLocation.gameObject.transform.position != snapLocation.gameObject.transform.position)
                    {
                        foreach (int[] s in anticipatedPotentialSquares)
                            DrawOutLine(s[0], s[1], s[2]);

                        prevSnapLocation = snapLocation;
                    }
                }

                //Highlight all tiles that will form squares
                foreach (Tile t in anticipatedSquareTiles)
                {
                    t.SetAnticipatedHighlight(TileData.TileType.Regular);
                }
            }
            else
            {
                ClearOutline();
            }
        }
        else
        {
            if (prevSnapLocation != snapLocation)
            {
                ClearOutline();
                prevSnapLocation = null;
            }

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

    public bool CheckForMatches()
    {
        //Debug.Log("Checking for matches...");

        int biggestSquareSize = Mathf.Min(width, height);

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
                    // Check for squares from length 3 upwards.
                    for (int length = 3; length <= biggestSquareSize; length++)
                    {
                        CheckForSquares(r, c, length, ref toRemove, ref squaresFormed, null);
                    }
                }
            }
        }

        //If toRemove is not empty, then there is at least one square formed
        if (toRemove.Count != 0)
        {
            squareFormed = true;

            //Exclude smaller squares
            SmallerSquareExclusion(ref squaresFormed);
            //Turn vestiges when isPlaced is true
            List<Tile> newVestiges = new List<Tile>();
            foreach (int[] s in squaresFormed)
            {
                //Spawn a text indicating scores at the center of the cleared square
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
                //If a legal square is formed, tell the event handler,
                OnSquareFormed(s[2], textPos);

                FormVestiges(s[0], s[1], s[2], toRemove, null, ref newVestiges);
            }

            //Upgrading vestiges
            foreach (Tile v in newVestiges)
            {
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
            }

            //Remove all tiles that form squares if isPlaced is true
            foreach (Tile t in toRemove)
            {
                t.Clear();
            }
            List<Tile> duplicatesRemoved = toRemove.Distinct().ToList();
            energyCounter.PopUp("+", duplicatesRemoved.Count);
            energyGainController.SetBool("active", true);
            energyGainController.SetInteger("energyGained", duplicatesRemoved.Count);
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
                /*
                else //Rest of rows just check two tiles
                {
                    if(copy == null)
                    {
                        if (tiles[currentRow, c].GetTileType() != TileData.TileType.Regular ||
                        tiles[currentRow, c + length - 1].GetTileType() != TileData.TileType.Regular)
                        {
                            isLegal = false;
                            processed.Clear();
                            break;  //exit while loop
                        }
                        if (processed.Find(t => t == tiles[currentRow, c]) == null)
                            processed.Add(tiles[currentRow, c]);
                        if (processed.Find(t => t == tiles[currentRow, c + length - 1]) == null)
                            processed.Add(tiles[currentRow, c + length - 1]);
                        count += 2;
                    }
                    else
                    {
                        if (copy[currentRow, c] != TileData.TileType.Regular ||
                        copy[currentRow, c + length - 1] != TileData.TileType.Regular)
                        {
                            isLegal = false;
                            processed.Clear();
                            break;  //exit while loop
                        }
                        if (tiles[currentRow, c].GetTileType() == TileData.TileType.Regular &&
                            processed.Find(t => t == tiles[currentRow, c]) == null)
                            processed.Add(tiles[currentRow, c]);
                        if (tiles[currentRow, c + length - 1].GetTileType() == TileData.TileType.Regular && 
                            processed.Find(t => t == tiles[currentRow, c + length - 1]) == null)
                            processed.Add(tiles[currentRow, c + length - 1]);
                        count += 2;
                    }
                    
                }
                */
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

    private void FormVestiges(int row, int col, int length, List<Tile> toRemove, TileData.TileType[,] copy, ref List<Tile> newVestiges)
    {
        //Turn all adjacent outside regular tiles into vestiges.
        //for the specified square.
        
        //Left edge
        if (col > 0)
            for (int r = row; r < row + length; r++)
            {
                if(copy == null)
                {
                    if (toRemove.Find(t => t == tiles[r, col - 1]) == null)
                    {
                        if (newVestiges.Find(v => v == tiles[r, col - 1]) == null)
                            newVestiges.Add(tiles[r, col - 1]);
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
     
    }

    private void DrawOutLine(int r, int c, int length)
    {
        if (outlines.Find(o => o.GetLocation()[0] == r && o.GetLocation()[1] == c && o.GetLocation()[2] == length) == null)
        {
            GameObject[] outlineObjs = new GameObject[4];
            float randomR = UnityEngine.Random.Range(0, 1.0f);
            float randomG = UnityEngine.Random.Range(0, 1.0f);
            float randomB = UnityEngine.Random.Range(0, 1.0f);
            for (int i = 0; i < 4; i++)
            {
                GameObject obj = Instantiate(outLinePrefab, transform, false);
                obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                obj.GetComponent<Image>().color = new Color(randomR, randomG, randomB);
                outlineObjs[i] = obj;
            }

            float adjustX = GetTileWidth() / 2;
            float adjustY = GetTileHeight() / 2;
            Vector3 posUpperLeft = new Vector3(tiles[r, c].transform.localPosition.x - adjustX,
                                                          tiles[r, c].transform.localPosition.y + adjustY,
                                                          tiles[r, c].transform.localPosition.z);
            Vector3 posUpperRight = new Vector3(posUpperLeft.x + length * GetTileWidth(), posUpperLeft.y, posUpperLeft.z);
            Vector3 posLowerRight = new Vector3(posUpperRight.x, posUpperRight.y - length * GetTileHeight(), posUpperRight.z);
            Vector3 posLowerLeft = new Vector3(posLowerRight.x - length * GetTileWidth(), posLowerRight.y, posLowerRight.z);
            Vector3[] vertices = new Vector3[] { posUpperLeft, posUpperRight, posLowerRight, posLowerLeft };

            outlines.Add(new Outline(outlineObjs, new int[] { r, c, length },vertices, new int[] { 1, 2, 3, 0}));          
        }
        
    }

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
                    for (int c = col; c < col + row +length - 2; c++)
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

    /*
    private List<Tile> MarkInsideTiles(int row, int col, int length)
    {
        List<Tile> inside = new List<Tile>();
        //Looping inside the square
        for (int r = row + 1; r < row + length - 1; r++)
        {
            for (int c = col + 1; c < col + length - 1; c++)
            {
                if (!tiles[r, c].GetIsOccupied())
                {
                    inside.Clear();
                    return inside;
                }
                                
                if (tiles[r, c].GetIsOccupied() && inside.Find(t => t == tiles[r, c]) == null)
                    inside.Add(tiles[r, c]);
            }
        }


        return inside;
    }
    */

    /*
    public void CheckForMatches()
    {
        /*
         * To best minimize the amount of calculation, we will firstly
         * check if there are L-Shaped formations (all squares consist of
         * four L-shaped formations with different directions). At each time
         * player inserting a block, we check the L-shaped formations consist
         * of new inserted regualar tiles and put these formations (if there is)
         * into corresponding lists. Then we could just loop either one of the
         * four list and check if specific L-shapes exist or not in other 3 lists
         * so we could know if there's potential squares. After this step we 
         * check 4 edges of the potential squares, remove them if all
         * 4 edges are filled, do nothing otherwise.
         * 
         * Note: not yet considering vestiges.
         */


    //Clear columns:
    /*
        for (int c = 0; c < width; c++)
        {
            bool isFilled = true;
            for (int r = 0; r < height; r++)
            {
                if(!tiles[r, c].GetIsOccupied())
                    isFilled = false;
            }
            if(isFilled)
            {
				for (int r = 0; r < height; r++)
				{
                    tiles[r, c].Clear();
				}
            }
        }

		//Clear rows:
		for (int r = 0; r < height; r++)
		{
			bool isFilled = true;
            for (int c = 0; c < width; c++)
			{
				if (!tiles[r, c].GetIsOccupied())
					isFilled = false;
			}
			if (isFilled)
			{
                for (int c = 0; c < width; c++)
				{
					tiles[r, c].Clear();
				}
			}
		}

    }

    private class LShape
    {
        //Simple Data structure used to facilitize CheckForMatches().
        Tile center;     //The tile at the center of L (the elbow).
        int direction;   // Indicate direction. 1 = topLeft, 2 = topRight, 3 = bottomLeft, 4 = bottomRight
        Coordinate coordinate;

        public LShape(Tile ctr, int dir, Coordinate cod)
        {
            center = ctr;
            direction = dir;
            coordinate = cod;
        }

        public Tile GetCenter()
        {
            return center;
        }

        public int GetDirection()
        {
            return direction;
        }

        public Coordinate GetCoordinate()
        {
            return coordinate;
        }
    }

    private void LShapeCheck(List<Coordinate> inserted)
    {

        //Check new L-shaped formations consist of
        //newly inserted tiles
        foreach(Coordinate c in inserted)
        {
            //Top-left
            if (tiles[c.GetY() + 1, c.GetX()].GetTileType() == Tile.TileType.Regular
                && tiles[c.GetY(), c.GetX() + 1].GetTileType() == Tile.TileType.Regular)
                topLeft.Add(new LShape(tiles[c.GetY(), c.GetX()], 1, c));
            //Top-right
            if(c.GetX() - 1 >= 0)
                if (tiles[c.GetY() + 1, c.GetX()].GetTileType() == Tile.TileType.Regular
                    && tiles[c.GetY(), c.GetX() - 1].GetTileType() == Tile.TileType.Regular)
                    topRight.Add(new LShape(tiles[c.GetY(), c.GetX()], 2, c));
            //Bottom-left
            if (c.GetY() - 1 >= 0)
                if (tiles[c.GetY() - 1, c.GetX()].GetTileType() == Tile.TileType.Regular
                && tiles[c.GetY(), c.GetX() + 1].GetTileType() == Tile.TileType.Regular)
                    topLeft.Add(new LShape(tiles[c.GetY(), c.GetX()], 3, c));
            //Bottom-right
            if (c.GetX() - 1 >= 0 && c.GetY() - 1 >= 0)
                if (tiles[c.GetY() - 1, c.GetX()].GetTileType() == Tile.TileType.Regular
                && tiles[c.GetY(), c.GetX() - 1].GetTileType() == Tile.TileType.Regular)
                    topLeft.Add(new LShape(tiles[c.GetY(), c.GetX()], 4, c));
        }

    }

    private void PotentialSquareCheck()
    {
        //Check if potential squares (with 4 direction L-shapes) exist.

        //Start from the topLeft list
        foreach(LShape i in topLeft)
        {
            //Record possible square with specific length for current LShape
            List<int> potentialSquareLength = new List<int>();

            int ix = i.GetCoordinate().GetX();
            int iy = i.GetCoordinate().GetY();

            //Process diagonal LShapes to get the first version of potential lengths
            foreach (LShape j in bottomRight)
            {
                if(j.GetCoordinate().GetX() - ix == j.GetCoordinate().GetY() - iy)
                {
                    //Not plus 1 because we will only use the difference between those two 
                    //coordinate but not the actual length
                    potentialSquareLength.Add(j.GetCoordinate().GetX() - ix);
                }
            }

            //Process topRight and bottomLeft to cut unqualified length
            int index = 0;
            while(true)
            {
                //Stop processing when index has already pointed to the last one
                if (potentialSquareLength.Count == 0 || index >= potentialSquareLength.Count)
                    break;
                //If any of them does not have a qualified LShape remove this one from the list.
                //Not increment index here because removing the current one actually make it point 
                //to the next one.
                //Otherwise, by increament index to process the next one.
                if(topRight.Find(ls => ls.GetCoordinate().GetX() - ix == potentialSquareLength[index]) == null
                    || bottomLeft.Find(ls => ls.GetCoordinate().GetX() - ix == potentialSquareLength[index]) == null)
                {
                    potentialSquareLength.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            //Do edge checking for each topLeft LShapes
            EdgeCheck(i, potentialSquareLength);
        }
    }

    private void EdgeCheck(LShape tl, List<int> lens)
    {
        //Check if 4 edges of the potential square are filled.
        if (lens.Count == 0)
            return;
        foreach(int len in lens)
        {
            if (len <= 3) //For square length 2 to 4 there is no need to check
                MarkToRemove();
            //Check top edge
            SingleEdgeCheck(tl.GetCoordinate().GetX() + 2, len - 1, 0, false);
            //Check bottom edge
            SingleEdgeCheck(tl.GetCoordinate().GetX() + 2, len - 1, len, false);
            //Check left edge
            SingleEdgeCheck(tl.GetCoordinate().GetY() + 2, len - 1, 0, true);
            //Check right edge
            SingleEdgeCheck(tl.GetCoordinate().GetY() + 2, len - 1, len, true);
        }
    }

    private bool SingleEdgeCheck(int start, int end, int other, bool direction)
    {
        //other: when horizontal, other = y value; otherwhise x value.
        //direction: false = horizontal, true = vertical
        for (int i = start; i < end; i++)
        {
            if (!direction)
            {
                //When horizontal:
                if (tiles[other, i].GetTileType() != Tile.TileType.Regular)
                    return false;
            }
            else
            {
                //When vertical:
                if (tiles[i, other].GetTileType() != Tile.TileType.Regular)
                    return false;
            }
        }
        return true;
    }

    private void MarkToRemove()
    {
        
    }

    private void SquareRemoval()
    {

    }
    */

    /*
    public void MoveAllBlocks(Enums.Direction direction)
    {   	
        switch(direction){
            case Enums.Direction.Right:
                int[] pushRightStatus = new int[height];
                //Loop column from last to 2nd. 1st column does not need to
                //be checked because no further column with be moved into that
                //column
                for (int c = width - 1; c > 0; c--)
                {
                    for (int r = 0; r < height; r++)
                    {
                        //Assign the state of tiles in this column.
                        //1 = occupied, 0 = empty
                        if (tiles[r, c].GetIsOccupied())
                            pushRightStatus[r] = 1;
                        else
                            pushRightStatus[r] = 0;
                    }
                    //Move c-1 th column to cth column.
                    //After this step the c-1 th column is up to date
                    //and is ready for the next loop
                    for (int r = 0; r < height; r++)
                    {
                        if (tiles[r, c - 1].GetIsOccupied() && pushRightStatus[r] == 0)
                        {
                            tiles[r, c].Fill();
                            tiles[r, c - 1].Clear();
                        }
                    }
                }
                break;
            case Enums.Direction.Down:
                int[] pushDownStatus = new int[width];

				//Loop row from last to 2nd. 1st row does not need to
				//be checked because no further row with be moved into that
				//row
				for (int r = height - 1; r > 0; r--)
				{
                    for (int c = 0; c < width; c++)
					{
						//Assign the state of tiles in this column.
						//1 = occupied, 0 = empty
						if (tiles[r, c].GetIsOccupied())
							pushDownStatus[c] = 1;
						else
							pushDownStatus[c] = 0;
					}
					//Move r-1 th row to rth row.
					//After this step the r-1 th row is up to date
					//and is ready for the next loop
					for (int c = 0; c < width; c++)
					{
						if (tiles[r - 1, c].GetIsOccupied() && pushDownStatus[c] == 0)
						{
							tiles[r, c].Fill();
							tiles[r - 1, c].Clear();
						}
					}
				}
                break;
            case Enums.Direction.Left:
				int[] pushLeftStatus = new int[height];
				//Loop column from 1st to width-1 th. last column does not need to
				//be checked because no further column with be moved into that
				//column
				for (int c = 0; c < width - 1; c++)
				{
					for (int r = 0; r < height; r++)
					{
						//Assign the state of tiles in this column.
						//1 = occupied, 0 = empty
						if (tiles[r, c].GetIsOccupied())
							pushLeftStatus[r] = 1;
						else
							pushLeftStatus[r] = 0;
					}
					//Move c+1 th column to cth column.
					//After this step the c+1 th column is up to date
					//and is ready for the next loop
					for (int r = 0; r < height; r++)
					{
						if (tiles[r, c + 1].GetIsOccupied() && pushLeftStatus[r] == 0)
						{
							tiles[r, c].Fill();
							tiles[r, c + 1].Clear();
						}
					}
				}
                break;
            case Enums.Direction.Up:
				int[] pushUpStatus = new int[width];
                //Loop row from 1st to height-1 th. last row does not need to
                //be checked because no further row with be moved into that
                //row
                for (int r = 0; r < height - 1; r++)
				{
					for (int c = 0; c < width; c++)
					{
						//Assign the state of tiles in this column.
						//1 = occupied, 0 = empty
						if (tiles[r, c].GetIsOccupied())
							pushUpStatus[c] = 1;
						else
							pushUpStatus[c] = 0;
					}
					//Move r+1 th row to rth row.
					//After this step the r+1 th row is up to date
					//and is ready for the next loop
					for (int c = 0; c < width; c++)
					{
						if (tiles[r + 1, c].GetIsOccupied() && pushUpStatus[c] == 0)
						{
							tiles[r, c].Fill();
							tiles[r + 1, c].Clear();
						}
					}
				}
                break;
        }

        CheckForMatches();
    }
    */

    // Instantiates all Spaces on the Grid.
    void InstantiateSpaces()
    {
        InstantiateCertainSpaces(1, 1);
        /*InstantiateCertainSpaces(1, 2);
        InstantiateCertainSpaces(2, 1);
        InstantiateCertainSpaces(2, 2);*/
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
    private int CountVestiges () 
    {
        int vestigeNum = 0;
		//Count the number of vestiges on the grid
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
    public void PlacedDraggableBlock()
    {    
        //If there was not a square formed this turn, then energy will be reduced by 1 plus number of vestiges
        if (!CheckForMatches())
        {
            int vestigeNum = CountVestiges();
            vestigeCounter.SetCurrentVestiges(vestigeNum); //Set the current number of vestiges for analytics
            int energyChange = baseEnergyDecayRate + baseEnergyDecayRateBonus;
            //Calculating total energy drain
            for (int r = 0; r < height; r++)
                for (int c = 0; c < width; c++)
                    if (tiles[r, c].GetTileType() == TileData.TileType.Vestige)
                        energyChange += decayRates[tiles[r, c].GetVestigeLevel() - 1];

            energyCounter.RemoveEnergy(energyChange);
            energyCounter.PopUp("-", energyChange);
        }

        blockSpawner.ProgressQueue();
        //Update Available spaces for all draggable blocks
        blockSpawner.UpdateAllBlocks();
        //Update turns played - this counts as a turn
        turnCounter.PlayedTurn();
        //Count vestiges again, even if a square was formed this turn - for analytics
        vestigeCounter.SetCurrentVestiges(CountVestiges());
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

    public List<Tile> GetReferencesToType(TileData.TileType type)
    {
        List<Tile> result = new List<Tile>();
        foreach (Tile t in tiles)
        {
            if (t.GetTileType() == type)
            {
                result.Add(t);
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
    public void AddAsteroids(int asteroidCount)
    {
        int asteroidsAdded = 0;
        List<Tile> refs = GetReferencesToType(TileData.TileType.Unoccupied);
        // If asteroids can spawn in filled cells, add the occupied Tiles to the refs List.
        if (asteroidsCanSpawnInFilledCells)
        {
            List<Tile> occupieds = GetReferencesToOccupiedTiles();
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
    private void Tile_Changed(TileData.TileType newType)
    {
        //If a type is changed to Unoccupied, then add energyPerCell energy
        if (newType == TileData.TileType.Unoccupied)
        {
            energyCounter.AddEnergy(energyPerCell);
        }
    }

    private void OnSquareFormed(int size, Vector3 textPos)
    {
        if (SquareFormed != null)
        {
            SquareFormed(size, textPos);
        }
    }
}
