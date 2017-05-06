using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {
    static int gameMode = INSTRUCTION;

    const int INSTRUCTION = -1;
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
            if(value == ONE_PLAYER || value == TWO_PLAYER || value == INSTRUCTION)
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
        else if (GameMode == INSTRUCTION)
        {
            gameObject.AddComponent<InstructionGame>();
        }
        Destroy(this);
    }
}
