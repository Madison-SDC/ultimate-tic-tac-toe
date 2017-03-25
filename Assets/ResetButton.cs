using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResetButton : Button {
	
	// Update is called once per frame
	void Update () {
        interactable = !Game.IsClear;
	}
}
