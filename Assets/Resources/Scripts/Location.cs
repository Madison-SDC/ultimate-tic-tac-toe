public class Location {

    int row, col;

    public int Row { get { return row; } }

    public int Col { get { return col; } }

    const int NORTH = 0,
        NORTH_EAST = 1,
        EAST = 2,
        SOUTH_EAST = 3,
        SOUTH = 4,
        SOUTH_WEST = 5,
        WEST = 6,
        NORTH_WEST = 7,
        STRAIGHT = 0,
        HALF_RIGHT = 1,
        RIGHT = 2,
        HALF_CIRCLE = 4,
        FULL_CIRCLE = 8;

	public Location(int r, int c)
    {
        row = r;
        col = c;
    }

    /// <summary>
    /// Returns an integer in [STRAIGHT, FULL_CIRCLE)
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static int FixDir(int d)
    {
        d %= FULL_CIRCLE;
        if(d < STRAIGHT) { d += FULL_CIRCLE; }
        return d;
    }

    /// <summary>
    /// Returns the location adjacent to this one in the specified direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Location GetAdjacent(int dir)
    {
        int dR = 0;
        int dC = 0;

        dir = FixDir(dir);
        
        switch(dir)
        {
            case (NORTH):
                dR = -1;
                dC = 0;
                break;
            case(NORTH_EAST):
                dR = -1;
                dC = 1;
                break;
            case(EAST):
                dR = 0;
                dC = 1;
                break;
            case(SOUTH_EAST):
                dR = 1;
                dC = 1;
                break;
            case(SOUTH):
                dR = 1;
                dC = 0;
                break;
            case(SOUTH_WEST):
                dR = 1;
                dC = -1;
                break;
            case(WEST):
                dR = 0;
                dC = -1;
                break;
            case (NORTH_WEST):
                dR = -1;
                dC = -1;
                break;
        }

        return new Location(row + dR, col + dC);
    }
}
