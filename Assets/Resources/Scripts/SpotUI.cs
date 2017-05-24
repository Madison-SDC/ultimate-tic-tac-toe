using UnityEngine;
using UnityEngine.UI;

public class SpotUI : MonoBehaviour {
    private bool clicked;
    private static Sprite empty;
    private Location loc;
    internal Player owner;
    public Button button;

    /// <summary>
    /// The board this is a part of
    /// </summary>
    public Board ParentBoard
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
    /// Cycle upward through transforms
    /// </summary>
    public Game Game
    {
        get
        {
            Transform t = transform;
            Game g = t.GetComponent<Game>();
            while(g == null)
            {
                t = t.parent;
                g = t.GetComponent<Game>();
            }
            return g;
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

    void Awake()
    {
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
        if(!(Game.ActivePlayer is AI))
            Game.PreviewMove(this);
    }
    

    public virtual void Fill(Player player)
    {
        owner = player;
        Image image = GetComponent<Image>();
        if (owner != null)
        {
            image.enabled = true;
            image.sprite = Game.ActivePlayer.Sprite;
            image.color = Game.ActivePlayer.Color;

            ColorBlock cb = button.colors;
            cb.disabledColor = Game.ActivePlayer.Color;
            button.colors = cb; // weird workaround for struct vs class
        }
        else // clear spot
        {
            image.sprite = empty;
            button.interactable = true;
        }
    }

    /// <summary>
    /// Whether this spot can be owned by <paramref name="p"/>. 
    /// True if there's a chance, false if definitely not possible
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public virtual bool CanBeOwnedBy(Player p)
    {
        return Owner == null 
            || Owner == p;
    }
}
