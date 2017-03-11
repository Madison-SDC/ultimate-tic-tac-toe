using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    /// <summary>
    /// All 9 boards on scene
    /// </summary>
    static GameObject[] boards;

    static GameObject activeBoard;
    static bool firstTurn;

    /// <summary>
    /// The current active board
    /// </summary>
    public static GameObject ActiveBoard
    {
        get { return activeBoard; }
        set
        {
            activeBoard = value;
            SetActiveBoard(activeBoard);
            Enable(activeBoard);
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
        get
        {
            return FirstTurn ? Board.P1 : Board.P2;
        }
    }

	void Start () {
        InstantiateBoards();
        activeBoard = null;
        firstTurn = true;
	}

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
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
            foreach(BoardSpot spot in board.GetComponentsInChildren<BoardSpot>())
            {
                spot.Reset();
            }
            Enable(board);
            board.GetComponent<Image>().color = Color.white;
            board.GetComponent<Board>().Reset();
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
    /// Disables all boards except exception
    /// If exception is full, enables all boards
    /// </summary>
    /// <param name="active"></param>
    public static void SetActiveBoard(GameObject active)
    {
        bool isFull = active.GetComponent<Board>().IsFull;
        foreach (GameObject board in boards)
        {
            if (!isFull && board != active) { Disable(board); }
            if(isFull) { Enable(board); }
        }
    }

    /// <summary>
    /// Disables the button for all of the spots of this board
    /// </summary>
    /// <param name="board"></param>
    public static void Disable(GameObject board)
    {
        foreach (Button b in board.transform.GetComponentsInChildren<Button>())
        {
            b.interactable = false;
        }
    }

    /// <summary>
    /// Enables the button for all of the spots of this board
    /// if that spot hasn't been taken (clicked before)
    /// </summary>
    /// <param name="board"></param>
    public static void Enable(GameObject board)
    {
        for(int i = 0; i < board.transform.childCount; i++)
        {
            Transform spot = board.transform.GetChild(i);
            if(!spot.GetComponent<BoardSpot>().Clicked)
            {
                spot.GetComponent<Button>().interactable = true;
            }
        }
    }

    /// <summary>
    /// Advances the state of the board
    /// </summary>
    /// <param name="spot"></param>
    public static void FillSpot(GameObject spot)
    {
        // update visual
        UpdateImage(spot.GetComponent<BoardSpot>(), firstTurn);

        // update logic
        spot.GetComponent<BoardSpot>().Clicked = true; // can't re-enable a clicked spot
        spot.GetComponent<Button>().interactable = false; // can't use a filled spot

        spot.transform.parent.GetComponent<Board>().FillSpot(spot.name, CurrentPlayer);
        ActiveBoard = GameObject.Find(spot.name + " Board"); // assign the next board
        firstTurn = !firstTurn; // toggle turn
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
        Color imageColor = first ? Color.red : Color.blue;

        image.sprite = Resources.Load<Sprite>(first ? "Sprites/x" : "Sprites/o");
        image.color = imageColor;

        cb.disabledColor = imageColor;
        spot.GetComponent<Button>().colors = cb; // weird workaround for struct vs class
    }
}
