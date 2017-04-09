using UnityEngine.UI;
using UnityEngine;

public class UndoButton : Button {

    Game game;

    protected override void Start()
    {
        game = GameObject.Find("Global Board").GetComponent<Game>();
    }

    // Update is called once per frame
    void Update () {
        interactable = game.CanUndo();
	}
}
