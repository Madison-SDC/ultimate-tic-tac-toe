using UnityEngine;
using System.Collections.Generic;

public class Test : UnityEngine.UI.Button {
    
	public void AllTests()
    {
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

        TestGame(args, "Simple game passed", "Simple game failed");
    }

    static bool TestGame(List<int[]> args, string success, string fail)
    {
        Game.CurrentGame.Reset();

        int i = -1;
        int[] firstMoveArgs = args[0];
        int spotRow = firstMoveArgs[0];
        int spotCol = firstMoveArgs[1];
        int boardRow = firstMoveArgs[2];
        int boardCol = firstMoveArgs[3];
        bool testPass = true;

        while (testPass) // last move worked
        {
            // play next move
            i++;

            // all moves played
            if (args.Count <= i) { break; }

            int[] nextMoveArgs = args[i];
            spotRow = args[i][0];
            spotCol = args[i][1];
            if(nextMoveArgs.Length > 2)
            {
                // play on specific board
                boardRow = nextMoveArgs[2];
                boardCol = nextMoveArgs[3];
            }
            else
            {
                // play on active board
                boardRow = -1;
                boardCol = -1;
            }
            
            testPass = Play(spotRow, spotCol, boardRow, boardCol);
        }

        Debug.Log(testPass ? success : fail);

        return testPass;
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
        Game game = Game.CurrentGame;

        // can't play on a finished game
        if (!GameOverMatch(game, false)) { return false; };

        Board localBoard;
        if(boardRow == -1 || boardCol == -1) { localBoard = game.ActiveBoard; }
        else { localBoard = (Board)game.Get(new Location(boardRow, boardCol)); }

        // local board must be active to play
        if(!IsActive(localBoard)) { return false; }

        // The local spot to be played
        SpotUI spot = localBoard.Get(new Location(spotRow, spotCol));

        // spot must be empty to play
        if (OwnerIs(spot, null)) { game.PreviewMove(spot); }
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
    static bool OwnerIs(SpotUI spot, Player owner)
    {
        if(spot.Owner != owner)
        {
            if (!(spot is Board))
            {
                Debug.Log("Spot (" + spot.Loc +
                    ") on board  (" + spot.ParentBoard.Loc +
                    ")not owned by " + owner.Name + "!");
            } else
            {
                Debug.Log("Board (" + spot.Loc +
                    ")not owned by " + owner.Name + "!");
            }
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
