//Author: Wm. Josiah Erikson
//This class is for keeping track of how many turns have been played - very simple
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCounter : MonoBehaviour {

    [SerializeField]
    [Tooltip("Turn counter. For debug viewing purposes only.")]
    int turns;

    private void Start()
    {
        turns = 0;
    }
    public int GetTurns () 
    {
        return turns;
    }

    public void PlayedTurn()
    {
        turns += 1;
    }
}
