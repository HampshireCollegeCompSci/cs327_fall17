// Author(s): Paul Calande, [your name here]

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    int width;

    int height;

    Tile[,] tiles;

    private void Start()
    {

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
        return true; // Replace this with an actual implementation!
    }

    public void WriteBlock(int x, int y, Block block)
    {

    }

    public void CheckForMatches()
    {

    }

    public void MoveAllBlocks(Enums.Direction direction)
    {

    }
}