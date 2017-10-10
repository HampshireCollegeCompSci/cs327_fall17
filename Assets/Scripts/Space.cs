// Author(s): Paul Calande, Wm. Josiah Erikson

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The column of the Grid that this Space occupies.")]
	private int col;
    [SerializeField]
    [Tooltip("The row of the Grid that this Space occupies.")]
    private int row;
    [SerializeField]
    [Tooltip("The width of this Space.")]
    private int width;
    [SerializeField]
    [Tooltip("The height of this Space.")]
	private int height;
    [SerializeField]
    [Tooltip("Reference to the Grid that this Space occupies.")]
    private Grid grid;
    [SerializeField]
    [Tooltip("Reference to the SnapLocation component to interface with.")]
    SnapLocation snapLocation;

    private void Start()
    {
        //snapLocation = GetComponent<SnapLocation>();
        snapLocation.SnappedTo += SnapLocation_SnappedTo;
        snapLocation.HoveringTo += SnapLocation_Highlight;
    }

    public void Init(int mrow, int mcol, int mheight, int mwidth, Grid mgrid)
	{
        col = mcol;
        row = mrow;
        width = mwidth;
        height = mheight;
        grid = mgrid;

        // Center the Space in the middle of all of the Tiles it occupies.
        Vector3 tilePosTopLeft = grid.GetTilePosition(row, col);
        Vector3 tilePosBottomRight = grid.GetTilePosition(row + height - 1, col + width - 1);
        Vector3 averagePos = (tilePosTopLeft + tilePosBottomRight) * 0.5f;
        transform.localPosition = averagePos;
        // Scale the space as well.
        transform.localScale = new Vector3(width, height, 1.0f);
	}

    public bool CanBlockFit(Block block)
    {
        /*if (block.GetWidth() == width && block.GetHeight() == height)
        {
            // It fits, dimensionally speaking.
            return grid.CanBlockFit(row, col, block); // Check if the important Cells are empty.
        }
        else
        {
            // It doesn't.
            return false;
        }*/

        return grid.CanBlockFit(row, col, block);
    }

	//This method should be called when a DraggableBlock is dragged onto this Space.
	public void PlaceBlock(DraggableBlock block)
	{
        grid.WriteBlock(row, col, block.GetBlock()); //We're placing this block. Apparently it's the external responsibility to make sure this will work
        grid.PlacedDraggableBlock(); // Notify the Grid that we just placed a DraggableBlock.
        Destroy(block.gameObject); //And now we're done with this GameObject - it's on the grid
    }

    private void SnapLocation_SnappedTo(GameObject snapper)
    {
        PlaceBlock(snapper.GetComponent<DraggableBlock>());
    }

    private void SnapLocation_Highlight(GameObject snapper, bool on)
    {
        if (grid.SetHighlight(row, col, snapper.GetComponent<DraggableBlock>().GetBlock(), on)){
            grid.CheckForMatches(row, col, snapper.GetComponent<DraggableBlock>().GetBlock(), false);
        }
    }
}