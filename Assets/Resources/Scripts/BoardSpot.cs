using UnityEngine;
using UnityEngine.UI;

public class BoardSpot : MonoBehaviour {
    /// <summary>
    /// Whether this spot has been clicked
    /// </summary>
    private bool clicked;

    public bool Clicked { get { return clicked; } set { clicked = value; } }

    public void Start()
    {
        Clicked = false;
    }

    public void OnClick()
    {
        transform.parent.parent.GetComponent<Game>().FillSpot(gameObject);
    }

    public void Reset()
    {
        Clicked = false;
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/empty");
        GetComponent<Image>().color = Color.white;

        ColorBlock cb = GetComponent<Button>().colors;
        cb.disabledColor = Color.gray;
        GetComponent<Button>().colors = cb;
    }
}
