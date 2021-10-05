using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

[System.Serializable] public class UnityEventString : UnityEvent<string> { }

public class SelectName : MonoBehaviour
{
    public TMP_InputField NameInputField;
    [Header("Name Entered")]
    public UnityEventString OnConfirm;
    [Header("Scripts")]
    public PlayerControls playerControls;

    void OnEnable()
    {
        if (NameInputField != null)
        {
            NameInputField.Select();
        }

        GameData.ControlsEnabled = false;
    }

    void Update()
    {
        //Confirm
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnConfirm.Invoke(NameInputField.text);
        }

        //Cancel
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            NameInputField.text = "";
            gameObject.SetActive(false);
            GameData.ControlsEnabled = true;
        }
    }
}
