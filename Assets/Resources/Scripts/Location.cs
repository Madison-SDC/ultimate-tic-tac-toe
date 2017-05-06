public class Location {

    int row, col;

    public int Row { get { return row; } }

    public int Col { get { return col; } }

	public Location(int r, int c)
    {
        row = r;
        col = c;
    }

    public override bool Equals(object obj)
    {
        if(obj is Location)
        {
            Location other = (Location)obj;
            return Row == other.Row
                && Col == other.Col;
        }
        return false;
    }
}
