using UnityEngine;

public abstract class AI : Player
{
    GlobalGame game;

    public GlobalGame Game
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
}
