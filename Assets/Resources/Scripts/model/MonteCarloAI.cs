using System;
using UnityEngine;

public class MonteCarloAI : AI
{
    MCTSNode root;

    public override GlobalGame Game
    {
        get
        {
            return base.Game;
        }

        set
        {
            base.Game = value;
            root = new MCTSNode(Game, null, null);
        }
    }

    public MonteCarloAI(GlobalGame game, int turn, Color color, Sprite sprite, string name) : base(game, turn, color, sprite, name)
    {
        root = new MCTSNode(game, null, null);
    }

    /// <summary>
    /// Update the root node
    /// </summary>
    /// <param name="spot"></param>
    public override void LastMove(Spot spot)
    {
        root = root.NextRoot(spot);
    }

    /// <summary>
    /// Run a thousand simulations, 
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
