using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RedoButton : Button {

    Game game;

	// Use this for initialization
	protected override void Start () {
        game = GameObject.Find("Global Board").GetComponent<Game>();
	}
	
	// Update is called once per frame
	void Update () {
        interactable = game.CanRedo();
	}
}
