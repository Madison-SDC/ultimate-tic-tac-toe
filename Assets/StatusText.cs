using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this to the status text game object
/// </summary>
public class StatusText : MonoBehaviour
{
    Text text;
    GlobalGame game;

    public GlobalGame Game
    {
        get
        { return game; }
        set
        {
            if (game != null)
            {
                game.WinnerChanged -= HandleGameStateChanged;
                game.TurnChanged -= HandleGameStateChanged;
            }

            game = value;

            game.WinnerChanged += HandleGameStateChanged;
            game.TurnChanged += HandleGameStateChanged;
        }
    }

    private void Start()
    {
        text = GetComponent<Text>();
        if (game != null)
        {
            HandleGameStateChanged(null, null);
        }
    }

    public void HandleGameStateChanged(object o, GameEventArgs e)
    {
        if (game.GameOver())
        {
            if (game.Winner != null)
            {
                text.text = game.Winner.Name + " wins!";
                return;
            }
            text.text = "Tie game";
            return;
        }
        text.text = game.ActivePlayer().Name + "'s turn";
    }
}
