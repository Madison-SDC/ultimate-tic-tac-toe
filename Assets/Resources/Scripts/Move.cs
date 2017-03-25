public class Move
{
    private Board board;
    private BoardSpot spot;

    /// <summary>
    /// The active board at the time of this move
    /// </summary>
    public Board Board
    {
        get
        {
            return board;
        }

        set
        {
            board = value;
        }
    }

    /// <summary>
    /// The local spot this move was made on
    /// </summary>
    public BoardSpot Spot
    {
        get
        {
            return spot;
        }

        set
        {
            spot = value;
        }
    }
    
    /// <summary>
    /// A new move on board b at position s for player p
    /// </summary>
    /// <param name="b"></param>
    /// <param name="s"></param>
    /// <param name="p"></param>
    public Move(Board b, BoardSpot s)
    {
        Board = b;
        Spot = s;
    }

}