public class Move
{
    LocalGame game;
    Spot spot;

    public LocalGame Game { get { return game; } }
    public Spot Spot { get { return spot; } }

    public Move(LocalGame game, Spot spot)
    {
        this.game = game;
        this.spot = spot;
    }
}
