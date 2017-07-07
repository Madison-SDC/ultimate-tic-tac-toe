using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    Game game;
    public Image image;
    public Color disabledColor;
    public Color enabledColor;
    
    /// <summary>
    /// How similar the color of the board will be to the color of the winner
    /// Clamped between -1 and 1
    /// </summary>
    public float winnerOffset;

    /// <summary>
    /// The change in the board when it has a winner and changes enabled values
    /// Clamped between -1 and 1
    /// </summary>
    public float disabledOffset;

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
                    if (game is LocalGame)
                    {
                        ((LocalGame)game).OutlineChanged -= 
                            HandleOutlineChanged;
                    }
                }
                if(value != null)
                {
                    value.WinnerChanged += HandleWinnerChanged;
                    value.EnabledChanged += HandleEnabledChanged;
                    if (value is LocalGame)
                    {
                        ((LocalGame)value).OutlineChanged +=
                            HandleOutlineChanged;
                    }
                    game = value;
                }
                UpdateState();
            }
        }
    }

    void UpdateState()
    {
        HandleWinnerChanged(game, null);
        HandleEnabledChanged(game, null);
        HandleOutlineChanged(game, null);
    }

    void HandleWinnerChanged(object o, GameEventArgs e)
    {
        UpdateColor();
    }

    void HandleEnabledChanged(object o, GameEventArgs e)
    {
        UpdateColor();
    }

    void HandleOutlineChanged(object o, GameEventArgs e)
    {
        if(o is LocalGame)
        {
            UpdateOutline(((LocalGame)o).Outline);
        }
    }

    void UpdateOutline(Color color)
    {
        transform.GetChild(0).GetComponent<Image>().color = color;
    }

    /// <summary>
    /// Change the color of the target image based on the winner 
    /// (if there is one) and the enabled value
    /// </summary>
    void UpdateColor()
    {
        bool hasWinner = game.Winner != null;
        float offset = 0f;

        if(game.Enabled)
        {
            // enabled with winner, light offset
            if (hasWinner) { offset = winnerOffset; }
            
            // enabled with no winner, simply turn enabled color
            else
            {
                image.color = enabledColor;
                return;
            }
        }
        else
        {
            // disabled with winner, darker version of winner offset
            if (hasWinner) { offset = winnerOffset + disabledOffset; }
            
            // disabled, no winner, simply turn disabled color
            else
            {
                image.color = disabledColor;
                return;
            }
        }
        
        image.color = AugmentColor(game.Winner.Color, offset);
    }

    Color AugmentColor(Color color, float offset)
    {
        if(offset > 0)
        {
            return ApproachWhite(color, offset);
        }
        return ApproachBlack(color, offset);
    }

    Color ApproachWhite(Color color, float value)
    {
        float newR = ApproachWhite(color.r, value);
        float newG = ApproachWhite(color.g, value);
        float newB = ApproachWhite(color.b, value);
        return new Color(newR, newG, newB);
    }

    float ApproachWhite(float previous, float value)
    {
        float diff = 1 - previous;
        return previous + diff * value;
    }

    Color ApproachBlack(Color color, float offset)
    {
        float mult = 1 - offset;
        float newR = color.r * mult;
        float newG = color.g * mult;
        float newB = color.b * mult;
        return new Color(newR, newG, newB);
    }
}
