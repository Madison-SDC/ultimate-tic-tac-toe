using UnityEngine.UI;

public class ConfirmButton : Button {
	
	// Update is called once per frame
	void Update () {
        interactable = Game.HasNextMove;
	}
}
