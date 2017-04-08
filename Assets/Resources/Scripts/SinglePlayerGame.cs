using UnityEngine;
using System.Collections;

public class SinglePlayerGame : Game {

    AI ai;

	// Use this for initialization
	internal override void Start () {
        base.Start();
        ai = new RandomAI(Board.P2, Color.blue, Resources.Load<Sprite>("Sprites/o"));
	}

    public override void Play(BoardSpot spot, bool undo = false, Board prevActiveBoard = null, bool redo = false)
    {
        base.Play(spot, undo, prevActiveBoard, redo);
        if(!FirstTurn) { Play(ai.BestMove(this), undo, prevActiveBoard, redo); }
    }
}
