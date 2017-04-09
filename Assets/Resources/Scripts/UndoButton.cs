using UnityEngine.UI;
using UnityEngine;

public class UndoButton : Button {

    // Update is called once per frame
    void Update ()
    {
        interactable = Game.CurrentGame.CanUndo();
	}
}
