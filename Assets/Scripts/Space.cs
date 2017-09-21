// Author(s): Paul Calande, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
	private int x; //the x-coordinate of this space
	private int y; //the y-coordinate of this space
	private int width; //width of this space
	private int height; //height of this space
	private Grid grid; //which grid this space is on

	public void Init (int mx, int my, int mwidth, int mheight, Grid mgrid)
	{
        x = mx;
        y = my;
        width = mwidth;
        height = mheight;
        grid = mgrid;
	}

    public bool CanBlockFit(Block block)
    {
        if (block.GetWidth() == width && block.GetHeight() == height)
        {
            // It fits, dimensionally speaking.
            return grid.CanBlockFit(x, y, block); // Check if the important Cells are empty.
        }
        else
        {
            // It doesn't.
            return false;
        }
    }

	//This method should be called when a DraggableBlock is dragged onto this Space.
	public void PlaceBlock(DraggableBlock block)
	{
        grid.WriteBlock(x, y, block.GetBlock()); //We're placing this block. Apparently it's the external responsibility to make sure this will work
        Destroy(block.gameObject); //And now we're done with this GameObject - it's on the grid
    }
}