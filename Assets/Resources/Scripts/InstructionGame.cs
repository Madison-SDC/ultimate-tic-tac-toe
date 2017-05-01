using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// A guided game that provides players with knowledge of the game's rules
/// </summary>
public class InstructionGame : Game {

    Text text;

    /// <summary>
    /// Whether the next button is valid
    /// </summary>
    /// <returns>Whether the next button should be enabled</returns>
    delegate bool CanContinue();

    List<CanContinue> conditions;
    
	// Use this for initialization
	void Start () {
        base.Start();
        text = GameObject.Find("Status Text").GetComponent<Text>();
        InstantiateConditions();
	}

    void InstantiateConditions()
    {
        conditions = new List<CanContinue>();

        conditions.Add(delegate () { return history.Count == 2; });
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Sets the text of the the Text
    /// </summary>
    /// <param name="str"></param>
    void SetText(string str)
    {
        text.text = str;
    }

    public override bool CanConfirm()
    {
        if (base.CanConfirm())
        {
            if (history.Count == 1) // first move
            {
                // first move must be top left spot of center board
                return nextMove.Loc.Equals(new Location(0, 0))
                    && nextMove.ParentBoard.Loc.Equals(new Location(1, 1));
            }
            return true;
        }
        return false;
    }
}
