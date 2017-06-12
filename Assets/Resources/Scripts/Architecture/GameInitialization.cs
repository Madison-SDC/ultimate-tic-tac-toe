using UnityEngine;

/// <summary>
/// This script goes on the same GameObject as the GlobalGameUI
/// Conditions for GameObject:
/// Has 9 children that have GameUIs
/// Each child has 9 children that are SpotUIs
/// Child 0 is top left
/// Children and grandchildren are ordered left to right, top to bottom
/// </summary>
class GameInitialization : MonoBehaviour
{
    /// <summary>
    /// Create all scripts and set up their references to the UI
    /// </summary>
    private void Start()
    {
        int spotUIIndex = 0;
        int localGameUIIndex = 1; // first component is global
        SpotUI[] spotUIs = GetComponentsInChildren<SpotUI>();
        GameUI[] gameUIs = GetComponentsInChildren<GameUI>();
        LocalGame[,] localGames = new LocalGame[3, 3];

        for (int boardRow = 0; boardRow < 3; boardRow++)
        {
            for(int boardCol = 0; boardCol < 3; boardCol++)
            {
                // a grid of spots represents each local board
                Spot[,] spots = new Spot[3, 3];

                for(int spotRow = 0; spotRow < 3; spotRow++)
                {
                    for(int spotCol = 0; spotCol < 3; spotCol++)
                    {
                        // each spot
                        SpotUI spotUI = spotUIs[spotUIIndex];
                        Spot spot = new Spot(
                            new Location(spotRow, spotCol), null, true);

                        spotUI.Spot = spot;

                        spots[spotRow, spotCol] = spot;
                        spotUIIndex++;
                    }
                }

                // each local game
                Location loc = new Location(boardRow, boardCol);                
                LocalGame localGame = new LocalGame(spots, true, loc);
                localGames[boardRow, boardCol] = localGame;
                gameUIs[localGameUIIndex].Game = localGame;
                localGameUIIndex++;
            }
        }

        // initialize the global game and its view and controller
        GlobalGame game = new GlobalGame(
            localGames,    // the board for the global game
            true,          // enabled
            Settings.p1,   // player 1
            Settings.p2,   // player 2
            true           // player 1 turn
        );
        
        GetComponent<GameUI>().Game = game;
        GetComponent<GameController>().Game = game;

        Destroy(this);
    }
}
