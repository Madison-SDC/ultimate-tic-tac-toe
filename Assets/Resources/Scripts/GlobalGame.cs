using System;
using System.Collections.Generic;

public class GlobalGame : Game
{
    // gameMode constants for play method
    const int REGULAR = 0,
        UNDO = 1,
        REDO = 2;

    LocalGame[,] localGames;
    Player p1, p2;
    bool p1Turn;
    Spot nextMove;
    Stack<Move> history;
    LocalGame activeGame;
    Stack<Move> future;
    bool canConfirm, canUndo, canRedo;
    List<Spot> availableSpots;
    bool hasNextMove;
    
    private bool CanConfirm
    {
        get { return canConfirm; }
        set
        {
            if(canConfirm != value)
            {
                canConfirm = value;
                RaiseCanConfirmChanged(new BoolEventArgs(
                    canConfirm && !(ActivePlayer() is AI)));
            }
        }
    }

    private bool CanUndo
    {
        get { return canUndo; }
        set
        {
            if(canUndo != value)
            {
                canUndo = value;
                RaiseCanUndoChanged(new BoolEventArgs(canUndo));
            }
        }
    }

    private bool CanRedo
    {
        get { return canRedo; }
        set
        {
            if(canRedo != value)
            {
                canRedo = value;
                RaiseCanRedoChanged(new BoolEventArgs(
                    canRedo && !(ActivePlayer() is AI)));
            }
        }
    }

    public List<Spot> AvailableSpots
    {
        get { return new List<Spot>(availableSpots); }
    }
    
    public bool HasNextMove { get { return hasNextMove; } }

    public Player P1 { get { return p1; } }
    public Player P2 { get { return p2; } }

    public GlobalGame(
        LocalGame[,] localGames,
        bool enabled, 
        Player p1, 
        Player p2, 
        bool p1Turn
    ) 
        : base(enabled)
    {
        PopulateOwnerArray(localGames);
        this.localGames = localGames;
        this.p1 = p1;
        this.p2 = p2;
        this.p1Turn = p1Turn;
        nextMove = null;
        history = new Stack<Move>();
        SetActiveGame(null);
        future = new Stack<Move>();
        canConfirm = false;
        canUndo = false;
        canRedo = false;
        availableSpots = new List<Spot>();
        hasNextMove = false;

        // listen for when any spot in the game has been clicked
        foreach(LocalGame game in localGames)
        {
            foreach(Spot spot in game.Spots)
            {
                spot.Clicked += HandleSpotClicked;
                spot.EnabledChanged += HandleSpotEnabledChanged;
                if (game.Enabled && spot.Enabled) { availableSpots.Add(spot); }
            }
        }

        CheckWinner();
    }

    public event EventHandler<BoolEventArgs> CanConfirmChanged;
    protected virtual void RaiseCanConfirmChanged(BoolEventArgs e)
    {
        if (CanConfirmChanged != null) { CanConfirmChanged(this, e); }
    }

    public event EventHandler<BoolEventArgs> CanUndoChanged;
    protected virtual void RaiseCanUndoChanged(BoolEventArgs e)
    {
        if (CanUndoChanged != null) { CanUndoChanged(this, e); }
    }

    public event EventHandler<BoolEventArgs> CanRedoChanged;
    protected virtual void RaiseCanRedoChanged(BoolEventArgs e)
    {
        if(CanRedoChanged != null) { CanRedoChanged(this, e); }
    }
    
    void PopulateOwnerArray(LocalGame[,] localGames)
    {
        ownerArray = new Player[localGames.GetLength(0), localGames.GetLength(1)];
        foreach (LocalGame localGame in localGames)
        {
            localGame.WinnerChanged += HandleLocalWinnerChanged;
            HandleLocalWinnerChanged(localGame, null); // populate array
        }
    }

    void HandleLocalWinnerChanged(object o, GameEventArgs e)
    {
        LocalGame localGame = (LocalGame)o;
        UpdateOwnerArray(localGame.Loc, localGame.Winner);
    }

    void HandleSpotClicked(object o, SpotEventArgs e)
    {
        if (!(ActivePlayer() is AI))
        {
            Preview((Spot)o);
        }
    }

    void HandleSpotEnabledChanged(object o, SpotEventArgs e)
    {
        Spot spot = (Spot)o;
        if(spot.Enabled) { availableSpots.Add(spot); }
        else { availableSpots.Remove(spot); }
    }

    public void Preview(Spot spot)
    {
        // remove the mark from the last spot
        if (nextMove != null)
        {
            nextMove.Owner = null;
        }

        if (spot != null)
        {
            spot.Owner = ActivePlayer(); // add a mark to this spot
        }

        nextMove = spot;
        hasNextMove = nextMove != null;
        CanConfirm = hasNextMove;
        CanUndo = hasNextMove || history.Count != 0;
        CanRedo = !hasNextMove && future.Count != 0;
    }

    public void Confirm()
    {
        if(!CanConfirm) { return; }
        Spot spot = nextMove;
        Preview(null); // undo preview move
        Play(spot);
    }

    public void Undo()
    {
        if(!CanUndo) { return; }
        if(nextMove != null)
        {
            Preview(null);
            return;
        }
        Move lastMove = history.Pop();
        Play(lastMove.Spot, UNDO, lastMove.Game);
        future.Push(lastMove);
        CanRedo = true;
        CanUndo = history.Count != 0;
    }

    public void Redo()
    {
        if(!CanRedo) { return; }
        Play(future.Pop().Spot, REDO);
        CanRedo = future.Count != 0;
    }

    public void Reset()
    {
        while(CanUndo)
        {
            Undo();
        }
    }

    void Play(Spot spot, int moveType = REGULAR, LocalGame prevActiveGame = null)
    {
        bool undo = moveType == UNDO;
        bool redo = moveType == REDO;
        spot.Owner = undo ? null : ActivePlayer();
        spot.Enabled = spot.Owner == null;
        if(!undo)
        {
            history.Push(new Move(activeGame, spot));
            CanUndo = true;
            if(!redo)
            {
                // regular move resets the future, cannot redo old moves
                future = new Stack<Move>();
                CanRedo = false;
            }
        }
        SetActiveGame(undo ? prevActiveGame : GetGame(spot));
        p1Turn = !p1Turn;
    }
    
    public override bool GameOver()
    {
        bool gameOver = base.GameOver() && nextMove == null;
        Enabled = !gameOver;
        return gameOver;
    }

    /// <summary>
    /// A global game is full if all of its local games are over
    /// </summary>
    /// <returns></returns>
    public override bool IsFull()
    {
        foreach(LocalGame game in localGames)
        {
            if(!game.GameOver())
            {
                return false;
            }
        }
        return true;
    }

    public Player ActivePlayer()
    {
        return p1Turn ? p1 : p2;
    }

    void SetActiveGame(LocalGame localGame)
    {
        if(GameOver())
        {
            // disable all games
            foreach(LocalGame game in localGames)
            {
                SetEnabled(game, false);
            }
        }
        else if (localGame != null
            && !localGame.GameOver())
        {
            // enable only the active game
            foreach (LocalGame game in localGames)
            {
                SetEnabled(game, game == localGame);
            }
        }
        else
        {
            // enable all unfinished games
            foreach (LocalGame game in localGames)
            {
                SetEnabled(game, !game.GameOver());
            }
        }

        activeGame = localGame;
    }

    void SetEnabled(LocalGame localGame, bool value)
    {
        localGame.Enabled = value;
        foreach (Spot spot in localGame.Spots)
        {
            if (spot.Enabled != value // don't bother doing nothing
                && (!value // can always disable a spot
                || spot.Owner == null)) // can only enable empty spots
            {
                spot.Enabled = value;
            }
        }
    }

    /// <summary>
    /// The Local Game that listens to this spot
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    LocalGame GetGame(Spot spot)
    {
        Location loc = spot.Loc;
        return localGames[loc.Row, loc.Col];
    }
}