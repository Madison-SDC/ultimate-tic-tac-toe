using UnityEngine;
using System.Collections.Generic;
using System;

public class HeuristicAI : AI {

    /// <summary>
    /// The player this plays against
    /// </summary>
    Player opponent;

    /// <summary>
    /// Value of going in the corner of a local board
    /// </summary>
    int cornerWeight;

    /// <summary>
    /// Value of going in the side of a local board
    /// </summary>
    int sideWeight;

    /// <summary>
    /// Value of going in the center of a local board
    /// </summary>
    int centerWeight;

    /// <summary>
    /// Value of winning a local board
    /// </summary>
    int localWinWeight;

    /// <summary>
    /// Value of blocking other player on a local board
    /// </summary>
    int localBlockWeight;

    public HeuristicAI(
        int turn, 
        Color color, 
        Sprite sprite, 
        Player opponent, 
        int corner, 
        int side, 
        int center,
        int localWin,
        int localBlock
        ) 
        : base(turn, color, sprite)
    {
        this.opponent = opponent;
        cornerWeight = corner;
        sideWeight = side;
        centerWeight = center;
        localWinWeight = localWin;
        localBlockWeight = localBlock;
    }
    
    bool IsCorner(Location loc)
    {
        return loc.Col % 2 == 0
            && loc.Row % 2 == 0;
    }

    bool IsCenter(Location loc)
    {
        return loc.Col == 1
            && loc.Row == 1;
    }

    bool IsSide(Location loc)
    {
        return !IsCorner(loc) && !IsCenter(loc);
    }

    /// <summary>
    /// Returns whether moving at the specified spot wins the local board 
    /// for the given player
    /// </summary>
    /// <param name="spot">The spot to move</param>
    /// <param name="me">True if the AI is moving, false otherwise</param>
    /// <returns></returns>
    bool WinsLocal(Spot spot, bool me)
    {
        Player currentPlayer = me ? this : opponent;
        Player previousOwner = spot.Owner; 
        
        // cannot win a board that's already been won
        if (previousOwner == currentPlayer) { return false; }

        spot.ParentBoard.FillSpot(spot, currentPlayer); // fake a move
        bool wins = spot.ParentBoard.Owner == currentPlayer; // now won?
        spot.ParentBoard.FillSpot(spot, previousOwner); // remove the fake move
        return wins;
    }

    public override Spot BestMove(Game game)
    {
        List<Spot> openSpots = OpenSpots(game);
        int bestScore = 0;
        Spot bestMove = openSpots[0];
        foreach(Spot spot in openSpots)
        {
            int spotScore = ScoreOf(spot, game);
            if(spotScore > bestScore)
            {
                bestMove = spot;
                bestScore = spotScore;
            }
        }
        return bestMove;
    }

    /// <summary>
    /// How good it would be to go in this spot
    /// According to the given heuristic
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    int ScoreOf(Spot spot, Game game)
    {
        Location loc = spot.Loc;
        int score = 0;
        if(IsCenter(loc)) { score += centerWeight; }
        else if(IsCorner(loc)) { score += cornerWeight; }
        else if(IsSide(loc)) { score += sideWeight; }

        if (WinsLocal(spot, true)) { score += localWinWeight; } // can win
        if (WinsLocal(spot, false)) { score += localBlockWeight; } // can block
        return score; // just to satisfy code paths
    }
}
