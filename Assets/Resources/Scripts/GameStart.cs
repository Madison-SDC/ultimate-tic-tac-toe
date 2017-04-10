using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {
    static int gameMode = ONE_PLAYER;

    const int ONE_PLAYER = 1;
    const int TWO_PLAYER = 2;

    /// <summary>
    /// The game mode of the current game
    /// </summary>
    public static int GameMode
    {
        get { return gameMode; }
        set
        {
            if(value == ONE_PLAYER || value == TWO_PLAYER)
            {
                gameMode = value;
            }
        }
    }

    /// <summary>
    /// Add the game to the global board and then destroy this (no longer necessary)
    /// </summary>
    private void Awake()
    {
        if(GameMode == ONE_PLAYER)
        {
            gameObject.AddComponent<SinglePlayerGame>();
        }
        else if (GameMode == TWO_PLAYER)
        {
            gameObject.AddComponent<Game>();
        }
        Destroy(this);
    }
}
