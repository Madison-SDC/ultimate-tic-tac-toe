using UnityEngine;

public class Settings {
    public static Player
        p1 = new Player(1, Color.red, Resources.Load<Sprite>("Sprites/x"), "P1"),
        p2 = new MonteCarloAI(null, 2, Color.blue, Resources.Load<Sprite>("Sprites/o"), "P2");
}
