using UnityEngine;

public class GameController : MonoBehaviour
{
    GlobalGame game;

    public GlobalGame Game
    {
        get { return game; }
        set { game = value; }
    }

    public void Confirm()
    {
        game.Confirm();
    }
}
