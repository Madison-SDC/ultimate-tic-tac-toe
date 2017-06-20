using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayController : MonoBehaviour
{
    public InputField p1Name, p2Name;
    public ToggleGroup p1Toggle, p2Toggle;
    public Slider p1Difficulty, p2Difficulty;

    public void PlayGame()
    {
        CreatePlayers();
        OpenScene("Game");
    }

    void CreatePlayers()
    {
        Settings.p1 = CreatePlayer(p1Name, p1Toggle, p1Difficulty, true);
        Settings.p2 = CreatePlayer(p2Name, p2Toggle, p2Difficulty, false);
    }

    Player CreatePlayer(
        InputField nameField,
        ToggleGroup toggle,
        Slider diff,
        bool first)
    {
        int turn = first ? 1 : 2;
        Color color = first ? Color.red : Color.blue;
        Sprite sprite = Resources.Load<Sprite>("Sprites/" + (first ? "x" : "o"));
        string name = nameField.text;
        float time = diff.value;

        bool isAI = ActiveToggleTag(toggle).Equals("AI");

        if (isAI)
        {
            if (time > 0)
            {
                return new MonteCarloAI(null, turn, color, sprite, name, time);
            }
            else
            {
                return new RandomAI(null, turn, color, sprite, name);
            }
        }
        else
        {
            return new Player(turn, color, sprite, name);
        }
    }

    string ActiveToggleTag(ToggleGroup group)
    {
        IEnumerator<Toggle> enumerator = group.ActiveToggles().GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current.tag;
    }

    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
