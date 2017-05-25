using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    Game game;
    public Image image;

    public Game Game
    {
        get { return game; }
        set
        {
            if(game != value)
            {
                if(game != null)
                {
                    game.WinnerChanged -= HandleWinnerChanged;
                    game.EnabledChanged -= HandleEnabledChanged;
                }
                if(value != null)
                {
                    value.WinnerChanged += HandleWinnerChanged;
                    value.EnabledChanged += HandleEnabledChanged;
                    game = value;
                }
            }
        }
    }

    void HandleWinnerChanged(object o, GameEventArgs e)
    {
        UpdateColor();
    }

    void HandleEnabledChanged(object o, GameEventArgs e)
    {
        UpdateColor();
    }

    /// <summary>
    /// Change the color of the target image based on the winner 
    /// (if there is one) and the enabled value
    /// </summary>
    void UpdateColor()
    {
        bool enabled = game.Enabled;
        bool hasWinner = game.Winner != null;
        Color newColor;
        if(hasWinner) { newColor = game.Winner.Color; }
        else { newColor = enabled ? Color.white : Color.gray; }
        image.color = newColor;
    }
}
