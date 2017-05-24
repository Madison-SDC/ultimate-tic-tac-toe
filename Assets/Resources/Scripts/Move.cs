public class Move
{
    private Board board;
    private SpotUI spot;

    /// <summary>
    /// The active board at the time of this move<para></para>
    /// Note: this is not necessarily the board on which the move was made 
    /// <para>(if the active board was full or null)</para>
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
    public SpotUI Spot
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
    public Move(Board b, SpotUI s)
    {
        Board = b;
        Spot = s;
    }

}