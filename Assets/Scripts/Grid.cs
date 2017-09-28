// Author(s): Paul Calande, Yifeng Shi, Yixiang Xu
// A 2-dimensional collections of Tiles

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public delegate void SquareFormedHandler(int size, Vector3 textPos);
    public event SquareFormedHandler SquareFormed;

    [SerializeField]
    int width;
    [SerializeField]
    int height;
    [SerializeField]
    GameObject prefabTile;
    [SerializeField]
    GameObject prefabSpace;
    [SerializeField]
    BlockSpawner blockSpawner;
    [SerializeField]
    EnergyCounter energyCounter;

    Tile[,] tiles;

    Dictionary<Vector2, List<Space>> spaces = new Dictionary<Vector2, List<Space>>();

    List<GridBlock> gridBlocks;

    //Four lists storing lists of four direction of L-shapes respectively.
    //List<LShape> topLeft;
    //List<LShape> topRight;
    //List<LShape> bottomLeft;
    //List<LShape> bottomRight;

    private void Start()
    {
        //Instantiate tiles array
        tiles = TileUtil.CreateTileArray(prefabTile, transform, Vector3.zero, height, width);

        foreach (Tile t in tiles)
        {
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
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
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

    public GridBlock WriteBlock(int row, int col, Block block)
    {
        //List<Coordinate> coords = new List<Coordinate>();
        for (int c = 0; c < block.GetWidth(); c++)
        {
            for (int r = 0; r < block.GetHeight(); r++)
            {
                if (block.GetIsOccupied(r, c))
                {
                    tiles[row + r, col + c].Fill(block.GetTileType(r, c));
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

    public bool CheckForMatches()
    {
        //Debug.Log("Checking for matches...");

        int biggestSquareSize = Mathf.Min(width, height);

        bool squareFormed = false;

        // List keeping all tiles that are going to be removed.
        // We do remove after calculation because it's possible to remove
        // multiple squares together.
        List<Tile> toRemove = new List<Tile>();

        //Loop through all blocks from top left.
        //Consider only the case that the current tile is the top-left corner.
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                // Proceed only if the Tile is regular tile.
                if (tiles[r, c].GetTileType() == TileData.TileType.Regular)
                {
                    // Check for squares from length 3 upwards.
                    for (int length = 3; length <= biggestSquareSize; length++)
                    {
                        toRemove.AddRange(CheckForSquares(r, c, length));
                    }
                }
            }
        }

        //If toRemove is not empty, then there is at least one square formed
        if (toRemove.Count != 0)
        {
            squareFormed = true;
        }

        //Remove all tiles that form squares
        foreach (Tile t in toRemove)
        {
            
            t.Clear();
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

    private List<Tile> CheckForSquares(int r, int c, int length)
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
            while (count < length * length - (length - 2) * (length - 2) - 1)
            {
                //For first and last rows check the whole line
                if (currentRow == r || currentRow == r + length - 1)
                {
                    for (int i = c; i < c + length; i++)
                    {
                        if (tiles[currentRow, i].GetTileType() != TileData.TileType.Regular)
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
                else //Rest of rows just check two tiles
                {
                    if (tiles[currentRow, c].GetTileType() != TileData.TileType.Regular
                       || tiles[currentRow, c + length - 1].GetTileType() != TileData.TileType.Regular)
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

                currentRow += 1;
            }
            if (isLegal)
            {
                //Spawn a text indicating scores at the center of the cleared square
                Vector3 textPos = new Vector3();
                if (length % 2 == 1)
                {
                    textPos = GetTilePosition(r + (length - 1) / 2, c + (length - 1) / 2);
                }
                else
                {
                    Vector3 rightPos = GetTilePosition(r + (length - 1) / 2 + 1, c + (length - 1) / 2 + 1);
                    Vector3 leftPos = GetTilePosition(r + (length - 1) / 2, c + (length - 1) / 2);
                    textPos = new Vector3((leftPos.x + rightPos.x) / 2, (leftPos.y + rightPos.y) / 2, (leftPos.z + rightPos.z) / 2);
                }

                //If a legal square is formed, tell the event handler
                //Also clear all tiles inside the square (Just mark them here)
                processed.AddRange(MarkInsideTiles(r, c, length));
                OnSquareFormed(length, textPos);
            }
                
        }
        return processed;
    }

    private List<Tile> MarkInsideTiles(int row, int col, int length)
    {
        List<Tile> inside = new List<Tile>();
        //Looping inside the square
        for (int r = row + 1; r < row + length - 1; r++)
        {
            for (int c = col + 1; c < col + length - 1; c++)
            {
                if (tiles[r, c].GetIsOccupied() && inside.Find(t => t == tiles[r, c]) == null)
                    inside.Add(tiles[r, c]);
            }
        }
        return inside;
    }

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
                GameObject current = GameObject.Instantiate(prefabSpace, transform, false);
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

        // After four rotations, the block turns back to the beginning state.
        for (int rotate = 0; rotate < 4; rotate++, testBlock.Rotate(true))
        {
            //localSpaces = GetSpaces(testBlock.GetWidth(), testBlock.GetHeight());
            localSpaces = GetSpaces(1, 1);
            for (int i = 0; i < localSpaces.Count; i++)
            {
                if (localSpaces[i].CanBlockFit(testBlock))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // To be called by the Space class whenever a new DraggableBlock is successfully placed on the Grid.
    public void PlacedDraggableBlock()
    {    
        //If there is not square formed this turn, then energy will be reduced by 1 plus number of vestiges
        if (!CheckForMatches())
        {
            int vestigeNum = 0;

            //Count the vestiges number on the grid
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

            energyCounter.RemoveEnergy(1 + vestigeNum);
        }

        blockSpawner.ProgressQueue();
        //Update Available spaces for all draggable blocks
        blockSpawner.UpdateAllBlocks();
    }

    // Removes a GridBlock from the List of GridBlocks.
    public void RemoveGridBlock(GridBlock gb)
    {
        gridBlocks.Remove(gb);
    }

    //Called when a tiletype is changed
    private void Tile_Changed(TileData.TileType newType)
    {
        //If a type is changed to Unoccupied, then add 1 energy
        if (newType == TileData.TileType.Unoccupied)
        {
            energyCounter.AddEnergy(1);
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
