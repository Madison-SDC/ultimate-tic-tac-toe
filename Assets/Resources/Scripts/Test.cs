using UnityEngine;
using System.Collections.Generic;

public class Test : UnityEngine.UI.Button {

    static Game TestGame() { return Game.CurrentGame; }

	public void AllTests()
    {
        Game game = TestGame();

        PlaySimpleGame();
        
    }

    /// <summary>
    /// Play a simple game where X wins the middle line of the top three boards
    /// and O plays in the top line of the middle three boards
    /// </summary>
    static void PlaySimpleGame()
    {
        List<int[]> args = new List<int[]>();
        // populate moves
        {
            args.Add(new int[4] { 1, 0, 0, 0 });
            args.Add(new int[2] { 0, 0 });

            args.Add(new int[2] { 1, 1 });
            args.Add(new int[2] { 0, 0 });

            args.Add(new int[2] { 1, 2 });
            args.Add(new int[2] { 0, 1 });

            args.Add(new int[2] { 1, 0 });
            args.Add(new int[2] { 0, 1 });

            args.Add(new int[2] { 1, 1 });
            args.Add(new int[2] { 0, 1 });

            args.Add(new int[2] { 1, 2 });
            args.Add(new int[2] { 0, 2 });

            args.Add(new int[2] { 1, 0 });
            args.Add(new int[2] { 0, 2 });

            args.Add(new int[2] { 1, 1 });
            args.Add(new int[2] { 0, 2 });

            args.Add(new int[2] { 1, 2 }); // x wins
        }
        TestGame().Reset();

        int i = 0;
        int spotRow = args[i][0];
        int spotCol = args[i][1];
        int boardRow = args[i][2];
        int boardCol = args[i][3];

        bool val = Play(spotRow, spotCol, boardRow, boardCol);

        while (val) // last move worked
        {
            // play next move
            i++;
            
            // all moves played
            if (args.Count <= i) { break; }

            spotRow = args[i][0];
            spotCol = args[i][1];
            val = Play(spotRow, spotCol);
        }
        if(val) { Debug.Log("Simple game test passed"); }
    }

    /// <summary>
    /// Assuming the specified spot is empty, plays at that spot
    /// Returns false if game is over, local board is not active,
    /// or spot is filled, and does not play.
    /// Returns true otherwise
    /// </summary>
    /// <param name="spotRow"></param>
    /// <param name="spotCol"></param>
    /// <param name="boardRow"></param>
    /// <param name="boardCol"></param>
    static bool Play(
        int spotRow, 
        int spotCol, 
        int boardRow = -1, 
        int boardCol = -1
        )
    {
        Game game = TestGame();

        // can't play on a finished game
        if (!GameOverMatch(game, false)) { return false; };

        Board localBoard;
        if(boardRow == -1 || boardCol == -1) { localBoard = game.ActiveBoard; }
        else { localBoard = (Board)game.Get(new Location(boardRow, boardCol)); }

        // local board must be active to play
        if(!IsActive(localBoard)) { return false; }

        // The local spot to be played
        Spot spot = localBoard.Get(new Location(spotRow, spotCol));

        // spot must be empty to play
        if (IsEmpty(spot)) { game.UpdateDisplay(spot); }
        else { return false; }

        game.Confirm();
        return true;
    }

    /// <summary>
    /// Returns whether the <paramref name="spot"/> is empty
    /// Outputs a message if <paramref name="spot"/> is not empty
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    static bool IsEmpty(Spot spot)
    {
        if(spot.Owner != null)
        {
            Debug.Log("Spot not empty!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Returns whether the board is active.
    /// Outputs a message if board not active.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    static bool IsActive(Board board)
    {
        if(!board.Active)
        {
            Debug.Log("Board not active!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Returns whether the game over value matches parameter <paramref name="value"/>.
    /// Outputs a message if they don't match.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    static bool GameOverMatch(Game game, bool value)
    {
        if(game.GameOver != value)
        {
            Debug.Log("Game over value does not match parameter value");
            return false;
        }
        return true;
    }
}
