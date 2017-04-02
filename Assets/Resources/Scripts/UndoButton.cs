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
        if (game)
        {
            interactable = !game.IsClear || game.HasNextMove;
        } else { interactable = false; }
	}
}
