using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    Game game;
    public Image image;
    
    /// <summary>
    /// How similar the color of the board will be to the color of the winner
    /// </summary>
    public float highlightPercent;

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
        if(hasWinner) { newColor = OpaqueColor(highlightPercent*game.Winner.Color); }
        else { newColor = enabled ? Color.white : Color.gray; }
        image.color = newColor;
    }

    /// <summary>
    /// Returns the opaque version of the given color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    Color OpaqueColor(Color color)
    {
        return new Color(color.r, color.g, color.b, 1);
    }
}
