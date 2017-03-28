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

    public static Player p1, p2;

    public static Color disabledColor = Color.gray,
        enabledColor = Color.white;

    //static Sprite p1Sprite, p2Sprite;

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

    /// <summary>
    /// True if the game board is entirely clear (no moves have been made)
    /// </summary>
    public static bool IsClear { get { return history.Count == 1; } }

    /// <summary>
    /// Reset the game
    /// </summary>
	void Start()
    {
        InstantiateBoards();
        activeBoard = null;
        firstTurn = true;
        p1 = new Player(Board.P1, Color.red, Resources.Load<Sprite>("Sprites/x"));
        p2 = new Player(Board.P2, Color.blue, Resources.Load<Sprite>("Sprites/o"));
        history.Push(new Move(null, null));
    }
    
    /// <summary>
    /// Resets the game
    /// Empties boards, clears winners
    /// </summary>
    public void Reset()
    {
        while(history.Count > 1)
        {
            Undo();
        }
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
        if(board == null) { return; }

        board.Active = false;

        for (int i = 0; i < board.transform.childCount; i++)
        {
            BoardSpot spot = board.transform.GetChild(i).GetComponent<BoardSpot>();
            spot.interactable = false;
            if (!spot.Clicked)
            {
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
        if(board == null) { return; }

        board.Active = true;

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform spot = board.transform.GetChild(i);
            if (!spot.GetComponent<BoardSpot>().Clicked) // all unclicked spots
            {
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
    /// <param name="previous">The move to undo</param>
    public void Play(BoardSpot spot, bool undo = false, Move previous = null)
    {
        // update logic
        spot.Fill(undo ? Board.EMPTY : ActivePlayer.Turn);
        spot.Board.FillSpot(spot.name, undo ? Board.EMPTY : ActivePlayer.Turn); // check winner

        // record this move
        if(!undo) { history.Push(new Move(ActiveBoard, spot)); }
        
        firstTurn = !firstTurn; // toggle turn

        /* On an undo, the new active board is the board
         * that the previous move pointed to.           
        */
        Board localBoard = GameObject.Find(spot.name + " Board").GetComponent<Board>();
        ActiveBoard = undo ? previous.Board : localBoard; // assign the next board
    }

    /// <summary>
    /// Undo the most recent move, if there was one
    /// </summary>
    public void Undo()
    {
        if (history.Count > 1) // cannot undo original instantiation move
        {
            Move move = history.Pop();
            Play(move.Spot, true, move); // remove piece
        }
    }

    /// <summary>
    /// Update image of the spot and its disabled color
    /// </summary>
    /// <param name="spot"></param>
    /// <param name="first"></param>
    internal static void UpdateImage(BoardSpot spot, bool first)
    {
        Image image = spot.GetComponent<Image>();
        ColorBlock cb = spot.GetComponent<Button>().colors;

        image.sprite = ActivePlayer.Sprite;
        image.color = ActivePlayer.Color;

        cb.disabledColor = ActivePlayer.Color;
        spot.GetComponent<Button>().colors = cb; // weird workaround for struct vs class
    }
}
