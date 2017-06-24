using UnityEngine;

public class Settings
{
    public static Player p1 = NewPlayer(true, false);
    public static Player p2 = NewPlayer(false, false);

    /// <summary>
    /// Returns a new player with the default settings
    /// First player: Red X
    /// Second player: Blue O
    /// </summary>
    /// <param name="firstPlayer">True if first player, false if second player</param>
    /// <param name="AI">True if RandomAI, false if human player</param>
    /// <returns></returns>
    public static Player NewPlayer(bool firstPlayer, bool AI)
    {
        int turn = firstPlayer ? 1 : 2;
        Color color = firstPlayer ? Color.cyan : Color.magenta;
        Sprite sprite = 
            Resources.Load<Sprite>("Sprites/" + (firstPlayer ? "x" : "o"));
        string name = firstPlayer ? "X" : "O";

        if (AI)
        {
            return new RandomAI(null, turn, color, sprite, name);
        }
        else
        {
            return new Player(turn, color, sprite, name);
        }
    }
}
