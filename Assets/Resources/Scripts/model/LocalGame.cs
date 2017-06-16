using UnityEngine;
using System;

public class LocalGame : Game
{
    private Location loc;
    Spot[,] spots;
    Color outline;

    public Location Loc { get { return loc; } }
    public Spot[,] Spots { get { return spots; } }

    public Color Outline
    {
        get { return outline; }
        internal set
        {
            if(outline != value)
            {
                outline = value;
                RaiseOutlineChanged(GetArgs());
            }
        }
    }

    public event EventHandler<GameEventArgs> OutlineChanged;
    protected virtual void RaiseOutlineChanged(GameEventArgs e)
    {
        if (OutlineChanged != null) { OutlineChanged(this, e); }
    }

    public LocalGame(Spot[,] spots, bool enabled, Location loc) 
        : base(enabled)
    {
        this.loc = loc;
        this.spots = spots;
        Outline = Color.clear;
        PopulateOwnerArray(spots);
        CheckWinner();
    }

    public Spot Get(Location loc)
    {
        return spots[loc.Row, loc.Col];
    }

    void PopulateOwnerArray(Spot[,] spots)
    {
        ownerArray = new Player[spots.GetLength(0), spots.GetLength(1)];
        foreach (Spot spot in spots)
        {
            spot.OwnerChanged += HandleSpotOwnerChanged;
            HandleSpotOwnerChanged(spot, null); // populate array
            spot.LocalGame = this;
        }
    }

    void HandleSpotOwnerChanged(object o, SpotEventArgs e)
    {
        Spot spot = (Spot)o;
        UpdateOwnerArray(spot.Loc, spot.Owner);
    }
}
