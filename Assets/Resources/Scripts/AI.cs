using System;
using UnityEngine;

public abstract class AI : Player
{
    // Chain to the base constructor
    public AI(int turn, Color color, Sprite sprite) : 
        base(turn, color, sprite) {}

    /// <summary>
    /// Return the best move for the AI in <paramref name="game"/>.
    /// Assumes it is this's turn.
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public abstract BoardSpot BestMove(Game game);

}
