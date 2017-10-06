//Author: Wm. Josiah Erikson
//This class is for keeping track of how many turns have been played - very simple
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCounter : MonoBehaviour {

    int turns;
    public int GetTurns () 
    {
        return turns;
    }

    public void PlayedTurn()
    {
        turns += 1;
    }
}
