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
    public override Spot BestMove(Game game)
    {
        List<Spot> openSpots = new List<Spot>();

        foreach(Board board in game.Boards)
        {
            if(board.Active)
            {
                foreach(Spot spot in board.BoardSpots)
                {
                    if (spot != board // change now that board itself is a spot
                        && !spot.Clicked)
                    { openSpots.Add(spot); }
                }
            }
        }

        return openSpots[Random.Range(0, openSpots.Count)];
    }
}
