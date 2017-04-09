using UnityEngine;
using UnityEngine.UI;

public class RedoButton : Button {
	
	// Update is called once per frame
	void Update ()
    {
        interactable = game.CanRedo();
	}
}
