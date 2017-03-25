using UnityEngine;
using UnityEngine.UI;

public class BoardSpot : Button {
    /// <summary>
    /// Whether this spot has been clicked
    /// </summary>
    private bool clicked;

    private static Sprite empty;

    public bool Clicked { get { return clicked; }
        set
        {
            clicked = value;
            interactable = !clicked;
        }
    }

    protected override void Start()
    {
        Clicked = false;
        empty = Resources.Load<Sprite>("Sprites/empty");
    }

    public void OnClick()
    {
        transform.parent.parent.GetComponent<Game>().Play(this);
    }

    public void Clear()
    {
        Clicked = false;
        GetComponent<Image>().sprite = empty;
        GetComponent<Image>().color = Color.white;

        ColorBlock cb = GetComponent<Button>().colors;
        cb.disabledColor = Game.enabledColor;
        cb.highlightedColor = Game.FirstTurn ? Game.p1.Color : Game.p2.Color;
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
            image.sprite = Game.ActivePlayerSprite;
            image.color = Game.ActivePlayerColor;

            ColorBlock cb = colors;
            cb.disabledColor = Game.ActivePlayerColor;
            colors = cb; // weird workaround for struct vs class
        }

    }
}
