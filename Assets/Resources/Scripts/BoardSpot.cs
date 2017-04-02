using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSpot : Button {
    private bool clicked;

    private static Sprite empty;

    /// <summary>
    /// Whether this has been clicked
    /// </summary>
    public bool Clicked { get { return clicked; }
        set
        {
            clicked = value;
            interactable = !clicked;
        }
    }

    /// <summary>
    /// The board this is a part of
    /// </summary>
    public Board Board
    {
        get
        {
            return transform.parent.GetComponent<Board>();
        }
    }

    public Board RelativeBoard
    {
        get
        {
            return GameObject.Find(name + " Board").GetComponent<Board>();
        }

    }

    /// <summary>
    /// The game this is a part of
    /// </summary>
    public Game Game
    {
        get
        {
            return transform.parent.parent.GetComponent<Game>();
        }
    }

    protected override void Start()
    {
        Clicked = false;
        empty = Resources.Load<Sprite>("Sprites/empty");
    }

    public void OnClick()
    {
        //Game.Play(this); // disabled during development of 0.4

        Game.UpdateDisplay(this);
    }

    public void Clear()
    {
        Clicked = false;
        GetComponent<Image>().sprite = empty;
        GetComponent<Image>().color = Color.white;

        ColorBlock cb = colors;
        cb.disabledColor = Game.EnabledColor;
        cb.highlightedColor = Game.FirstTurn ? Game.P1.Color : Game.P2.Color;
        GetComponent<Button>().colors = cb;
    }

    /// <summary>
    /// Update piece to reflect being occupied by 'player'
    /// If empty, be active and whatnot
    /// Else show the image and don't change
    /// Does not affect the board this belongs to
    /// </summary>
    /// <param name="turn"></param>
    public void Fill(int player)
    {
        if(player == Board.EMPTY)
        {
            Clear();
        }
        else
        {
            Clicked = true;
            Image image = GetComponent<Image>();
            image.enabled = true;
            image.sprite = Game.ActivePlayer.Sprite;
            image.color = Game.ActivePlayer.Color;

            ColorBlock cb = colors;
            cb.disabledColor = Game.ActivePlayer.Color;
            colors = cb; // weird workaround for struct vs class
        }
    }
}
