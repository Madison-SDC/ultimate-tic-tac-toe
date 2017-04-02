using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    static Stack<Move> history = new Stack<Move>();
    
    /// <summary>
    /// All 9 boards on scene
    /// </summary>
    static Board[] boards;

    static Board activeBoard;
    static bool firstTurn;


        Player p1, p2;

    Color disabledColor, enabledColor, highlight;

    static BoardSpot nextMove;

    bool resetting;

    /// <summary>
    /// Delay between piece removal during reset (in ms)
    /// </summary>
    float time;

    /// <summary>
    /// Current time remaining until next removal (in ms)
    /// </summary>
    float timer;

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
                else if (activeIsNull || activeIsFull) { Enable(board); }
                else if (board != activeBoard) { Disable(board); }
            }

            if (!GetComponent<Board>().GameOver) { Enable(activeBoard); }
        }
    }

    /// <summary>
    /// Whether it is the first player's turn
    /// </summary>
    public static bool FirstTurn
    {
        get { return firstTurn; }
    }

    /// <summary>
    /// The current player's turn (Board.P1 or Board.P2)
    /// </summary>
    public static Player ActivePlayer
    {
        get { return FirstTurn ? p1 : p2; }
    }
    public Player P1 { get { return p1; } }
    public Player P2 { get { return p2; } }

    public Color DisabledColor { get { return disabledColor; } }
    public Color EnabledColor { get { return enabledColor; } }

    /// <summary>
    /// True if the game board is entirely clear (no moves have been made)
    /// </summary>
    public static bool IsClear { get { return history.Count == 1; } }

    public static bool HasNextMove { get { return nextMove != null; } }

    /// <summary>
    /// Reset the game
    /// </summary>
	void Start()
    {
        time = 0.1f;
        timer = time;
        InstantiateBoards();
        activeBoard = null;
        firstTurn = true;
        disabledColor = Color.gray;
        enabledColor = Color.white;
        highlight = Color.gray / 2;
        p1 = new Player(Board.P1, Color.red, Resources.Load<Sprite>("Sprites/x"));
        p2 = new Player(Board.P2, Color.blue, Resources.Load<Sprite>("Sprites/o"));
        history.Push(new Move(null, null));
        resetting = false;
    }

    private void Update()
    {
        if(IsClear)
        {
            resetting = false;
            timer = time; // reset timer
        }
        else if (resetting)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                Undo();
                timer += time;
            }
        }
    }

    /// <summary>
    /// Resets the game
    /// Empties boards, clears winners
    /// </summary>
    public void Reset()
    {
        resetting = true;
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

        for (int i = 0; i < board.transform.childCount; i++)
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
    public static void Enable(Board board)
    {
        if (board == null) { return; }

        board.Active = true;

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform spot = board.transform.GetChild(i);
            if (!spot.GetComponent<BoardSpot>().Clicked) // all unclicked spots
            {
                // show picture so it can be clicked
                spot.GetComponent<Image>().enabled = true;

                // can be clicked
                Button button = spot.GetComponent<Button>();
                button.interactable = true;

                // show player color when highlighted, blend in when not
                ColorBlock cb = button.colors;
                cb.highlightedColor = ActivePlayer.Color;
                cb.normalColor = board.GetComponent<Image>().color;
                button.colors = cb;
            }
        }
    }

    /// <summary>
    /// Advances or reverts the state of the board
    /// </summary>
    /// <param name="spot">The spot to play</param>
    /// <param name="undo">Whether to undo a move</param>
    /// <param name="prevActiveBoard">The previous active board</param>
    public void Play(BoardSpot spot, bool undo = false, Board prevActiveBoard = null)
    {
        // update logic
        spot.Fill(undo ? Board.EMPTY : ActivePlayer.Turn);
        spot.Board.FillSpot(spot.name, undo ? Board.EMPTY : ActivePlayer.Turn);

        // record this move
        if (!undo) { history.Push(new Move(ActiveBoard, spot)); }

        firstTurn = !firstTurn; // toggle turn

        // On an undo, the new active board is the board 
        // that the previous move pointed to.
        ActiveBoard = undo ? prevActiveBoard : spot.RelativeBoard;

        nextMove = null; // no next move chosen as this one has been confirmed
    }

    /// <summary>
    /// Undo the most recent move, if there was one
    /// </summary>
    public void Undo()
    {
        if(nextMove)
        {
            ShowMove(nextMove, true);
            nextMove = null;
        }

        if (history.Count > 1) // cannot undo original instantiation move
        {
            Move move = history.Pop();
            Play(move.Spot, true, move.Board); // remove piece
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
            Highlight(spot.RelativeBoard, undo);
        }
    }

    /// <summary>
    /// Highlights the playable boards if <paramref name="board"/> is the 
    /// active board. Will highlight all boards if <paramref name="board"/> 
    /// is full
    /// </summary>
    /// <param name="board">The next active board</param>
    /// <param name="undo">True if darken back to original hue, 
    /// false to lighten</param>
    private void Highlight(Board board, bool undo)
    {
        Color change = (undo ? -1 : 1) * highlight;
        
        // only highlight active board if game continues
        if (!GetComponent<Board>().GameOver)
        {
            if (board.IsFull) // highlight all other boards
            {
                foreach (Board b in boards)
                {
                    if (b != board)
                    {
                        b.GetComponent<Image>().color += change;
                    }
                }
            }
            else // board not full, can be played on
            {
                board.GetComponent<Image>().color += change;
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
}
