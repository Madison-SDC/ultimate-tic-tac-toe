using System.Collections.Generic;
using System;

/// <summary>
/// Contains information on how a board is considered won or if its game is over
/// </summary>
public abstract class Game
{
    Board board;
    Player winner;
    bool gameOver;
    bool enabled;
    protected List<List<Location>> winCombos;
    protected static List<List<Location>> ticTacToeWinCombos = TicTacToeWinCombos();
    
    public Board Board { get { return board; } }
    public Player Winner
    {
        get { return winner; }
        protected set
        {
            if (winner != value)
            {
                winner = value;
                RaiseWinnerChanged(GetArgs());
            }
        }
    }
    public bool GameOver { get { return gameOver; } }
    public bool Enabled
    {
        get { return enabled; }
        set
        {
            if (enabled != value)
            {
                enabled = value;
                RaiseEnabledChanged(GetArgs());
            }
        }
    }

    public event EventHandler<GameEventArgs> WinnerChanged;
    protected virtual void RaiseWinnerChanged(GameEventArgs e)
    {
        if(WinnerChanged != null) { WinnerChanged(this, e); }
    }

    public event EventHandler<GameEventArgs> EnabledChanged;
    protected virtual void RaiseEnabledChanged(GameEventArgs e)
    {
        if(EnabledChanged != null) { EnabledChanged(this, e); }
    }

    /// <summary>
    /// Returns the 8 winning combos for tic-tac-toe
    /// </summary>
    /// <returns></returns>
    static List<List<Location>> TicTacToeWinCombos()
    {
        List<List<Location>> combos = new List<List<Location>>();

        // 8 ways to win tic-tac-toe
        for (int i = 0; i < 8; i++) { combos.Add(new List<Location>()); }

        // horizontal and vertical lines
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                combos[i].Add(new Location(i, j)); // horizontal lines
                combos[i + 3].Add(new Location(j, i)); // vertical lines
            }
        }

        // diagonal lines
        for (int i = 0; i < 3; i++)
        {
            combos[6].Add(new Location(i, i)); // TL to BR
            combos[7].Add(new Location(2 - i, i)); // TR to BL
        }

        return combos;
    }

    /// <summary>
    /// listen to the board
    /// </summary>
    /// <param name="board"></param>
    public Game(Board board, bool enabled)
    {
        this.board = board;
        this.enabled = enabled;
        winCombos = ticTacToeWinCombos;
        board.OwnerArrayChanged += HandleBoardOwnerArrayChanged;
        UpdateState();
    }

    /// if necessary, check the winner
    void HandleBoardOwnerArrayChanged(object o, BoardEventArgs e)
    {
        UpdateState();
    }

    /// <summary>
    /// Checks the winner and whether the game is over
    /// </summary>
    void UpdateState()
    {
        CheckWinner();
        CheckGameOver();
    }

    GameEventArgs GetArgs() { return new GameEventArgs(); }
    
    /// <summary>
    /// By default, checking a winner runs through each combo
    /// If a player owns every location on the combo, that player wins the game
    /// If no player owns every location on any combo, 
    /// then there is no winner of this game
    /// </summary>
    protected virtual void CheckWinner()
    {
        // probably would look something like this:

        Player player = null;
        Player[,] ownership = board.OwnerArray;
        bool foundWinner = true;
    
        foreach (List<Location> combo in winCombos)
        {
            foundWinner = true; // assume we'll find a winner, get proven wrong
            Location firstLoc = combo[0];
            player = ownership[firstLoc.Row, firstLoc.Col];

            if (player != null) // first spot occupied
            {
                // check the rest of the spots
                for (int i = 1; i < combo.Count; i++)
                {
                    // if they don't match, we have no winner
                    if (ownership[combo[i].Row, combo[i].Col] != player)
                    {
                        foundWinner = false;
                        break;
                    }
                }
            }
            else // first spot empty
            {
                foundWinner = false; // no winner on this combo
            }
            if (foundWinner)
            {
                break; // we have a winner! no need to check any more combos
            }
        }

        if (foundWinner)
        {
            Winner = player; // this fires event as well
        }
    }

    /// <summary>
    /// A generic game is over when it has a winner or its board is full
    /// </summary>
    protected virtual void CheckGameOver()
    {
        gameOver = winner != null // somebody won means game over
            || board.IsFull; // no spots left means game over
    }
}
