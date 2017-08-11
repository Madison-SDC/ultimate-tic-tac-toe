public class Instruction
{
    /// <summary>
    /// Called upon Update()
    /// </summary>
    public delegate void Action();

    string info;
    Action act, advanceIn, advanceOut, backIn, backOut;
    float time, timer;
    GlobalGame game;

    public string Info { get { return info; } }
    public Action Act { get { return act; } }
    public Action AdvanceIn { get { return advanceIn; } }
    public Action AdvanceOut { get { return advanceOut; } }
    public Action BackIn { get { return backIn; } }
    public Action BackOut { get { return backOut; } }

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
        GlobalGame game,
        string info, 
        Action action, 
        Action advanceIn, 
        Action advanceOut, 
        Action backIn, 
        Action backOut
    )
    {
        this.game = game;
        this.info = info;
        act = action;
        this.advanceIn = advanceIn;
        this.advanceOut = advanceOut;
        this.backIn = backIn;
        this.backOut = backOut;
    }
}
