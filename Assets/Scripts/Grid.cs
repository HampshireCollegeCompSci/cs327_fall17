// Author(s): Paul Calande, Yifeng Shi
// A 2-dimensional collections of tiles

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    int width;
    int height;

    Tile[,] tiles;
    Space[,] spaces;

    GameObject prefabTile;
    GameObject prefabSpace;

    BlockSpawner blockSpawner;

    List<GridBlock> gridBlocks;

    //Four lists storing lists of four direction of L-shapes respectively.
    //List<LShape> topLeft;
    //List<LShape> topRight;
    //List<LShape> bottomLeft;
    //List<LShape> bottomRight;

    private void Start()
    {
        //Instantiate tiles array
        tiles = new Tile[width, height];
        for (int c = 0; c < width; c++){
            for (int r = 0; r < height; r++){
                //Need to be changed after knowing specific positions
                GameObject currentPrefabTile = Instantiate(prefabTile);
                tiles[r, c] = currentPrefabTile.GetComponent<Tile>();
            }
        }

        //Instantiate spaces array
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

        //Instantiate BlockSpawner
        blockSpawner.Init(spaces);

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

    public Tile[,] GetTiles()
    {
        return tiles;
    }

    public bool CanBlockFit(int x, int y, Block block)
    {
        //Assume each tile is 1x1 size.
        for (int c = 0; c < block.GetWidth(); c++){
            for (int r = 0; r < block.GetHeight(); r++){
                if (tiles[y + r, x + c].GetIsOccupied() &&
                    block.GetTiles()[r, c].GetIsOccupied())
                    return false;
            }
        }

        return true;
    }

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

    public void WriteBlock(int x, int y, Block block)
    {
        //List<Coordinate> coords = new List<Coordinate>();
		for (int c = 0; c < block.GetWidth(); c++)
		{
			for (int r = 0; r < block.GetHeight(); r++)
			{
                if (block.GetTiles()[r, c].GetIsOccupied()){
                    tiles[y + r, x + c].Fill();
                    //Note x is col and y is row
                    //coords.Add(new Coordinate(x + c, y + r));
                }
			}
		}
        gridBlocks.Add(new GridBlock(x, y, block));

        //call LShapeCheck after each insertion
        //LShapeCheck(coords);
    }

    public void CheckForMatches(){

        //List keeping all tiles that are going to be removed.
        //We do remove after calculation because it's possible to remove
        //multiple squares together
        List<Tile> toRemove = new List<Tile>();

        //Loop through all blocks from topLeft.
        //Consider only the case that the current tile is the top-lefr corner
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                //Only if the tile is regular black tile
                if(tiles[r, c].GetTileType() == Tile.TileType.Regular)
                {
                    //Check for squares from length 3 to 8
                    for (int length = 3; length <= 8; length++)
                    {
                        toRemove.AddRange(CheckForSquares(r, c, length));
                    }
                }
            }
        }

        //Remove all tiles that form squares
        toRemove.ForEach(t => t.Clear());

        //For vestiges, leave it for now
    }

    private List<Tile> CheckForSquares(int r, int c, int length)
    {
        List<Tile> processed = new List<Tile>();
        //Do not check if the square is not possible
        //to be formed at this tile
        if (r + length - 1 < height && c + length - 1 < width)
        {
            int count = 0;
            int tR = r;
            bool isLegal = true;
            //The max number of tiles to be checked
            while (count < length * length - (length - 2) * (length - 2) - 1)
            {
                //For first and last rows check the whole line
                if (tR == r || tR == r + length - 1)
                {
                    for (int i = c; i < c + length; i++)
                    {
                        if (tiles[tR, i].GetTileType() != Tile.TileType.Regular)
                        {
                            isLegal = false;
                            processed.Clear();
                            break;  //exit for loop
                        }
                        //Check to avoid repeated tiles
                        if (processed.Find(t => t == tiles[tR, i]) == null)
                            processed.Add(tiles[tR, i]);
                        count++;
                    }
                    if (!isLegal)
                        break;  //exit while loop
                }
                else //Rest of rows just check two tiles
                {
                    if (tiles[tR, c].GetTileType() != Tile.TileType.Regular
                       && tiles[tR, c + length - 1].GetTileType() != Tile.TileType.Regular)
                    {
                        isLegal = false;
                        processed.Clear();
                        break;  //exit while loop
                    }
                    if (processed.Find(t => t == tiles[tR, c]) == null)
                        processed.Add(tiles[tR, c]);
                    if (processed.Find(t => t == tiles[tR, c + length - 1]) == null)
                        processed.Add(tiles[tR, c + length - 1]);
                    count += 2;
                }
            }
        }
        return processed;
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
                        if(tiles[r, c].GetIsOccupied())
                            pushRightStatus[r] = 1;
                        else
                            pushRightStatus[r] = 0;
                    }
					//Move c-1 th column to cth column.
					//After this step the c-1 th column is up to date
					//and is ready for the next loop
					for (int r = 0; r < height; r++)
					{
                        if(tiles[r, c-1].GetIsOccupied() && pushRightStatus[r] == 0)
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
        blockSpawner.SpawnRandomBlock();
    }
}