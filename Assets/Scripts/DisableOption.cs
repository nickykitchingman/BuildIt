using UnityEngine;
using UnityEngine.UI;

public class DisableOption : MonoBehaviour
{
    public string NullWord;

    private Text item;
    private string text;

    void Start()
    {
        item = GetComponentInChildren<Text>();
        if (item)
        {
            text = item.text;
            if (text == NullWord)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
