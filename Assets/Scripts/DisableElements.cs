using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableElements : MonoBehaviour
{
    public List<Selectable> Elements;
    public bool Disable;
    public bool RetainState;

    private List<bool> states;

    private void OnEnable()
    {
        states = new List<bool>();

        for (int i = 0; i < Elements.Count; i++)
        {
            states.Add(Elements[i].interactable);
            Elements[i].interactable = !Disable;
        }
    }

    private void OnDisable()
    {
        if (RetainState)
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].interactable = states[i];
            }
    }
}
