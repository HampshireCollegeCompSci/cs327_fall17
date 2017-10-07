//Author: Wm. Josiah Erikson
//This class is just to keep track of cleared squares for Analytics
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearedSquaresCounter : MonoBehaviour {
    [SerializeField]
    [Tooltip("Cleared Squares counter. For debug viewing purposes only.")]
    int clearedSquares;

    private void Start()
    {
        clearedSquares = 0;
    }

    public void ClearedSquare() //Called when a square is cleared
    {
        clearedSquares++;
    }

    public int GetClearedSquares () //Call to get number of cleared squares so far in the game
    {
        return clearedSquares;
    }
	
}
