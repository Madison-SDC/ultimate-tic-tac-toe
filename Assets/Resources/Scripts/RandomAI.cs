using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomAI : AI {

    public RandomAI(int turn, Color color, Sprite sprite) 
        : base(turn, color, sprite, "AI") {}

    /// <summary>
    /// Return a random open spot
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public override Spot BestMove(Game game)
    {
        List<Spot> openSpots = OpenSpots(game); 
        return openSpots[Random.Range(0, openSpots.Count)];
    }
}
