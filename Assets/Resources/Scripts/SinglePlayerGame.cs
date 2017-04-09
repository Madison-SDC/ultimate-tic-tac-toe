using UnityEngine;
using System.Collections;

public class SinglePlayerGame : Game {

    /// <summary>
    /// The AI player in this game
    /// </summary>
    AI ai;

    /// <summary>
    /// Time remaining until the AI previews its move (in ms)
    /// </summary>
    private float previewTime;

    /// <summary>
    /// How long the AI takes to preview its move (in ms)
    /// </summary>
    private float previewTimer;

    /// <summary>
    /// Time remaining until AI confirms the previewed move (in ms)
    /// </summary>
    private float confirmTime;

    /// <summary>
    /// How long the AI takes to confirm its move (in ms)
    /// </summary>
    private float confirmTimer;

	// Use this for initialization
	internal override void Start () {
        base.Start();
        ai = new RandomAI(Board.P2, Color.blue, Resources.Load<Sprite>("Sprites/o"));
        p2 = ai; // for active player reference
        previewTimer = 0.5f;
        confirmTimer = 1f;
	}

    /// <summary>
    /// If AI's turn, preview or confirm move
    /// </summary>
    internal override void Update()
    {
        base.Update();
        if (!resetting)
        {
            if (ActivePlayer == ai)
            {
                if (HasNextMove)
                {
                    if (confirmTime <= 0)
                    {
                        // play AI move
                        Confirm();
                    }
                    else
                    {
                        confirmTime -= Time.deltaTime;
                    }
                }
                else
                {
                    if (previewTime <= 0)
                    {
                        // preview AI move
                        UpdateDisplay(ai.BestMove(this));
                    }
                    else
                    {
                        previewTime -= Time.deltaTime;
                    }
                }
            }
            else
            {
                // not AI turn, reset timers
                confirmTime = confirmTimer;
                previewTime = previewTimer;
            }
        }
    }

    /// <summary>
    /// To undo the most recent move, do a generic undo twice.
    /// This removes both the player's move and the AI's move
    /// </summary>
    public override void Undo()
    {
        bool undoTwice = !HasNextMove; // track, may change
        base.Undo();
        if (undoTwice) { base.Undo(); }
    }

    /// <summary>
    /// To redo the most recent move, do a generic redo twice.
    /// This redoes both the player's move and the AI's move
    /// </summary>
    public override void Redo()
    {
        base.Redo();
        base.Redo();
    }
}
