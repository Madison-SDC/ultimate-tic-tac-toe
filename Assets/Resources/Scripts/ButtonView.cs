using UnityEngine;
using UnityEngine.UI;

public class ButtonView : MonoBehaviour
{
	public void OnValueChanged(object o, BoolEventArgs e)
    {
        GetComponent<Button>().interactable = e.Value;
    }
}
