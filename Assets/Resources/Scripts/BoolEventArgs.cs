public class BoolEventArgs : System.EventArgs
{
    bool value;
    public bool Value { get { return value; } }
    public BoolEventArgs(bool value) { this.value = value; }
}
