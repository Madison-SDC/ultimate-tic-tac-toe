using System.Collections.Generic;
using UnityEngine;

public class RandomAI : AI
{
    public RandomAI(GlobalGame game, int turn, Color color, Sprite sprite, string name) 
        : base(game, turn, color, sprite, name)
    {
    }

    public override Spot BestMove()
    {
        List<Spot> spots = Game.AvailableSpots;
        return spots[Random.Range(0, spots.Count)];
    }
}
