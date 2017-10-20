// Author(s): Paul Calande, Wm. Josiah Erikson, Yixiang Xu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public enum GameOverCause
    {
        NoRemainingSpaces,
        QueueOverflow,
        NoMoreEnergy,
        Reset
    }

    public delegate void GameLostHandler(GameOverCause cause);
    public event GameLostHandler GameLost;

    // This function triggers the game over condition by invoking the GameLost event.
    public void GameOver(GameOverCause cause)
    {
        //Debug.Log("Game over! Cause: " + cause);
        OnGameLost(cause);
        Time.timeScale = 0f;
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