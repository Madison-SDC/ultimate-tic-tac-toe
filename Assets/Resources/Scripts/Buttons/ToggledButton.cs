using UnityEngine.UI;

public class ToggledButton : Button
{
	void HandleCanConfirmChanged(object o, ButtonEventArgs e)
    {
        enabled = e.value;
    }
}
