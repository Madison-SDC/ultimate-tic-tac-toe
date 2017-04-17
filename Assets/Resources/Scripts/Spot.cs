using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Spot : Button {
    private bool clicked;
    private static Sprite empty;
    private Location loc;
    internal Player owner;

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

    /// <summary>
    /// The board that corresponds to this spot. 
    /// If this is the bottom right spot, returns the bottom right board
    /// </summary>
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

    public Location Loc { get { return loc; } }

    public Player Owner
    {
        get
        {
            return owner;
        }
    }

    protected override void Awake()
    {
        Clicked = false;
        empty = Resources.Load<Sprite>("Sprites/empty");
        loc = StringToLoc(tag);
    }

    public static Location StringToLoc(string str)
    {
        int r = -1, c = -1;

        if(str.Contains("Top")) { r = 0; }
        else if(str.Contains("Center")) { r = 1; }
        else if(str.Contains("Bottom")) { r = 2; }

        if(str.Contains("Left")) { c = 0; }
        else if(str.Contains("Mid")) { c = 1; }
        else if(str.Contains("Right")) { c = 2; }

        // invalid string
        if(r == -1 || c == -1) { return null; }

        return new Location(r, c);
    }

    public void OnClick()
    {
        Game.UpdateDisplay(this);
    }

    public void Clear()
    {
        Clicked = false;
        GetComponent<Image>().sprite = empty;
    }

    public void Fill(Player player)
    {
        owner = player;
        if (Owner != null)
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
        else
        {
            Clear();
        }
    }
}
