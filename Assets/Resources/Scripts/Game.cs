using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    /// <summary>
    /// All 9 boards on scene
    /// </summary>
    static GameObject[] boards;

    static GameObject activeBoard;
    static bool firstTurn;

    public static Color p1Color = Color.red,
        p2Color = Color.blue,
        disabledColor = Color.gray,
        enabledColor = Color.white,
        enabledOffset = Color.gray / 2;

    static Sprite p1Sprite, p2Sprite;

    /// <summary>
    /// The current active board
    /// </summary>
    public GameObject ActiveBoard
    {
        get { return activeBoard; }
        set
        {
            activeBoard = value;
            SetActiveBoard(activeBoard);
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
    public static int CurrentPlayer
    {
        get { return FirstTurn ? Board.P1 : Board.P2; }
    }

    /// <summary>
    /// The color of the active player
    /// </summary>
    static Color ActivePlayerColor
    {
        get
        {
            return firstTurn ? p1Color : p2Color;
        }
    }

    /// <summary>
    /// The sprite of the active player's piece
    /// </summary>
    static Sprite ActivePlayerSprite
    {
        get
        {
            return firstTurn ? p1Sprite : p2Sprite;
        }
    }

    /// <summary>
    /// Reset the game
    /// </summary>
	void Start()
    {
        InstantiateBoards();
        activeBoard = null;
        firstTurn = true;
        p1Sprite = Resources.Load<Sprite>("Sprites/x");
        p2Sprite = Resources.Load<Sprite>("Sprites/o");
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
        foreach (GameObject board in boards)
        {
            foreach (BoardSpot spot in board.GetComponentsInChildren<BoardSpot>())
            {
                spot.Reset();
            }
            board.GetComponent<Board>().Reset();
            Enable(board);
            board.GetComponent<Image>().color = Color.white;
        }
        GameObject.Find("Global Board").GetComponent<Board>().Reset();
        firstTurn = true;
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
    /// Disables all boards except exception
    /// If exception is full, enables all boards
    /// </summary>
    /// <param name="active"></param>
    public void SetActiveBoard(GameObject active)
    {
        bool activeIsFull = active.GetComponent<Board>().IsFull;
        bool gameOver = GetComponent<Board>().GameOver;
        foreach (GameObject board in boards)
        {
            if(gameOver) { Disable(board); }
            else if (activeIsFull) { Enable(board); }
            else if (board != active) { Disable(board); }
        }
    }

    /// <summary>
    /// Disables the button for all of the spots of this board
    /// </summary>
    /// <param name="board"></param>
    public static void Disable(GameObject board)
    {
        //board.GetComponent<Board>().Active = false;
        //*
        if (!board.GetComponent<Board>().GameOver)
        { board.GetComponent<Image>().color = disabledColor; }
        // */

        for (int i = 0; i < board.transform.childCount; i++)
        {
            BoardSpot spot = board.transform.GetChild(i).GetComponent<BoardSpot>();
            spot.GetComponent<Button>().interactable = false;
            if (!spot.Clicked) { spot.GetComponent<Image>().color = board.GetComponent<Image>().color; }
        }
    }

    /// <summary>
    /// Enables the button for all of the spots of this board
    /// if that spot hasn't been taken (clicked before)
    /// </summary>
    /// <param name="board"></param>
    public static void Enable(GameObject board)
    {
        //board.GetComponent<Board>().enabled = true;
        //*
        if (!board.GetComponent<Board>().GameOver)
        {
            board.GetComponent<Image>().color = enabledColor;
        }
        //*/

        for (int i = 0; i < board.transform.childCount; i++)
        {
            Transform spot = board.transform.GetChild(i);
            if (!spot.GetComponent<BoardSpot>().Clicked) // unclicked spot
            {
                // blend in to background by default
                spot.GetComponent<Image>().color = board.GetComponent<Image>().color;

                Button button = spot.GetComponent<Button>();
                button.interactable = true;

                // when highlighted display player color
                ColorBlock cb = button.colors;
                cb.highlightedColor = FirstTurn ? p1Color : p2Color;
                button.colors = cb;
            }
        }
    }

    /// <summary>
    /// Advances the state of the board
    /// </summary>
    /// <param name="spot"></param>
    public void FillSpot(GameObject spot)
    {
        // update visual
        UpdateImage(spot.GetComponent<BoardSpot>(), firstTurn);

        // update logic
        spot.GetComponent<BoardSpot>().Clicked = true; // can't re-enable a clicked spot
        spot.GetComponent<Button>().interactable = false; // can't use a filled spot

        spot.transform.parent.GetComponent<Board>().FillSpot(spot.name, CurrentPlayer);

        firstTurn = !firstTurn; // toggle turn

        ActiveBoard = GameObject.Find(spot.name + " Board"); // assign the next board
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

        image.sprite = ActivePlayerSprite;
        image.color = ActivePlayerColor;

        cb.disabledColor = ActivePlayerColor;
        spot.GetComponent<Button>().colors = cb; // weird workaround for struct vs class
    }
}
