using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public Color p1Color, p2Color;
    public Sprite p1Sprite, p2Sprite;
    public Text p1Name, p2Name;
    public ToggleGroup p1Group, p2Group;

    /// <summary>
    /// Load the settings scene
    /// </summary>
    public void ToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    /// <summary>
    /// Start the game based on the conditions of the setup
    /// </summary>
    public void ToGame()
    {
        // p1Type is the name of the first active toggle of the p1 group
        string p1Type = ActiveToggleName(p1Group);
        Player p1;
        string p1NameString;
        if(p1Name.text.Equals("")) { p1NameString = "X"; }
        else { p1NameString = p1Name.text; }
        if(p1Type.Equals("Human Toggle"))
        {
            p1 = new Player(1, p1Color, p1Sprite, p1NameString);
        }
        else
        {
            p1 = new HeuristicAI(
            1, // turn
            p1Color,
            p1Sprite,
            p1NameString, // name
            null, // opponent
            2,  // depth
            3,  // corner weight
            1,  // side weight
            2,  // center weight
            20, // local win
            10, // local block
            -25 // relative over
            );
        }


        // p2Type is the name of the first active toggle of the p1 group
        string p2Type = ActiveToggleName(p2Group);
        Player p2;
        string p2NameString;
        if (p2Name.text.Equals("")) { p2NameString = "O"; }
        else { p2NameString = p2Name.text; }
        if (p2Type.Equals("Human Toggle"))
        {
            p2 = new Player(2, p2Color, p2Sprite, p2NameString);
        }
        else
        {
            p2 = new HeuristicAI(
            2, // turn
            p2Color,
            p2Sprite,
            p2NameString, // name
            p1, // opponent
            2,  // depth
            3,  // corner weight
            1,  // side weight
            2,  // center weight
            20, // local win
            10, // local block
            -25 // relative over
            );
        }

        // can only be assigned once instantiated
        if(p1 is HeuristicAI) { ((HeuristicAI)p1).Opponent = p2; }

        Settings.p1 = p1;
        Settings.p2 = p2;

        SceneManager.LoadScene("Game");
    }

    string ActiveToggleName(ToggleGroup group)
    {
        IEnumerator < Toggle > enumerator = group.ActiveToggles().GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current.name;
    }
}