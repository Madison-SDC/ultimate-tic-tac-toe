public class Instruction
{
    /// <summary>
    /// Called upon Update()
    /// </summary>
    public delegate void Action();

    string info;
    Action act, advanceIn, backIn;

    public string Info { get { return info; } }
    public Action Act { get { return act; } }
    public Action AdvanceIn { get { return advanceIn; } }
    public Action BackIn { get { return backIn; } }

    /// <summary>
    /// Creates a new instruction
    /// </summary>
    /// <param name="info"></param>
    /// <param name="action"></param>
    /// <param name="advanceIn"></param>
    /// <param name="advanceOut"></param>
    /// <param name="backIn"></param>
    /// <param name="backOut"></param>
    public Instruction(
        string info,
        Action action,
        Action advanceIn,
        Action backIn
    )
    {
        this.info = info;
        act = action;
        this.advanceIn = advanceIn;
        this.backIn = backIn;
    }
}
