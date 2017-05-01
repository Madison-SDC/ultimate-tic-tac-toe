using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Status : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        Game game = Game.CurrentGame;
        if(game is InstructionGame) { return; } // Instructions take care of their own status info
        Move previousMove = game.PreviousMove;
        Spot spot = previousMove.Spot;

        if (spot != null) // a previous move has been made
        {
            // The previous player to move
            Player previousPlayer = game.ActivePlayer == game.P1 ? game.P2 : game.P1;

            SetText(
                Name(previousPlayer) + " went in the " +
                LocToName(spot.Loc) + " spot of the " +
                LocToName(spot.ParentBoard.Loc) + " board. "
            );
        }
        else { SetText(""); }
        
        if(game.GameOver && !game.HasNextMove)
        {
            Player winner = game.Owner;
            if (winner != null)
            {
                string name = Name(winner);
                if (name.Equals("You")) { AppendText(name + " win!"); }
                else { AppendText(name + " wins!"); }
                }
            else { AppendText("Tie game."); }
            return;
        }

        if (game is SinglePlayerGame)
        {
            if (!(game.ActivePlayer is AI)) // Human's turn
            {
                AppendText("Your turn.");
            }
            else { AppendText("AI's turn."); }
        }
        else
        {
            AppendText(Name(game.ActivePlayer) + "'s turn");
        }
    }

    string LocToName(Location loc)
    {
        if(loc == null) { return null; }
        if(loc.Row == 1 && loc.Col == 1) { return "center"; }

        string name = "";

        switch(loc.Row)
        {
            case (0):
                name += "top";
                break;
            case (1):
                name += "middle";
                break;
            case (2):
                name += "bottom";
                break;
            default:
                return null;
        }

        name += "-";

        switch(loc.Col)
        {
            case (0):
                name += "left";
                break;
            case (1):
                name += "middle";
                break;
            case (2):
                name += "right";
                break;
            default:
                return null;
        }

        return name;
    }

    string Name(Player p)
    {
        if(p is AI) { return "AI"; }
        bool onePlayer = Game.CurrentGame is SinglePlayerGame;
        if(p.Turn == 1) { return onePlayer ? "You" : "X"; }
        if(p.Turn == 2) { return onePlayer ? "You" : "O"; }
        return null;
    }

    /// <summary>
    /// Changes the text of the text component to be <paramref name="str"/>
    /// </summary>
    /// <param name="str"></param>
    void SetText(string str)
    {
        GetComponent<Text>().text = str;
    }

    /// <summary>
    /// Appends <paramref name="str"/> to the text of the text component
    /// </summary>
    /// <param name="str"></param>
    void AppendText(string str)
    {
        GetComponent<Text>().text += str;
    }
}
