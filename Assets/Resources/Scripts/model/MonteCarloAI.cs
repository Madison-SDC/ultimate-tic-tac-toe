using System;
using UnityEngine;

public class MonteCarloAI : AI
{
    MCTSNode root;

    public MonteCarloAI(GlobalGame game, int turn, Color color, Sprite sprite, string name) : base(game, turn, color, sprite, name)
    {
        root = new MCTSNode(game, null, null);
    }

    /// <summary>
    /// Run ten thousand simulations, 
    /// then return the move of the child with the most simulations
    /// </summary>
    /// <returns></returns>
    public override Spot BestMove()
    {
        for(int i = 0; i < 10000; i++)
        {
            root.ChooseChild();
        }

        return root.BestMove();
    }
}
