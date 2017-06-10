public class LocalGame : Game
{
    private Location loc;
    Spot[,] spots;

    public Location Loc { get { return loc; } }
    public Spot[,] Spots { get { return spots; } }

	public LocalGame(Spot[,] spots, bool enabled, Location loc) 
        : base(enabled)
    {
        this.loc = loc;
        this.spots = spots;
        PopulateOwnerArray(spots);
        UpdateState();
    }

    void PopulateOwnerArray(Spot[,] spots)
    {
        ownerArray = new Player[spots.GetLength(0), spots.GetLength(1)];
        foreach (Spot spot in spots)
        {
            spot.OwnerChanged += HandleSpotOwnerChanged;
            HandleSpotOwnerChanged(spot, null); // populate array
        }
    }

    void HandleSpotOwnerChanged(object o, SpotEventArgs e)
    {
        Spot spot = (Spot)o;
        UpdateOwnerArray(spot.Loc, spot.Owner);
    }
}
