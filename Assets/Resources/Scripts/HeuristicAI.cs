using UnityEngine;
using System.Collections.Generic;
using System;

public class HeuristicAI : AI {

    int cornerWeight;
    int sideWeight;
    int centerWeight;

    public HeuristicAI(int turn, Color color, Sprite sprite, int corner, int side, int center) 
        : base(turn, color, sprite)
    {
        cornerWeight = corner;
        sideWeight = side;
        centerWeight = center;
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

    public override Spot BestMove(Game game)
    {
        List<Spot> openSpots = OpenSpots(game);
        int bestScore = 0;
        Spot bestMove = openSpots[0];
        foreach(Spot spot in openSpots)
        {
            int spotScore = ScoreOf(spot);
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
    int ScoreOf(Spot spot)
    {
        Location loc = spot.Loc;
        if(IsCenter(loc)) { return centerWeight; }
        if(IsCorner(loc)) { return cornerWeight; }
        if(IsSide(loc)) { return sideWeight; }
        return 0; // just to satisfy code paths
    }
}
