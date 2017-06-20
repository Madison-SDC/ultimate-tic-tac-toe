using UnityEngine;

public abstract class AI : Player
{
    GlobalGame game;

    virtual public GlobalGame Game
    {
        get { return game; }
        set { game = value; }
    }

    public AI(GlobalGame game, int turn, Color color, Sprite sprite, string name)
        : base(turn, color, sprite, name)
    {
        this.game = game;
    }

    abstract public Spot BestMove();

    /// <summary>
    /// Update any information when the last move is made
    /// </summary>
    /// <param name="spot"></param>
    virtual public void LastMove(Spot spot)
    {
        // do nothing by default
    }
}
