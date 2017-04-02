using UnityEngine;
using UnityEngine.UI;

public class ResetButton : Button {

    private bool isResetting;
    /// <summary>
    /// Delay in seconds between removals
    /// </summary>
    private const float TIME = 0.1f;
    private float timer;

    private Game game;

    /// <summary>
    /// Summary
    /// </summary>
    public Game Game { get { return game; } }

    protected override void Start()
    {
        isResetting = false;
        timer = TIME;
        game = GameObject.Find("Global Board").GetComponent<Game>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Game.IsClear)
        {
            isResetting = false;
            timer = TIME;
            interactable = false;
            return;
        }

        interactable = true;

        if (isResetting)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                game.Undo();
                timer += TIME;
            }
        }
	}

    public void ToggleReset()
    {
        isResetting = !isResetting;
    }
}
