using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGame : Game
{
    // gameMode constants for play method
    const int REGULAR = 0,
        UNDO = 1,
        REDO = 2;

    Stack<Move> history;
    Stack<Move> future;
    Spot nextMove;
    LocalGame[,] localGames;
    Player p1, p2;
    bool p1Turn;
    LocalGame activeGame;
    bool canConfirm, canUndo, canRedo;
    List<Spot> availableSpots;
    bool hasNextMove;

    public LocalGame[,] LocalGames { get { return localGames; } }

    public Player P1 { get { return p1; } }
    public Player P2 { get { return p2; } }
    public bool P1Turn { get { return p1Turn; } }

    public LocalGame ActiveGame { get { return activeGame; } }

    private bool CanConfirm
    {
        get { return canConfirm; }
        set
        {
            if (canConfirm != value)
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
            if (canUndo != value)
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
            canRedo = value;
            RaiseCanRedoChanged(new BoolEventArgs(
                canRedo && !(ActivePlayer() is AI)));
        }
    }

    public List<Spot> AvailableSpots
    {
        get { return new List<Spot>(availableSpots); }
    }

    public bool HasNextMove { get { return hasNextMove; } }

    public GlobalGame(
        LocalGame[,] localGames,
        bool enabled,
        Player p1,
        Player p2,
        bool p1Turn,
        LocalGame activeGame = null
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
        SetActiveGame(activeGame);
        future = new Stack<Move>();
        canConfirm = false;
        canUndo = false;
        canRedo = false;
        availableSpots = new List<Spot>();
        hasNextMove = false;

        // listen for when any spot in the game has been clicked
        foreach (LocalGame game in localGames)
        {
            foreach (Spot spot in game.Spots)
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
        if (CanRedoChanged != null) { CanRedoChanged(this, e); }
    }

    public event EventHandler<GameEventArgs> TurnChanged;
    protected virtual void RaiseTurnChanged(GameEventArgs e)
    {
        if(TurnChanged != null) { TurnChanged(this, e); }
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
        if (spot.Enabled) { availableSpots.Add(spot); }
        else { availableSpots.Remove(spot); }
    }

    public void Preview(Spot spot)
    {

        // remove the mark from the last spot
        if (nextMove != null)
        {
            nextMove.Owner = null;
            Outline(null, true);
        }

        nextMove = spot;

        if (spot != null)
        {
            spot = Get(spot.LocalGame.Loc).Get(spot.Loc);
            spot.Owner = ActivePlayer(); // add a mark to this spot
            Outline(GetGame(spot));
        }
        
        hasNextMove = nextMove != null;
        CanConfirm = hasNextMove;
        CanUndo = hasNextMove || history.Count != 0;
        CanRedo = !hasNextMove && future.Count != 0;
    }

    public void Confirm()
    {
        if (!CanConfirm) { return; }
        Spot spot = nextMove;
        Preview(null); // undo preview move
        Play(spot);
    }

    public void Undo(bool sim=false)
    {
        if (!CanUndo) { return; }

        if (ActivePlayer() is AI)
        {
            Preview(null);
            UndoLastMove(sim); // now it's the human's turn
        }
        else // it's a human's turn
        {
            if (nextMove != null)
            {
                Preview(null); // undo human's preview and be done
                return;
            }
            UndoLastMove(sim); // undo last move
            if (ActivePlayer() is AI)
            {
                UndoLastMove(sim); // undo human's last move
            }
            // now it's a human's turn
        }
    }

    public void UndoLastMove(bool sim)
    {
        Move lastMove = history.Pop();
        lastMove.Spot.Owner = null;
        lastMove.Spot.Enabled = true;
        SetActiveGame(lastMove.Game);
        p1Turn = !p1Turn;
        future.Push(lastMove);
        CanRedo = true;
        CanUndo = history.Count != 0;
        RaiseTurnChanged(null);
        if (!sim)
        {
            InformPlayers();
        }
    }

    public void Redo()
    {
        if (!CanRedo) { return; }
        Play(future.Pop().Spot, true);
        if (!GameOver() && ActivePlayer() is AI)
        {
            Play(future.Pop().Spot, true);
        }
        CanRedo = future.Count != 0;
        RaiseTurnChanged(null);
    }

    public void Reset()
    {
        while (CanUndo)
        {
            Undo();
        }
    }

    public void Play(Spot spot, bool redo = false, bool simulation = false)
    {
        Play(spot.LocalGame.Loc, spot.Loc, redo, simulation);
    }

    /// <summary>
    /// Advances the game by playing at the specified spot
    /// </summary>
    /// <param name="spot"></param>
    /// <param name="redo">Whether this move is a redo</param>
    void Play(Location localGameLoc, Location spotLoc, bool redo = false, bool simulation = false)
    {
        Spot spot = Get(localGameLoc).Get(spotLoc);

        spot.Owner = ActivePlayer();
        spot.Enabled = false;
        history.Push(new Move(activeGame, spot));
        CanUndo = true;
        future = redo ? future : new Stack<Move>();
        CanRedo = redo;
        SetActiveGame(GetGame(spot));
        p1Turn = !p1Turn;
        if (!simulation)
        {
            InformPlayers();
            RaiseTurnChanged(null);
        }
    }



    /// <summary>
    /// Inform the artificial intelligences that a move has been made
    /// </summary>
    void InformPlayers()
    {
        Spot lastMove = history.Count > 0 ? history.Peek().Spot : null;
        Inform(p1, lastMove);
        Inform(p2, lastMove);
    }

    void Inform(Player player, Spot spot)
    {
        player.UpdateLastMove(spot);
    }

    LocalGame Get(Location loc)
    {
        return localGames[loc.Row, loc.Col];
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
        foreach (LocalGame game in localGames)
        {
            if (!game.GameOver())
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

    public Player OtherPlayer()
    {
        return p1Turn ? p2 : p1;
    }

    /// <summary>
    /// All games that would be enabled if <paramref name="localGame"/> 
    /// was the active game
    /// </summary>
    /// <param name="localGame"></param>
    /// <returns></returns>
    List<LocalGame> ActiveGames(LocalGame localGame)
    {
        List<LocalGame> activeGames = new List<LocalGame>();
        
        if (GameOver())
        {
            // no active games if the global game is over
            return activeGames;
        }

        if (localGame != null && !localGame.GameOver())
        {
            // return only the active game
            activeGames.Add(localGame);
            return activeGames;
        }
        else
        {
            // return all unfinished games
            foreach (LocalGame game in localGames)
            {
                if (!game.GameOver())
                {
                    activeGames.Add(game);
                }
            }
            return activeGames;
        }
    }

    /// <summary>
    /// Toggles the outline of active games for the next turn, assuming
    /// the next active game will be <paramref name="nextActiveGame"/>
    /// </summary>
    /// <param name="nextActiveGame"></param>
    void Outline(LocalGame nextActiveGame, bool remove = false)
    {
        List<LocalGame> outlinedGames = base.GameOver()? 
            new List<LocalGame>() : ActiveGames(nextActiveGame);

        foreach (LocalGame game in localGames)
        {
            Color color = !remove && outlinedGames.Contains(game) ? 
                OtherPlayer().Color : Color.clear;
            SetOutlined(game, color);
        }
    }

    void SetOutlined(LocalGame game, Color color)
    {
        game.Outline = color;
    }

    void SetActiveGame(LocalGame localGame)
    {
        List<LocalGame> activeGames = ActiveGames(localGame);

        foreach (LocalGame game in localGames)
        {
            SetEnabled(game, activeGames.Contains(game));
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
    /// The Local Game that corresponds to this spot
    /// E.g. if the spot is the top-right spot, 
    /// returns the top-right local game
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    LocalGame GetGame(Spot spot)
    {
        if (spot == null) { return null; }
        Location loc = spot.Loc;
        return localGames[loc.Row, loc.Col];
    }
}