using UnityEngine.UI;

public class ButtonAction : UserAction
{
    public Button button;
    public bool ChangeState;

    private bool interactState;

    private void OnEnable()
    {
        interactState = button.interactable;
        button.interactable = true;
    }

    private void Start()
    {
        button.onClick.AddListener(SetComplete);
    }

    private void OnDisable()
    {
        if (!ChangeState)
            button.interactable = interactState;
    }

    private void SetComplete()
    {
        Complete = true;
    }
}
