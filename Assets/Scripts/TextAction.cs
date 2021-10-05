using TMPro;
using UnityEngine;

public class TextAction : UserAction
{
    public TMP_InputField text;
    [Range(0, 10)]
    public int minLength;
    public bool ChangeState;

    private bool interactState;

    private void OnEnable()
    {
        interactState = text.interactable;
        text.interactable = true;
    }

    private void Start()
    {
        text.onValueChanged.AddListener(SetComplete);
    }

    private void OnDisable()
    {
        if (!ChangeState)
            text.interactable = interactState;
    }

    private void SetComplete(string value)
    {
        if (value.Length >= minLength)
            Complete = true;
    }
}
