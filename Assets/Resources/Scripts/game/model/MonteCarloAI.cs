﻿using System;
using System.Diagnostics;
using UnityEngine;

public class MonteCarloAI : AI
{
    MCTSNode root;

    /// <summary>
    /// The amount of time this AI has to think, in seconds
    /// </summary>
    float time;
    
    public float Time { get { return time; } }

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

    public MonteCarloAI(GlobalGame game, int turn, Color color, Sprite sprite, string name, float time) : base(game, turn, color, sprite, name)
    {
        root = new MCTSNode(game, null, null);
        this.time = Math.Max(float.Epsilon, time);
    }

    /// <summary>
    /// Update the root node
    /// </summary>
    /// <param name="spot"></param>
    public override void UpdateLastMove(Spot spot)
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
        Stopwatch stopwatch = new Stopwatch();
        float seconds = time;
        float milliseconds = seconds * 1000;
        stopwatch.Start();
        while (stopwatch.ElapsedMilliseconds < milliseconds)
        {
            root.ChooseChild();
        }
        return root.BestMove();
    }
}
