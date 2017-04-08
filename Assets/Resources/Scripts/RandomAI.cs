using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomAI : AI {

    public RandomAI(int turn, Color color, Sprite sprite) 
        : base(turn, color, sprite) {}

    /// <summary>
    /// Return a random open spot
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public override BoardSpot BestMove(Game game)
    {
        List<BoardSpot> openSpots = new List<BoardSpot>();
        foreach (BoardSpot spot in game.ActiveBoard.BoardSpots)
        {
            if(!spot.Clicked) { openSpots.Add(spot); }
        }
        return openSpots[Random.Range(0, openSpots.Count)];
    }
}
