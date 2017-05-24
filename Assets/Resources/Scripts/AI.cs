using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class AI : Player
{
    // Chain to the base constructor
    public AI(int turn, Color color, Sprite sprite, string name) : 
        base(turn, color, sprite, name) {}

    /// <summary>
    /// Return the best move for the AI in <paramref name="game"/>.
    /// Assumes it is this's turn.
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public abstract SpotUI BestMove(Game game);

    /// <summary>
    /// All the open spots in the game
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    internal List<SpotUI> OpenSpots(Game game)
    {
        List<SpotUI> openSpots = new List<SpotUI>();

        foreach (Board board in game.Boards)
        {
            if (board.Active)
            {
                foreach (SpotUI spot in board.Spots)
                {
                    if (spot.Owner == null)
                    { openSpots.Add(spot); }
                }
            }
        }

        return openSpots;
    }
}
