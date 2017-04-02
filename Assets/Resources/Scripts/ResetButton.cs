using UnityEngine;
using UnityEngine.UI;

public class ResetButton : Button {

    private Game game;

    protected override void Start()
    {
        game = GameObject.Find("Global Board").GetComponent<Game>();
    }

    // Update is called once per frame
    void Update ()
    {
        interactable = !game.IsClear;
	}
}
