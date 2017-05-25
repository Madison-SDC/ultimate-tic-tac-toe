public class GlobalBoard : Board
{
    LocalGame[,] localGames;

    public LocalGame[,] LocalGames { get { return localGames; } }

    public GlobalBoard(LocalGame[,] localGames)
    {
        this.localGames = localGames;

        foreach(Game localGame in localGames)
        {
            localGame.WinnerChanged += HandleLocalGameWinnerChanged;
        }
    }

    void HandleLocalGameWinnerChanged(object o, GameEventArgs e)
    {
        LocalGame localGame = (LocalGame)o;
        UpdateOwnerArray(localGame.Loc, localGame.Winner);
    }

}
