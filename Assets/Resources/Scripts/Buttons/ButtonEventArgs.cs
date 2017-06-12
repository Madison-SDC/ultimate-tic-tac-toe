public class ButtonEventArgs : System.EventArgs
{
    public bool value;

	public ButtonEventArgs(bool value)
    {
        this.value = value;
    }
}
