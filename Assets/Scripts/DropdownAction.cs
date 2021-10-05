using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class DropdownAction : UserAction
{
    public Dropdown dropdown;
    public bool UseValue;
    public string Value;
    public bool ChangeState;

    private bool interactState;

    private void OnEnable()
    {
        interactState = dropdown.interactable;
        dropdown.interactable = true;

        if (UseValue)
        {
            //Index of value
            int num = dropdown.options.Select(f => f.text).ToList().IndexOf(Value);

            //Colour option
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.red);
            texture.Apply();
            OptionData item = new OptionData(Value, Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)));

            //Set option to new item
            dropdown.options[num] = item;
        }
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(SetComplete);
    }

    private void OnDisable()
    {
        if (!ChangeState)
            dropdown.interactable = interactState;
    }

    private void SetComplete(int value)
    {
        if (UseValue)
        {
            if (dropdown.options[dropdown.value].text == Value)
                Complete = true;
        }
        else
            Complete = true;
    }
}
