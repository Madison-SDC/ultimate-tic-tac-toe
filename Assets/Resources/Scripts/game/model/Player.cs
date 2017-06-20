using UnityEngine;
using System.Collections;

public class Player {

    private int turn;
    private Color color;
    private Sprite sprite;
    private string name;

    public int Turn { get { return turn; } set { turn = value; } }

    public Color Color { get { return color; } set { color = value; } }

    public Sprite Sprite { get { return sprite; }  set { sprite = value; } }

    public string Name { get { return name; } set { name = value; } }

    public Player(int turn, Color color, Sprite sprite, string name)
    {
        Turn = turn;
        Color = color;
        Sprite = sprite;
        Name = name;
    }
}
