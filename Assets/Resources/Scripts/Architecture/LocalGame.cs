public class LocalGame : Game
{
    private Location loc;

    public Location Loc { get { return loc; } }
    public LocalBoard LocalBoard { get { return (LocalBoard)Board; } }

	public LocalGame(LocalBoard board, bool enabled, Location loc) 
        : base(board, enabled)
    {
        winCombos = ticTacToeWinCombos;
        this.loc = loc;
    }
}
