using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssignObjectives : MonoBehaviour
{
    public SaveLevel SaveLevelScript;
    public string ButtonsName;

    private TextsAction[] textsComponents;
    private ButtonsAction[] buttonsComponents;
    private List<TMP_InputField> texts;
    private List<Button> buttons;

    private void OnEnable()
    {
        textsComponents = GetComponents<TextsAction>();
        buttonsComponents = GetComponents<ButtonsAction>();
        getObjectives();
        AssignObjectToComponents();
    }

    private void getObjectives()
    {
        Transform content = SaveLevelScript.AllConditionsContent;
        texts = content.GetComponentsInChildren<TMP_InputField>(true).ToList();
        buttons = content.GetComponentsInChildren<Button>(true).Where(f => f.name.Contains(ButtonsName)).ToList();
    }

    private void AssignObjectToComponents()
    {
        foreach (TextsAction component in textsComponents)
            component.texts = texts;
        foreach (ButtonsAction component in buttonsComponents)
            component.buttons = buttons;
    }
}
