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
                Spot[,] board = new Spot[3, 3];

                for(int spotRow = 0; spotRow < 3; spotRow++)
                {
                    for(int spotCol = 0; spotCol < 3; spotCol++)
                    {
                        // each spot
                        SpotUI spotUI = spotUIs[spotUIIndex];
                        Spot spot = new Spot(
                            new Location(spotRow, spotCol),
                            null,
                            true
                        );

                        spotUI.Spot = spot;

                        board[spotRow, spotCol] = spot;
                        spotUIIndex++;
                    }
                }

                // each board
                Location loc = new Location(boardRow, boardCol);
                LocalBoard localBoard = new LocalBoard(loc, board);
                
                LocalGame localGame = new LocalGame(localBoard, true, loc);
                localGames[boardRow, boardCol] = localGame;
                gameUIs[localGameUIIndex].Game = localGame;
                localGameUIIndex++;
            }
        }

        // initialize the global board
        GlobalBoard globalBoard = new GlobalBoard(localGames);
        GetComponent<GameUI>().Game = new GlobalGame(
            globalBoard, 
            true, 
            Settings.p1, 
            Settings.p2, 
            true
        );
    }
}
