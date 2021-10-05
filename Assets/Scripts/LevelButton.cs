using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public Button Name;
    public Menu MainMenu;

    public string LevelName { get; private set; }

    public void UpdateName(string name)
    {
        LevelName = name;
        Name.transform.GetComponentInChildren<TextMeshProUGUI>().text = name;
    }
}
