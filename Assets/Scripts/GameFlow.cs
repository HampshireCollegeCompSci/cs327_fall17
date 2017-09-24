// Author(s): Paul Calande

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public enum GameOverCause
    {
        NoRemainingSpaces,
        QueueOverflow
    }

    public delegate void GameLostHandler(GameOverCause cause);
    public event GameLostHandler GameLost;

    // This function triggers the game over condition by invoking the GameLost event.
    public void GameOver(GameOverCause cause)
    {
        //Debug.Log("Game over! Cause: " + cause);
        OnGameLost(cause);
    }

    // Invoke the GameLost event.
    private void OnGameLost(GameOverCause cause)
    {
        if (GameLost != null)
        {
            GameLost(cause);
        }
    }
}