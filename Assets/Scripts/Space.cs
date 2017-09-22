// Author(s): Paul Calande, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField]
	private int x; //the x-coordinate of this space
    [SerializeField]
	private int y; //the y-coordinate of this space
    [SerializeField]
    private int width; //width of this space
    [SerializeField]
	private int height; //height of this space
    [SerializeField]
    private Grid grid; //which grid this space is on

    [SerializeField]
    SnapLocation snapLocation;

    private void Start()
    {
        //snapLocation = GetComponent<SnapLocation>();
        snapLocation.SnappedTo += SnapLocation_SnappedTo;
    }

    public void Init (int mx, int my, int mwidth, int mheight, Grid mgrid)
	{
        x = mx;
        y = my;
        width = mwidth;
        height = mheight;
        grid = mgrid;

        // Center the Space in the middle of all of the Tiles it occupies.
        Vector3 tilePosTopLeft = grid.GetTilePosition(x, y);
        Vector3 tilePosBottomRight = grid.GetTilePosition(x + width - 1, y + height - 1);
        Vector3 averagePos = (tilePosTopLeft + tilePosBottomRight) * 0.5f;
        transform.position = averagePos;
        // Scale the space as well.
        transform.localScale = new Vector3(height, width, 1.0f);
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
        grid.PlacedDraggableBlock(); // Notify the Grid that we just placed a DraggableBlock.
        Destroy(block.gameObject); //And now we're done with this GameObject - it's on the grid
    }

    private void SnapLocation_SnappedTo(GameObject snapper)
    {
        PlaceBlock(snapper.GetComponent<DraggableBlock>());
    }
}