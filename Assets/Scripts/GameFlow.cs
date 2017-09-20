// Author(s): Paul Calande, [your name here]

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public delegate void GameLostHandler();
    public event GameLostHandler GameLost;

    // This function triggers the game over condition by invoking the GameLost event.
    public void GameOver()
    {
        OnGameLost();
    }

    // Invoke the GameLost event.
    private void OnGameLost()
    {
        if (GameLost != null)
        {
            GameLost();
        }
    }
}