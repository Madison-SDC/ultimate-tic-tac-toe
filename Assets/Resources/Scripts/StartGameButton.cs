using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public Color p1Color, p2Color;
    public Sprite p1Sprite, p2Sprite;
    public Text p1Name, p2Name;
    public ToggleGroup p1TypeGroup, p2TypeGroup;
    public ToggleGroup p1DiffGroup, p2DiffGroup;
    public Slider p1SkillSlider, p2SkillSlider;

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

        Player p1 = AssignPlayer(
            1,
            p1Color,
            p1Sprite,
            p1Name,
            ActiveToggleName(p1TypeGroup),
            ActiveToggleName(p1DiffGroup),
            (int)p1SkillSlider.value,
            null);

        Player p2 = AssignPlayer(
            2,
            p2Color,
            p2Sprite,
            p2Name,
            ActiveToggleName(p2TypeGroup),
            ActiveToggleName(p2DiffGroup),
            (int)p2SkillSlider.value,
            p1);

        // can only be assigned once instantiated
        if (p1 is HeuristicAI) { ((HeuristicAI)p1).Opponent = p2; }

        Settings.p1 = p1;
        Settings.p2 = p2;

        SceneManager.LoadScene("Game");
    }

    Player AssignPlayer(
        int turn, 
        Color color, 
        Sprite sprite, 
        Text nameUI, 
        string type, 
        string difficulty,
        int skill,
        Player opponent)
    {
        string name = AssignName(nameUI, turn);

        if(type.Equals("Human"))
        {
            return new Player(turn, color, sprite, name);
        }
        else
        {
            if(difficulty.Equals("Easy"))
            {
                return new RandomAI(turn, color, sprite, name);
            }
            else
            {
                return new HeuristicAI(
                    turn, // turn
                    color,
                    sprite,
                    name, // name
                    opponent, // opponent
                    skill,  // depth
                    3,  // corner weight
                    1,  // side weight
                    2,  // center weight
                    20, // local win
                    10, // local block
                    -25 // relative over
                );
            }
        }
    }

    /// <summary>
    /// Returns what a player should be named given its text object and turn
    /// </summary>
    /// <param name="textUI"></param>
    /// <param name="turn"></param>
    /// <returns></returns>
    string AssignName(Text textUI, int turn)
    {
        if(textUI.text.Equals(""))
        {
            switch(turn)
            {
                case (1):
                    return "X";
                case (2):
                    return "O";
            }
        }
        return textUI.text;
    }

    /// <summary>
    /// Returns the name of the active toggle in this group
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    string ActiveToggleName(ToggleGroup group)
    {
        IEnumerator <Toggle> enumerator = group.ActiveToggles().GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current.name;
    }
}