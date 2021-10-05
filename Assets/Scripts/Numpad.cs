using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Numpad : MonoBehaviour
{
    [Header("Components")]
    public Image Background;
    [Header("Rotate")]
    public Button RotateX;
    public Button RotateY;
    public Button RotateZ;
    [Header("Move")]
    public Button MoveX;
    public Button MoveY;
    public Button MoveZ;
    [Header("Scale")]
    public Button ScaleX;
    public Button ScaleY;
    public Button ScaleZ;
    public Button Scale;

    [Header("Materials")]
    public Material OriginalMat;
    public Material SelectedMat;

    private Image CurrentSelected;
    private Dictionary<string, Button> Buttons;

    void Awake()
    {
        //Add each button to dictionary
        Buttons = new Dictionary<string, Button>
        {
            { "RotateX", RotateX },
            { "RotateY", RotateY },
            { "RotateZ", RotateZ },
            { "MoveX", MoveX },
            { "MoveY", MoveY },
            { "MoveZ", MoveZ },
            { "ScaleX", ScaleX },
            { "ScaleY", ScaleY},
            { "ScaleZ", ScaleZ},
            { "Scale", Scale }
        };

        CurrentSelected = RotateX.image;
        Select(0);
    }

    public void Select(int num)
    {
        string name = Buttons.Keys.ToList()[num];
        Select(name);
    }

    public void Select(string name)
    {
        //Reset current selected color
        if (CurrentSelected != null)
            CurrentSelected.material = OriginalMat;

        //Set current selected
        CurrentSelected = Buttons[name].image;

        //Set new current selected color
        CurrentSelected.material = SelectedMat;
    }

    public void AddListeners(UnityAction<int> ChangeMode)
    {
        //Add handler in prerenderscript to buttons to change mode
        for (int i = 0; i < Buttons.Count; i++)
        {
            //Use x to stop i being passed as reference
            int x = i;
            Buttons.Values.ToList()[i].onClick.AddListener(() =>
            {
                ChangeMode(x);
            });
        }
    }

    public void Activate(bool value)
    {
        //Set active or inactive
        if (value)
        {
            LeanTween.scale(Background.gameObject, Vector3.zero, 0f);
            Background.gameObject.SetActive(true);
            LeanTween.scale(Background.gameObject, Vector3.one, 0.2f);
        }
        else
        {
            LeanTween.scale(Background.gameObject, Vector3.zero, 0.2f).setOnComplete(Disable);
        }
    }

    public void ActivateInstant(bool value)
    {
        //Set active or inactive
        if (value)
            Background.gameObject.SetActive(true);
        else
            Background.gameObject.SetActive(false);
    }

    private void Disable()
    {
        Background.gameObject.SetActive(false);
        LeanTween.scale(Background.gameObject, Vector3.one, 0f);
    }
}
