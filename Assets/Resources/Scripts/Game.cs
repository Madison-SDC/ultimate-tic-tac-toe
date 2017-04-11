﻿using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    static Game currentGame;

    /// <summary>
    /// Previous moves made for this game
    /// </summary>
    Stack<Move> history;

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
    internal BoardSpot nextMove;
    
    Board[] boards;
    Board activeBoard;
    bool firstTurn;
    internal Player p1, p2;
    Color disabledColor, enabledColor;

    /// <summary>
    /// The current game on the screen
    /// </summary>
    public static Game CurrentGame { get { return currentGame; } }
    
    /// <summary>
    /// All 9 boards on scene
    /// </summary>
    public Board[] Boards { get { return boards; } }

    /// <summary>
    /// The current active board
    /// </summary>
    public Board ActiveBoard
    {
        get { return activeBoard; }
        set
        {
            activeBoard = value;

            bool activeIsNull = activeBoard == null;
            bool activeIsFull = !activeIsNull
                && activeBoard.GetComponent<Board>().IsFull;

            bool gameOver = GetComponent<Board>().GameOver;

            foreach (Board board in boards)
            {
                if (gameOver) { Disable(board); }

                // initialize all boards at first
                else if (activeIsNull || activeIsFull) { Enable(board); }

                else if (board != activeBoard) { Disable(board); }
            }

            if (!GetComponent<Board>().GameOver) { Enable(activeBoard); }
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

    /// <summary>
    /// Reset the game
    /// </summary>
    internal virtual void Start()
    {
        history = new Stack<Move>();
        history.Push(new Move(null, null));
        future = new Stack<Move>();
        InstantiateBoards();
        resetting = false;
        activeBoard = null;
        firstTurn = true;
        disabledColor = Color.gray;
        enabledColor = Color.white;
        p1 = new Player(Board.P1, Color.red, Resources.Load<Sprite>("Sprites/x"));
        p2 = new Player(Board.P2, Color.blue, Resources.Load<Sprite>("Sprites/o"));
        
        currentGame = this; // most recent game is the current game
    }
    
    public bool GameOver() { return GetComponent<Board>().GameOver; }

    /// <summary>
    /// Resets the game
    /// Empties boards, clears winners
    /// </summary>
    public void Reset()
    {
        if(HasNextMove) { Undo(); }
        while(history.Count > 1) { Undo(); }
    }

    /// <summary>
    /// Populate the boards array with the nine boards on scene
    /// </summary>
    internal void InstantiateBoards()
    {
        boards = new Board[9];
        boards[0] = GameObject.Find("Top Left Board").GetComponent<Board>();
        boards[1] = GameObject.Find("Top Mid Board").GetComponent<Board>();
        boards[2] = GameObject.Find("Top Right Board").GetComponent<Board>();
        boards[3] = GameObject.Find("Center Left Board").GetComponent<Board>();
        boards[4] = GameObject.Find("Center Mid Board").GetComponent<Board>();
        boards[5] = GameObject.Find("Center Right Board").GetComponent<Board>();
        boards[6] = GameObject.Find("Bottom Left Board").GetComponent<Board>();
        boards[7] = GameObject.Find("Bottom Mid Board").GetComponent<Board>();
        boards[8] = GameObject.Find("Bottom Right Board").GetComponent<Board>();
    }

    /// <summary>
    /// Disables the button for all of the spots of this board
    /// </summary>
    /// <param name="board"></param>
    public static void Disable(Board board)
    {
        if (board == null) { return; }

        board.Active = false;

        // ignore first child, is outline
        for (int i = 1; i < board.transform.childCount; i++)
        {
            BoardSpot spot = board.transform.GetChild(i).GetComponent<BoardSpot>();
            spot.interactable = false;
            if (!spot.Clicked)
            {
                spot.GetComponent<Image>().enabled = false; // hide image so board color is visible
                ColorBlock cb = spot.colors;
                cb.disabledColor = board.GetComponent<Image>().color;
                spot.colors = cb;
            }
        }
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

        // ignore first child: is outline
        for (int i = 1; i < board.transform.childCount; i++)
        {
            Transform spot = board.transform.GetChild(i);
            if (!spot.GetComponent<BoardSpot>().Clicked) // all unclicked spots
            {
                // show picture so it can be clicked
                spot.GetComponent<Image>().enabled = true;

                // can be clicked
                Button button = spot.GetComponent<Button>();
                button.interactable = !(ActivePlayer is AI);

                // show player color when highlighted, blend in when not
                ColorBlock cb = button.colors;
                cb.highlightedColor = ActivePlayer.Color;
                cb.normalColor = board.GetComponent<Image>().color;
                button.colors = cb;
            }
        }
    }

    /// <summary>
    /// Advances or reverts the state of the board by one turn
    /// </summary>
    /// <param name="spot">The spot to play</param>
    /// <param name="undo">Whether to undo a move</param>
    /// <param name="prevActiveBoard">The previous active board</param>
    public void Play(BoardSpot spot, bool undo = false, Board prevActiveBoard = null, bool redo = false)
    {
        // update logic
        spot.Fill(undo ? Board.EMPTY : ActivePlayer.Turn);
        spot.Board.FillSpot(spot.name, undo ? Board.EMPTY : ActivePlayer.Turn);

        // record this move
        if (!undo)
        {
            history.Push(new Move(ActiveBoard, spot));
            if(!redo)
            {
                future = new Stack<Move>();
            }
        }

        firstTurn = !firstTurn; // toggle turn

        // On an undo, the new active board is the board 
        // that the previous move pointed to.
        ActiveBoard = undo ? prevActiveBoard : spot.RelativeBoard;

        nextMove = null; // no next move chosen as this one has been confirmed
    }

    /// <summary>
    /// Undo the most recent move, if there was one
    /// </summary>
    public virtual void Undo()
    {
        if(nextMove)
        {
            ShowMove(nextMove, true);
            nextMove = null;
        }

        else if (history.Count > 1) // cannot undo original instantiation move
        {
            Move move = history.Pop();
            Play(move.Spot, true, move.Board); // remove piece
            future.Push(move);
        }
    }

    public virtual void Redo()
    {
        if(future.Count > 0)
        {
            Move move = future.Pop();
            Play(move.Spot, redo:true); // act as though this is a new move
        }
    }

    /// <summary>
    /// Shows the results of making this move, but does not confirm it
    /// </summary>
    /// <param name="spot"></param>
    public void UpdateDisplay(BoardSpot spot)
    {
        // undo previous move shown (if not confirmed)
        if (nextMove) { ShowMove(nextMove, true); }
        nextMove = spot;
        ShowMove(nextMove);
    }

    /// <summary>
    /// Display the results of making a move at this spot, do not confirm it
    /// </summary>
    /// <param name="spot">The spot to play on</param>
    /// <param name="undo">Whether to undo the shown move</param>
    private void ShowMove(BoardSpot spot, bool undo = false)
    {
        // the owner of the shown move
        int player = undo ? Board.EMPTY : ActivePlayer.Turn;
        
        bool gameOverBefore = GetComponent<Board>().GameOver;

        spot.Fill(undo ? Board.EMPTY : player); // update piece image

        bool gameOverAfter = GetComponent<Board>().GameOver;

        // highlight local and global board
        spot.Board.FillSpot(spot.name, player);

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
    private void Outline(Board board, bool remove)
    {
        // The color of the player who will play next (not currently)
        Color otherPlayerColor = ActivePlayer == P1 ? P2.Color : P1.Color;
        
        // only highlight active board if global game continues
        if (!GetComponent<Board>().GameOver)
        {
            if (board.IsFull) // outline all other boards
            {
                foreach (Board b in boards)
                {
                    if (b != board)
                    {
                        b.Outline.enabled = !remove;
                        b.Outline.color = otherPlayerColor;
                    }
                }
            }
            else // board not full, can be played on
            {
                board.Outline.enabled = !remove;
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

    public bool CanUndo()
    {
        return !(ActivePlayer is AI) // not AI's turn
            && (history.Count > 1 // and has a move to undo
            || HasNextMove); // or has a preview move
    }

    public bool CanRedo()
    {
        return !(ActivePlayer is AI) // not AI turn
            && future.Count > 0; // has move to redo
    }

    public bool CanConfirm()
    {
        return !(ActivePlayer is AI) // not AI turn
            && HasNextMove; // has move to redo
    }
}