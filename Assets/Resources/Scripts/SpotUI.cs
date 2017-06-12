using UnityEngine;
using UnityEngine.UI;

public class SpotUI : MonoBehaviour {

    /// <summary>
    /// An invisible sprite, for spots without owners
    /// </summary>
    private static Sprite empty;

    public Button button;
    public int row, col;

    Spot spot;

    /// <summary>
    /// The spot this tracks and listens to
    /// </summary>
    public Spot Spot
    {
        get { return spot; }
        set
        {
            if(spot != value)
            {
                if(spot != null)
                {
                    spot.OwnerChanged -= HandleSpotOwnerChanged;
                    spot.EnabledChanged -= HandleSpotEnabledChanged;
                }
                if(value != null)
                {
                    value.OwnerChanged += HandleSpotOwnerChanged;
                    value.EnabledChanged += HandleSpotEnabledChanged;
                    spot = value;
                }
            }
        }
    }
    
    /// <summary>
    /// The game this is a part of
    /// Cycle upward through transforms
    /// </summary>
    public GlobalGame Game
    {
        get
        {
            Transform grandparent = transform.parent.parent;
            return (GlobalGame)grandparent.GetComponent<GameUI>().Game;
        }
    }

    void Awake()
    {
        InstantiateEmpty();
    }

    /// <summary>
    /// Create the reference for the empty sprite
    /// </summary>
    static void InstantiateEmpty()
    {
        if (empty == null)
        {
            empty = Resources.Load<Sprite>("Sprites/empty");
        }
    }
    
    /// <summary>
    /// Called when the button attached to this is clicked
    /// Tell the spot to broadcast its message so the game hears it
    /// </summary>
    public void OnButtonClicked()
    {
        spot.HandleOnClicked();
    }

    /// <summary>
    /// Change image to reflect the new owner
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    public virtual void HandleSpotOwnerChanged(object o, SpotEventArgs e)
    {
        Player owner = ((Spot)o).Owner;
        Image image = GetComponent<Image>();
        if (owner != null)
        {
            image.sprite = owner.Sprite;
            image.color = owner.Color;            
        }
        else // no owner, clear the spot
        {
            image.sprite = empty;
            // color irrelevant as empty sprite is transparent
        }
    }

    /// <summary>
    /// Update the button and its colors to reflect the enabled state of spot
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    public virtual void HandleSpotEnabledChanged(object o, SpotEventArgs e)
    {
        button.interactable = ((Spot)o).Enabled;
    }
}
