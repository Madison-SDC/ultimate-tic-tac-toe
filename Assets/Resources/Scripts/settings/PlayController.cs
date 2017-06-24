using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayController : MonoBehaviour
{
    public InputField p1Name, p2Name;
    public Toggle p1AIToggle, p1HumanToggle,
        p2AIToggle, p2HumanToggle;
    public Slider p1Difficulty, p2Difficulty;

    private void Awake()
    {
        if (p1Name != null)
        {
            UpdateViews();
        }
    }

    /// <summary>
    /// Update the views to match the current settings
    /// </summary>
    void UpdateViews()
    {
        p1Name.text = Settings.p1.Name;
        p2Name.text = Settings.p2.Name;

        bool p1AI = Settings.p1 is AI;
        bool p2AI = Settings.p2 is AI;

        p1AIToggle.isOn = p1AI;
        p1HumanToggle.isOn = !p1AI;
        
        p2AIToggle.isOn = p2AI;
        p2HumanToggle.isOn = !p2AI;

        p1Difficulty.value = Settings.p1 is MonteCarloAI ? 
            ((MonteCarloAI)Settings.p1).Time : 0;

        p2Difficulty.value = Settings.p2 is MonteCarloAI ? 
            ((MonteCarloAI)Settings.p2).Time : 0;
    }

    public void PlayGame()
    {
        CreatePlayers();
        OpenScene("Game");
    }

    void CreatePlayers()
    {
        Settings.p1 = CreatePlayer(p1Name, p1AIToggle, p1Difficulty, true);
        Settings.p2 = CreatePlayer(p2Name, p2AIToggle, p2Difficulty, false);
    }

    Player CreatePlayer(
        InputField nameField,
        Toggle aiToggle,
        Slider diff,
        bool first)
    {
        int turn = first ? 1 : 2;
        Color color = first ? Color.cyan : Color.magenta;
        Sprite sprite = Resources.Load<Sprite>("Sprites/" + (first ? "x" : "o"));
        string name = NameFrom(nameField.text, first);
        float time = diff.value;

        if (aiToggle.isOn)
        {
            if(time == 0)
            {
                return new RandomAI(null, turn, color, sprite, name);
            }
            return new MonteCarloAI(null, turn, color, sprite, name, time);
        }
        else
        {
            return new Player(turn, color, sprite, name);
        }
    }

    string NameFrom(string name, bool first)
    {
        if(name.Equals(""))
        {
            return first ? "X" : "O";
        }
        return name;
    }

    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
