using UnityEngine;
using System.Collections.Generic;
using System;

public class HeuristicAI : AI {

    /// <summary>
    /// The player this plays against
    /// </summary>
    Player opponent;

    /// <summary>
    /// How far ahead this looks [0 to 10]
    /// </summary>
    int depth;

    /// <summary>
    /// Value of going in the corner of a local board
    /// </summary>
    int cornerWeight;

    /// <summary>
    /// Value of going in the side of a local board
    /// </summary>
    int sideWeight;

    /// <summary>
    /// Value of going in the center of a local board
    /// </summary>
    int centerWeight;

    /// <summary>
    /// Value of winning a local board
    /// </summary>
    int localWinWeight;

    /// <summary>
    /// Value of blocking other player on a local board
    /// </summary>
    int localBlockWeight;

    /// <summary>
    /// Value of sending opponent to a game over board, 
    /// allowing opponent to play anywhere
    /// </summary>
    int relativeOverWeight;

    public HeuristicAI(
        int turn, 
        Color color, 
        Sprite sprite, 
        Player opponent, 
        int depth,
        int corner, 
        int side, 
        int center,
        int localWin,
        int localBlock,
        int relativeOver
        ) 
        : base(turn, color, sprite)
    {
        this.opponent = opponent;
        this.depth = depth;
        cornerWeight = corner;
        sideWeight = side;
        centerWeight = center;
        localWinWeight = localWin;
        localBlockWeight = localBlock;
        relativeOverWeight = relativeOver;
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

    /// <summary>
    /// Returns whether moving at the specified spot wins the local board 
    /// for the given player
    /// </summary>
    /// <param name="spot">The spot to move</param>
    /// <param name="me">True if the AI is moving, false otherwise</param>
    /// <returns></returns>
    bool WinsLocal(Spot spot, bool me)
    {
        Player currentPlayer = me ? this : opponent;
        Player previousOwner = spot.Owner; 
        
        // cannot win a board that's already been won
        if (previousOwner == currentPlayer) { return false; }

        spot.ParentBoard.FillSpot(spot, currentPlayer); // fake a move
        bool wins = spot.ParentBoard.Owner == currentPlayer; // now won?
        spot.ParentBoard.FillSpot(spot, previousOwner); // remove the fake move
        return wins;
    }

    public override Spot BestMove(Game game)
    {
        return BestMove(game, depth, this);
    }

    /// <summary>
    /// Finds the best move for <paramref name="p"/> searching
    /// <paramref name="d"/> spots ahead
    /// </summary>
    /// <param name="d"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    Spot BestMove(Game g, int d, Player p)
    {
        List<Spot> openSpots = OpenSpots(g);
        int bestScore = Int32.MinValue;
        Spot bestMove = null;
        foreach (Spot spot in openSpots)
        {
            int spotScore = ScoreOf(spot, d, p);
            if (spotScore > bestScore)
            {
                bestMove = spot;
                bestScore = spotScore;
            }
        }
        return bestMove;
    }

    /// <summary>
    /// Returns whether the specified player going in this spot
    /// wins the entire game of the spot
    /// </summary>
    /// <param name="spot">The spot to play</param>
    /// <param name="me">Determines whether this or opponent plays
    /// True for this
    /// False for opponent</param>
    /// <returns></returns>
    bool WinsGlobal(Spot spot, bool me)
    {
        Game game = spot.Game;
        Player active = me ? this : opponent;
        // can't win a game that's already been won
        if(game.GameOver) { return false; }

        Player previousOwner = spot.Owner;

        spot.ParentBoard.FillSpot(spot, active);
        bool winsGlobal = game.Owner == active;
        spot.ParentBoard.FillSpot(spot, previousOwner);

        return winsGlobal;
    }

    /// <summary>
    /// Returns the other player
    /// </summary>
    /// <param name="p">The given player</param>
    /// <returns>This if <paramref name="p"/> != this, else opponent</returns>
    Player OtherPlayer(Player p)
    {
        return p == this ? opponent : this;
    }

    /// <summary>
    /// The value of playing in this spot
    /// According to the given heuristic weights
    /// For player <paramref name="p"/>
    /// </summary>
    /// <param name="spot">The spot to evaluate, can be played</param>
    /// <param name="game">The game to evaluate</param>
    /// <param name="d">The depth of the search</param>
    /// <param name="p">The player to score</param>
    /// <returns></returns>
    int ScoreOf(Spot spot, int d, Player p)
    {
        if(spot == null) { return 0; }
        Location loc = spot.Loc;
        int score = 0;

        // always go for the win, duh. arbitrary large magic number for now 
        if (WinsGlobal(spot, p == this)) { return 1000000; }

        if (d != 0)
        {
            score = ScoreOf(spot, 0, p); // find score without looking ahead
            Player previousOwner = spot.Owner;
            Game game = spot.Game;
            Board activeBoard = game.ActiveBoard;
            game.Play(spot, Game.PREVIEW); // preview moves are not in history

            // subtract the score of the best move the opponent can make
            Player otherPlayer = OtherPlayer(p);
            d--;

            score -= ScoreOf(
                BestMove(spot.Game, d, otherPlayer), d, otherPlayer);

            // undo hypothetical move
            game.Play(spot, Game.UNDO, activeBoard);

            return score;
        }

        if(IsCenter(loc)) { score += centerWeight; }
        else if(IsCorner(loc)) { score += cornerWeight; }
        else if(IsSide(loc)) { score += sideWeight; }

        if (WinsLocal(spot, p == this)) { score += localWinWeight; } // can win
        if (WinsLocal(spot, p != this)) { score += localBlockWeight; } // can block
        if(spot.RelativeBoard.GameOver) { score += relativeOverWeight; }
        
        return score;
    }
}
