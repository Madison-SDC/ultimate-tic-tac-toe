using UnityEngine;
using UnityEngine.UI;

public class RedoButton : Button {
	
	// Update is called once per frame
	void Update ()
    {
        interactable = Game.CurrentGame.CanRedo();
	}

    public void OnClick()
    {
        if (interactable) { Game.CurrentGame.Redo(); }
    }
}
