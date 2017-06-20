using UnityEngine;

public class Settings {
    public static Player p1 = RandomAI(true), p2 = RandomAI(false);

    public static RandomAI RandomAI(bool firstPlayer)
    {
        int turn = firstPlayer ? 1 : 2;
        Color color = firstPlayer ? Color.red : Color.blue;
        Sprite sprite = 
            Resources.Load<Sprite>("Sprites/" + (firstPlayer ? "x" : "o"));
        string name = firstPlayer ? "X" : "O";
        return new RandomAI(null, turn, color, sprite, name);
    }
}
