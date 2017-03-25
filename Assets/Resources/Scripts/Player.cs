using UnityEngine;
using System.Collections;

public class Player {

    private int turn;
    private Color color;
    private Sprite sprite;

    public int Turn { get { return turn; } set { turn = value; } }

    public Color Color { get { return color; } set { color = value; } }

    public Sprite Sprite { get { return sprite; }  set { sprite = value; } }

    public Player(int turn, Color color, Sprite sprite)
    {
        Turn = turn;
        Color = color;
        Sprite = sprite;
    }
}
