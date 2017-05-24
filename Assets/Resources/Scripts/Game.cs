using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Game : Board
{
    static Game currentGame;

    /// <summary>
    /// Previous moves made for this game
    /// </summary>
    internal Stack<Move> history;

    /// <summary>
    /// All move that have been undone
    /// Resets when a "fresh" move is made
    /// </summary>
    Stack<Move> future;

    /// <summary>
    /// Whether the board is currently resetting
    /// </summary>
    internal bool resetting;

    /// <summary>
    /// The spot that contains the next move for this board. 
    /// Null if no next move
    /// </summary>
    internal SpotUI nextMove;
    
    Board[,] boards;
    Board activeBoard;
    bool firstTurn;
    internal Player p1, p2;
    Color disabledColor, enabledColor;

    /// <summary>
    /// The exact number of humans in the game
    /// </summary>
    bool zeroHumans;

    /// <summary>
    /// The win count for this game
    /// </summary>
    int p1Wins, p2Wins, ties;

    /// <summary>
    /// Time remaining until the AI previews its move (in ms)
    /// </summary>
    private float previewTime;

    /// <summary>
    /// How long the AI takes to preview its move (in ms)
    /// </summary>
    private float previewTimer;

    /// <summary>
    /// Time remaining until AI confirms the previewed move (in ms)
    /// </summary>
    private float confirmTime;

    /// <summary>
    /// How long the AI takes to confirm its move (in ms)
    /// </summary>
    private float confirmTimer;

    public const int REGULAR = 0,
        UNDO = 1,
        REDO = 2,
        PREVIEW = 3;

    /// <summary>
    /// The current game on the screen
    /// </summary>
    public static Game CurrentGame { get { return currentGame; } }
    
    /// <summary>
    /// All 9 boards on scene
    /// </summary>
    public Board[,] Boards { get { return boards; } }

    /// <summary>
    /// The current active board
    /// </summary>
    public Board ActiveBoard
    {
        get { return activeBoard; }
        set
        {
            activeBoard = value;

            // active is null on opening move
            bool activeIsNull = activeBoard == null;
            bool activeIsOver = !activeIsNull
                && activeBoard.GameOver;

            bool gameOver = GameOver;

            foreach (Board board in boards)
            {
                if (gameOver || board.GameOver) { Disable(board); }
                else if (activeIsNull || activeIsOver) { Enable(board); }
                else if (board != activeBoard) { Disable(board); }
                else { Enable(board); }
            }
        }
    }

    public Move PreviousMove
    {
        get
        {
            return history.Peek();
        }
    }

    /// <summary>
    /// Whether it is the first player's turn
    /// </summary>
    public bool FirstTurn
    {
        get { return firstTurn; }
    }

    /// <summary>
    /// The current player
    /// </summary>
    public Player ActivePlayer
    {
        get { return FirstTurn ? P1 : P2; }
    }
    public Player P1 { get { return p1; } }
    public Player P2 { get { return p2; } }

    public Color DisabledColor { get { return disabledColor; } }
    public Color EnabledColor { get { return enabledColor; } }

    /// <summary>
    /// True if the game board is entirely clear (no moves have been made)
    /// </summary>
    public bool IsClear {
        get {
            return history != null 
                && history.Count == 1;
        }
    }

    public bool HasNextMove { get { return nextMove != null; } }

    public override bool IsFull
    {
        get
        {
            foreach(Board board in boards)
            {
                if(!board.GameOver) { return false; }
            }
            return true;
        }
    }

    /// <summary>
    /// The number of times p1 has won this game
    /// </summary>
    public int P1Wins
    {
        get
        {
            return p1Wins;
        }

        set
        {
            p1Wins = value;
        }
    }

    /// <summary>
    /// the number of times p2 has won this game
    /// </summary>
    public int P2Wins
    {
        get
        {
            return p2Wins;
        }

        set
        {
            p2Wins = value;
        }
    }

    /// <summary>
    /// the number of times no player has won this game
    /// </summary>
    public int Ties
    {
        get
        {
            return ties;
        }

        set
        {
            ties = value;
        }
    }

    /// <summary>
    /// Whether exactly zero humans are playing this game
    /// </summary>
    public bool ZeroHumans
    {
        get
        {
            return zeroHumans;
        }
    }

    /// <summary>
    /// Reset the game
    /// </summary>
    internal virtual void Start()
    {
        history = new Stack<Move>();
        history.Push(new Move(null, null));
        future = new Stack<Move>();
        InstantiateBoards();
        InitializeSpots();
        resetting = false;
        activeBoard = null;
        firstTurn = true;
        disabledColor = Color.gray;
        enabledColor = Color.white;
        p1 = Settings.p1;
        p2 = Settings.p2;

        previewTimer = 0.5f;
        confirmTimer = 1f;
        
        // both computers: go super speed!
        if(p1 is AI && p2 is AI)
        {
            zeroHumans = true;
            previewTimer = 0;
            confirmTimer = 0.1f;
        }

        previewTime = previewTimer;
        confirmTime = confirmTimer;

        game = this;
        currentGame = this; // most recent game is the current game

        Active = true;
    }

    /// <summary>
    /// Playing if the game is not over, or if the AI is making a move
    /// </summary>
    /// <returns></returns>
    bool Playing()
    {
        bool gameOver = GameOver;

        if (gameOver)
        {
            // game isn't really over, just next move ends it
            return ActivePlayer is AI && HasNextMove;
        }
        return true;
    }

    /// <summary>
    /// If AI's turn, preview or confirm move
    /// </summary>
    void Update()
    {
        if (Playing() && ActivePlayer is AI)
        {
            if (HasNextMove)
            {
                if (confirmTime <= 0)
                {
                    Confirm();
                    confirmTime = confirmTimer;
                }
                else
                {
                    confirmTime -= Time.deltaTime;
                }
            }
            else
            {
                if (previewTime <= 0)
                {
                    PreviewMove(((AI)ActivePlayer).BestMove(this));
                    previewTime = previewTimer;
                }
                else
                {
                    previewTime -= Time.deltaTime;
                }
            }
        }
        else if (GameOver)
        {
            if(!HasNextMove)
            {
                if(Owner == null) { Ties++; }
                else if(Owner.Turn == 1) { p1Wins++; }
                else if(Owner.Turn == 2) { p2Wins++; }
            }
            if(zeroHumans) { Reset(); }
        }
    }

    /// <summary>
    /// Resets the game
    /// Empties boards, clears winners
    /// </summary>
    new public void Reset()
    {
        if(HasNextMove) { Undo(); }
        while(history.Count > 1) { Undo(); }
    }

    /// <summary>
    /// Populate the boards array with the nine boards on scene
    /// </summary>
    internal void InstantiateBoards()
    {
        boards = new Board[3,3];
        boards[0,0] = GameObject.Find("Top Left Board").GetComponent<Board>();
        boards[0,1] = GameObject.Find("Top Mid Board").GetComponent<Board>();
        boards[0,2] = GameObject.Find("Top Right Board").GetComponent<Board>();
        boards[1,0] = GameObject.Find("Center Left Board").GetComponent<Board>();
        boards[1,1] = GameObject.Find("Center Mid Board").GetComponent<Board>();
        boards[1,2] = GameObject.Find("Center Right Board").GetComponent<Board>();
        boards[2,0] = GameObject.Find("Bottom Left Board").GetComponent<Board>();
        boards[2,1] = GameObject.Find("Bottom Mid Board").GetComponent<Board>();
        boards[2,2] = GameObject.Find("Bottom Right Board").GetComponent<Board>();
    }

    /// <summary>
    /// Disables the button for all of the spots of this board
    /// </summary>
    /// <param name="board"></param>
    public static void Disable(Board board)
    {
        if (board == null) { return; }

        board.Active = false;
        
        foreach (SpotUI spot in board.Spots) { spot.button.interactable = false; }
    }

    /// <summary>
    /// Enables the button for all of the spots of this board
    /// if that spot hasn't been taken (clicked before)
    /// </summary>
    /// <param name="board"></param>
    public void Enable(Board board)
    {
        if (board == null) { return; }

        board.Active = true;
        
        foreach (SpotUI spot in board.Spots)
        {
            // all empty spots
            if (spot.Owner == null) { spot.button.interactable = true; }
        }
    }

    /// <summary>
    /// Advances or reverts the state of the board by one turn
    /// </summary>
    /// <param name="spot">The spot to play</param>
    /// <param name="undo">Whether to undo a move</param>
    /// <param name="prevActiveBoard">The previous active board</param>
    public void Play(SpotUI spot, int moveType = REGULAR, Board prevActiveBoard = null)
    {
        // update logic
        spot.ParentBoard.FillSpot(spot, moveType == UNDO ? null : ActivePlayer);

        // record this move
        if (moveType != UNDO && moveType != PREVIEW)
        {
            history.Push(new Move(ActiveBoard, spot));
            if(moveType != REDO)
            {
                future = new Stack<Move>();
            }
        }

        firstTurn = !firstTurn; // toggle turn

        // On an undo, the new active board is the board 
        // that the previous move pointed to.
        ActiveBoard = moveType == UNDO ? prevActiveBoard : spot.RelativeBoard;

        nextMove = null; // no next move chosen as this one has been confirmed
    }

    /// <summary>
    /// Undo the most recent move, if there was one
    /// </summary>
    public virtual void Undo()
    {
        if(nextMove)
        {
            ShowMove(nextMove, true); // undo preview only
            nextMove = null;
            return;
        }

        else if (history.Count > 1) // cannot undo original instantiation move
        {
            Move move = history.Pop();
            Play(move.Spot, UNDO, move.Board); // remove piece
            future.Push(move);
            if(ActivePlayer is AI && history.Count > 1)
            {
                move = history.Pop();
                Play(move.Spot, UNDO, move.Board); // remove piece
                future.Push(move);
            }
        }
    }

    public virtual void Redo()
    {
        if(future.Count > 0)
        {
            if (nextMove) { ShowMove(nextMove, true); } // undo the preview move
            Move move = future.Pop();
            Play(move.Spot, REDO); // act as though this is a new move

            // redo AI moves
            if (ActivePlayer is AI) { Play(future.Pop().Spot, REDO); }
        }
    }

    /// <summary>
    /// Shows the results of making this move, but does not confirm it
    /// </summary>
    /// <param name="spot"></param>
    public void PreviewMove(SpotUI spot)
    {
        // undo previous move shown, then show new move
        if (nextMove) { ShowMove(nextMove, true); }
        nextMove = spot;
        ShowMove(nextMove);
    }

    /// <summary>
    /// Display the results of making a move at this spot, do not confirm it
    /// </summary>
    /// <param name="spot">The spot to play on</param>
    /// <param name="undo">Whether to undo the shown move</param>
    private void ShowMove(SpotUI spot, bool undo = false)
    {
        // the owner of the shown move
        Player player = undo ? null : ActivePlayer;
        
        bool gameOverBefore = GameOver;

        // highlight local and global board
        spot.ParentBoard.FillSpot(spot, player);

        bool gameOverAfter = GameOver;

        // highlight next playable board(s) if game hasn't ended
        if (!gameOverBefore && !gameOverAfter)
        {
            Outline(spot.RelativeBoard, undo);
        }
    }

    /// <summary>
    /// Outlines the playable boards if <paramref name="board"/> is the 
    /// active board. Will outline all boards if <paramref name="board"/> 
    /// is full
    /// </summary>
    /// <param name="board">The next active board</param>
    /// <param name="remove">True if remove outline</param>
    new private void Outline(Board board, bool remove)
    {
        // The color of the player who will play next (not currently)
        Color otherPlayerColor = ActivePlayer == P1 ? P2.Color : P1.Color;
        
        // simply remove the outline for all boards
        if(remove)
        {
            foreach (Board b in boards)
            {
                b.Outline.enabled = false;
            }
            return;
        }

        // only highlight active board if global game continues
        if (!GameOver)
        {
            if (board.GameOver) // outline all valid boards
            {
                foreach (Board b in boards)
                {
                    if (!b.GameOver)
                    {
                        b.Outline.enabled = true;
                        b.Outline.color = otherPlayerColor;
                    }
                }
            }
            else // active board not full, can be played on
            {
                board.Outline.enabled = true;
                board.Outline.color = otherPlayerColor;
            }
        } // if game is over, do nothing
    }

    /// <summary>
    /// Confirms the next move
    /// </summary>
    public void Confirm()
    {
        if(nextMove)
        {
            ShowMove(nextMove, true); // undo color changes
            Play(nextMove); // make it official
        }
    }

    public virtual bool CanUndo()
    {
        return !zeroHumans &&
            (!(ActivePlayer is AI) ||  GameOver) && // not AI turn
            (history.Count > 1  || HasNextMove); // something to undo
    }

    public virtual bool CanRedo()
    {
        return !(ActivePlayer is AI) && // cannot redo on AI turn
            future.Count > 0; // has move to redo
    }

    public virtual bool CanConfirm()
    {
        return !(ActivePlayer is AI) && // cannot confirm AI move
            HasNextMove; // has move to confirm
    }
}