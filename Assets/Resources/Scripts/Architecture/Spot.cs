using System;

public class Spot
{
    Location loc;
    Board board;
    Player owner;
    bool enabled;

    public Location Loc { get { return loc; } }
    public Board Board {
        get { return board; }
        set { board = value; }
    }
    public Player Owner
    {
        get { return owner; }
        set
        {
            owner = value;
            RaiseOwnerChanged(GetArgs());
        }
    }
    public bool Enabled
    {
        get { return enabled; }
        set
        {
            enabled = value;
            RaiseEnabledChanged(GetArgs());
        }
    }

    public event EventHandler<SpotEventArgs> OwnerChanged;
    protected virtual void RaiseOwnerChanged(SpotEventArgs e)
    {
        if (OwnerChanged != null) // if there are some listeners
        {
            OwnerChanged(this, e); // notify all listeners
        }
    }

    public event EventHandler<SpotEventArgs> EnabledChanged;
    protected virtual void RaiseEnabledChanged(SpotEventArgs e)
    {
        if (EnabledChanged!= null)  { EnabledChanged(this, e); }
    }

    public event EventHandler<SpotEventArgs> Clicked;
    protected virtual void RaiseClicked(SpotEventArgs e)
    {
        if(Clicked != null) { Clicked(this, e); }
    }

    public Spot(Location loc, Board board, Player owner, bool enabled)
    {
        this.loc = loc;
        this.board = board;
        this.owner = owner;
        this.enabled = enabled;
    }

    /// <summary>
    /// When this is told that it has been clicked,
    /// it broadcasts a message to any listeners of the Clicked event
    /// </summary>
    public void HandleOnClicked()
    {
        RaiseClicked(GetArgs());
    }

    private SpotEventArgs GetArgs()
    {
        return new SpotEventArgs();
    }
}
