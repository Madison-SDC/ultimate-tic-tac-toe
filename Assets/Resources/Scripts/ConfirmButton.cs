﻿using UnityEngine;
using UnityEngine.UI;

public class ConfirmButton : Button {
	
	// Update is called once per frame
	void Update ()
    {
        interactable = Game.CurrentGame.CanConfirm();
	}

    public void OnClick()
    {
        if(interactable) { Game.CurrentGame.Confirm(); }
    }
}
