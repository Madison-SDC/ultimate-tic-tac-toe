using UnityEngine;
using UnityEngine.UI;

public class ResetButton : Button {

    // Update is called once per frame
    void Update ()
    {
        interactable = !Game.CurrentGame.IsClear;
	}

    public void OnClick()
    {
        if (interactable) { Game.CurrentGame.Reset(); }
    }
}