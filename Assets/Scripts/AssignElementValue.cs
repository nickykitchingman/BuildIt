using UnityEngine;

public class AssignElementValue : MonoBehaviour
{
    public PlayTutorial tutorialScript;

    private DropdownAction[] dropdownComponents;
    private TMPDropdownAction[] tmpdropdownComponents;

    private void OnEnable()
    {
        dropdownComponents = GetComponents<DropdownAction>();
        tmpdropdownComponents = GetComponents<TMPDropdownAction>();

        foreach (DropdownAction component in dropdownComponents)
            component.Value = tutorialScript.GetElement();
        foreach (TMPDropdownAction component in tmpdropdownComponents)
            component.Value = tutorialScript.GetElement();
    }
}
