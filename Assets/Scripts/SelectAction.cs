using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectAction : UserAction
{
    public Selectable element;
    public bool RetainState;

    private bool interactState;

    private void OnEnable()
    {
        element.interactable = true;
        interactState = element.interactable;
    }

    private void OnDisable()
    {
        if (RetainState)
            element.interactable = interactState;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == element.gameObject)
            Complete = true;
    }
}