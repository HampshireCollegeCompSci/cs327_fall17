// Author: Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
	private int xcoord; //the x-coordinate of this space
	private int ycoord; //the y-coordinate if this space
	private int width; //width of this space
	private int height; //height of this space
	private Grid grid; //which grid this space is on

	public void Init (int mx, int my, int mwidth, int mheight, Grid mgrid)
	{
        xcoord = mx;
        ycoord = my;
        width = mwidth;
        height = mheight;
        grid = mgrid;

	}

	public bool CanBlockFit (Block block)
	{
        if (block.GetWidth() == width && block.GetHeight() == height)
        {
            return true; // It fits
        }
        else { return false; } //It doesn't}
	}

		//This method should be called when a DraggableBlock is dragged onto this Space.
		public void PlaceBlock (Block block)
	{
        grid.WriteBlock(block); //We're placing this block. Apparently it's the external responsibility to make sure this will work
        DestroyObject(block); //And now we're done with this GameObject - it's on the grid

	}
}