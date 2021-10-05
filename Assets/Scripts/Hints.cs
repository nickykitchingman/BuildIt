using UnityEngine;
using UnityEngine.UI;

public class Hints : MonoBehaviour
{
    public Image Background;
    public GameObject Mode;
    public GameObject Move;
    public GameObject Select;
    public GameObject Cancel;
    public GameObject Actions;
    public GameObject LookInteract;
    public GameObject LookMoving;

    private bool IsRendering;

    private void Start()
    {
        ResetAll();
        if (GameData.IsMoving)
            ToMovingMode();
        else
            ToInteractMode();
    }

    private void ResetAll()
    {
        Mode.SetActive(false);
        Move.SetActive(false);
        Select.SetActive(false);
        Cancel.SetActive(false);
        Actions.SetActive(false);
        LookInteract.SetActive(false);
        LookMoving.SetActive(false);
    }

    private void ToInteractMode()
    {
        Mode.SetActive(true);
        Move.SetActive(false);
        //Cancel.SetActive(true);
        //Actions.SetActive(true);
        LookInteract.SetActive(true);
        LookMoving.SetActive(false);
        
        if (!IsRendering)
            Select.SetActive(true);
    }

    private void ToMovingMode()
    {
        Mode.SetActive(true);
        Move.SetActive(true);
        //Cancel.SetActive(false);
        //Actions.SetActive(false);
        LookInteract.SetActive(false);
        LookMoving.SetActive(true);

        if (!IsRendering)
            Select.SetActive(false);
    }
    
    private void ToRenderMode(bool value)
    {
        IsRendering = value;
        //Mode.SetActive(true);
        //Move.SetActive(true);

        Select.SetActive(!value);
        Cancel.SetActive(value);
        Actions.SetActive(value);
        //LookInteract.SetActive(false);
        //LookMoving.SetActive(true);
    }


    /// <summary>
    /// Change hints on mode change
    /// </summary>
    /// <param name="mode"></param>
    public void HandleModeChange(string mode)
    {
        switch (mode.ToLower())
        {
            case "interact":
                ToInteractMode();
                break;
            case "moving":
                ToMovingMode();
                break;
        }
    }

    public void HandleRenderChange(bool value)
    {
        ToRenderMode(value);
    }

    public void Activate(bool value)
    {
        Background.gameObject.SetActive(value);
    }
}
