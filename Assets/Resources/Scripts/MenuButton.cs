using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : Button {
    public void OnClick()
    {
        SceneManager.LoadScene(0); // Menu scene
    }
}
