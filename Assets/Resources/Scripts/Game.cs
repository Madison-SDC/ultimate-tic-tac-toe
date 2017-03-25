using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    static Stack<Move> history = new Stack<Move>();
    /// <summary>
    /// All 9 boards on scene
    /// </summary>
    static GameObject[] boards;

    static GameObject activeBoard;
    static bool firstTurn;

    public static Player p1, p2;

    public static Color disabledColor = Color.gray,
        enabledColor = Color.white;

    //static Sprite p1Sprite, p2Sprite;

    /// <summary>
    /// The current active board
    /// </summary>
    public GameObject ActiveBoard
    {
        get { return activeBoard; }
        set
        {
            activeBoard = value;

            bool activeIsFull = activeBoard.GetComponent<Board>().IsFull;
            bool gameOver = GetComponent<Board>().GameOver;
            foreach (GameObject board in boards)
            {
                if (gameOver) { Disable(board); }
                else if (activeIsFull) { Enable(board); }
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
    /// Reset the game
    /// </summary>
	void Start()
    {
        InstantiateBoards();
        activeBoard = null;
        firstTurn = true;
        p1 = new Player(Board.P1, Color.red, Resources.Load<Sprite>("Sprites/x"));
        p2 = new Player(Board.P2, Color.blue, Resources.Load<Sprite>("Sprites/o"));
    }

    /// <summary>
    /// Reset upon right click
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Reset();
        }
    }

    /// <summary>
    /// Resets the game
    /// Empties boards, clears winners
    /// </summary>
    public void Reset()
    {
        firstTurn = true;
        foreach (GameObject board in boards)
        {
            foreach (BoardSpot spot in board.GetComponentsInChildren<BoardSpot>())
            {
                spot.Clear();
            }
            board.GetComponent<Board>().Reset();
            Enable(board);
        }
        GameObject.Find("Global Board").GetComponent<Board>().Reset();
    }

    /// <summary>
    /// Populate the boards array with the nine boards on scene
    /// </summary>
    internal void InstantiateBoards()
    {
        boards = new GameObject[9];
        boards[0] = GameObject.Find("Top Left Board");
        boards[1] = GameObject.Find("Top Mid Board");
        boards[2] = GameObject.Find("Top Right Board");
        boards[3] = GameObject.Find("Center Left Board");
        boards[4] = GameObject.Find("Center Mid Board");
        boards[5] = GameObject.Find("Center Right Board");
        boards[6] = GameObject.Find("Bottom Left Board");
        boards[7] = GameObject.Find("Bottom Mid Board");
        boards[8] = GameObject.Find("Bottom Right Board");
    }
    
    /// <summary>
    /// Disables the button for all of the spots of this board
    /// </summary>
    /// <param name="board"></param>
    public static void Disable(GameObject board)
    {
        board.GetComponent<Board>().Active = false;

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
    public static void Enable(GameObject board)
    {
        board.GetComponent<Board>().Active = true;

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
    /// Advances the state of the board
    /// </summary>
    /// <param name="spot"></param>
    public void Play(BoardSpot spot, bool undo = false)
    {
        Board board = spot.transform.parent.GetComponent<Board>();

        // update logic
        spot.Fill(undo ? Board.EMPTY : ActivePlayer.Turn);
        board.FillSpot(spot.name, undo ? Board.EMPTY : ActivePlayer.Turn); // check winner

        // record this move
        if(!undo) { history.Push(new Move(board, spot)); }
        
        firstTurn = !firstTurn; // toggle turn
        string boardName = undo ? spot.transform.parent.name : spot.name + " Board";
        ActiveBoard = GameObject.Find(boardName); // assign the next board
    }

    /// <summary>
    /// Undo the most recent move, if there was one
    /// </summary>
    public void Undo()
    {
        Play(history.Pop().Spot, true); // remove piece
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
