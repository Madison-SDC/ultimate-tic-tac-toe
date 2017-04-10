using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour {

    public int gameMode;

	public void OnClick()
    {
        GameStart.GameMode = gameMode;
        SceneManager.LoadScene(1); // game scene
    }
}
