using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;

public class TMPDropdownAction : UserAction
{
    public TMP_Dropdown dropdown;
    public bool UseValue;
    public string Value;
    public bool ChangeState;

    private bool interactState;
    private bool found = false;

    private void OnEnable()
    {
        interactState = dropdown.interactable;
        foreach (Selectable child in dropdown.GetComponentsInChildren<Selectable>(true))
            child.interactable = true;
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(SetComplete);
    }

    private void Update()
    {
        if (UseValue && !found)
        {
            //Colour option
            //Find items
            Toggle[] options = dropdown.GetComponentsInChildren<Toggle>(true);
            options = options.Where(f => f.name.Contains(Value)).ToArray();

            if (options.Length > 0)
            {
                //Find image of item
                Image image = options[0].GetComponentInChildren<Image>();
                if (image)
                {
                    //Change colour
                    image.color = Color.green;
                    found = true;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (!ChangeState)
            foreach (Selectable child in dropdown.GetComponentsInChildren<Selectable>(true))
                child.interactable = interactState;
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
