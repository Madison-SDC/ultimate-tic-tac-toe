using System.Collections.Generic;

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

    public void ChooseChild()
    {
        if(children == null)
        {
            GenerateChildren();
        }
    }

    void GenerateChildren()
    {
        children = new List<MCTSNode>();
        foreach(Spot spot in game.AvailableSpots)
        {
            game.Play(spot); // make the move
            children.Add(new MCTSNode(game, spot, this));
            game.Undo(); // undo that move
        }
    }

    /// <summary>
    /// Returns a deep copy of the given global game
    /// </summary>
    /// <param name="globalGame"></param>
    /// <returns></returns>
    GlobalGame CopyGlobalGame(GlobalGame globalGame)
    {
        LocalGame[,] localGames = new LocalGame[3, 3];
        foreach(LocalGame game in globalGame.LocalGames)
        {
            localGames[game.Loc.Row, game.Loc.Col] = CopyLocalGame(game);
        }

        return new GlobalGame(
            localGames, 
            globalGame.Enabled, 
            globalGame.P1, 
            globalGame.P2, 
            globalGame.P1Turn
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
