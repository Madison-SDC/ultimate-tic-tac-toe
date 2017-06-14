using UnityEngine;

public abstract class AI : Player
{
    protected GlobalGame game;

    public AI(GlobalGame game, int turn, Color color, Sprite sprite, string name)
        : base(turn, color, sprite, name)
    {
        this.game = game;
    }

    abstract public Spot BestMove();
}
