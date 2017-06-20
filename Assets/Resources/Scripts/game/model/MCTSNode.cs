using System;
using System.Collections.Generic;
using UnityEngine;

public class MCTSNode
{
    GlobalGame game;
    Spot lastMove;
    MCTSNode parent;
    int hits;
    int misses;
    int totalTrials;
    List<MCTSNode> children;

    public MCTSNode(GlobalGame game, Spot lastMove, MCTSNode parent)
    {
        this.game = CopyGlobalGame(game);
        this.lastMove = lastMove;
        this.parent = parent;
        hits = 0;
        misses = 0;
        totalTrials = 0;
    }

    internal Spot BestMove()
    {
        int mostSimulations = 0;
        Spot bestMove = null;
        foreach(MCTSNode child in children)
        {
            if(child.totalTrials > mostSimulations)
            {
                mostSimulations = child.totalTrials;
                bestMove = child.lastMove;
            }
        }
        return bestMove;
    }

    /// <summary>
    /// Returns a new root node when the given spot has been played in
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    internal MCTSNode NextRoot(Spot spot)
    {
        if(children == null) { GenerateChildren(); }
        foreach(MCTSNode child in children)
        {
            if(child.lastMove.Equals(spot))
            {
                child.parent = null;
                return child;
            }
        }
        return this;
    }

    /// <summary>
    /// Recursively choose an unexplored descendant, 
    /// or find a child where the game is over,
    /// and run a simulation on it
    /// </summary>
    public void ChooseChild()
    {
        // Make sure this has generated its children
        if(children == null)
        {
            GenerateChildren();
        }

        // No children? Game over, run simulation
        if(children.Count == 0)
        {
            RunSimulation();
            return;
        }

        // Has children, run simulation on first unexplored child
        foreach(MCTSNode child in children)
        {
            if(child.totalTrials == 0)
            {
                child.RunSimulation();
                return;
            }
        }

        // No unexplored children, choose child of best child
        BestChild().ChooseChild();
    }

    MCTSNode BestChild()
    {
        double highestPotential = -1f;
        MCTSNode bestChild = null;
        foreach(MCTSNode child in children)
        {
            double childPotential = Potential(child);
            if(childPotential > highestPotential)
            {
                highestPotential = childPotential;
                bestChild = child;
            }
        }
        return bestChild;
    }

    double Potential(MCTSNode child)
    {
        // misses of child are hits for parent
        int w = child.misses;
        int n = child.totalTrials;

        // w/n + sqrt(2)*sqrt( ln(totalTrials) / n)
        return (double)w / n + 1.414f * Math.Sqrt(Math.Log(totalTrials) / n);
    }

    void RunSimulation()
    {
        BackPropagate(Simulate(game));
    }

    /// <summary>
    /// Play <paramref name="game"/> randomly to the end
    /// Return positive if the player to make the first move won
    /// Return negative if the player to make the first move lost
    /// Return 0 if the game ends in a tie
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    int Simulate(GlobalGame game)
    {
        Player activePlayer = game.ActivePlayer();

        GlobalGame copy = CopyGlobalGame(game);

        while(!copy.GameOver())
        {
            List<Spot> moves = copy.AvailableSpots;
            bool moveFound = false;

            // If there's a game-winning move, play it
            // This shortens simulations and makes them more realistic
            foreach (Spot spot in moves)
            {
                copy.Play(spot, false, true);
                if (copy.Winner != null)
                {
                    moveFound = true;
                    break;
                }
                copy.UndoLastMove();
            }

            // Otherwise play a random move
            if (!moveFound)
            {
                Spot randomSpot = moves[UnityEngine.Random.Range(0, moves.Count)];
                copy.Play(randomSpot, false, true);
            }
        }

        if(copy.Winner == activePlayer) { return 1; }
        if(copy.Winner == null) { return 0; }
        return -1;
    }

    /// <summary>
    /// Increment hits or misses according to the sign of the result
    /// Have the parent node do this as well
    /// </summary>
    /// <param name="result"></param>
    void BackPropagate(int result)
    {
        if(result > 0) { hits++; }
        if(result < 0) { misses++; }
        totalTrials++;

        if(parent != null)
        {
            // parent active player is opposite of this active player
            // so negate sign of result
            parent.BackPropagate(-result);
        }
    }

    void GenerateChildren()
    {
        children = new List<MCTSNode>();
        foreach(Spot spot in game.AvailableSpots)
        {
            game.Play(spot, false, true); // make the move
            children.Add(new MCTSNode(game, spot, this));
            game.UndoLastMove(); // undo that move
        }
    }

    /// <summary>
    /// Returns a deep copy of the given global game
    /// </summary>
    /// <param name="globalGame"></param>
    /// <returns></returns>
    GlobalGame CopyGlobalGame(GlobalGame globalGame)
    {
        if(globalGame == null) { return null; }

        LocalGame[,] localGames = new LocalGame[3, 3];
        LocalGame activeGame = null;
        foreach(LocalGame game in globalGame.LocalGames)
        {
            localGames[game.Loc.Row, game.Loc.Col] = CopyLocalGame(game);
            if(game == globalGame.ActiveGame)
            {
                activeGame = localGames[game.Loc.Row, game.Loc.Col];
            }
        }

        return new GlobalGame(
            localGames, 
            globalGame.Enabled, 
            globalGame.P1,
            globalGame.P2,
            globalGame.P1Turn,
            activeGame
        );
    }

    /// <summary>
    /// Return a deep copy of the given local game
    /// Listens to a new set of copied spots
    /// </summary>
    /// <param name="localGame"></param>
    /// <returns></returns>
    LocalGame CopyLocalGame(LocalGame localGame)
    {
        Spot[,] spots = new Spot[3, 3];
        foreach(Spot spot in localGame.Spots)
        {
            // deep copy the array of spots
            spots[spot.Loc.Row, spot.Loc.Col] = CopySpot(spot);
        }

        return new LocalGame(spots, localGame.Enabled, localGame.Loc);
    }

    /// <summary>
    /// Returns a deep copy of the given spot
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    Spot CopySpot(Spot spot)
    {
        return new Spot(spot.Loc, spot.Owner, spot.Enabled);
    }
}
