/// <summary>
/// Contains information about a board of spots
/// </summary>
public class LocalBoard : Board
{
    Location loc;
    Spot[,] spots;

    public Location Loc { get { return loc; } }
    public Spot[,] Spots { get { return spots; } }

    public LocalBoard(Location loc, Spot[,] spots)
    {
        this.loc = loc;
        this.spots = spots;

        foreach(Spot spot in spots)
        {
            spot.OwnerChanged += HandleSpotOwnerChanged;
        }
    }

    protected void HandleSpotOwnerChanged(object o, SpotEventArgs e)
    {
        Spot spot = (Spot)o;
        UpdateOwnerArray(spot.Loc, spot.Owner);
    }
}
