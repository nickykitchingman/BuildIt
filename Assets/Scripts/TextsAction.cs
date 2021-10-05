using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextsAction : UserAction
{
    public List<TMP_InputField> texts;
    [Range(0, 10)]
    public int minLength;
    public bool ChangeState;

    private List<bool> interactStates;
    private bool[] completed;

    private void OnEnable()
    {
        interactStates = new List<bool>();
        completed = new bool[texts.Count];

        //Reacord all states and disable all
        if (texts != null)
            texts.ForEach(f => 
            { 
                interactStates.Add(f.interactable);
                f.interactable = true; 
            });
    }

    private void Update()
    {
        //Check every text
        for (int i = 0; i < completed.Length; i++)
            if (texts[i].text.Length > minLength)
                completed[i] = true;

        //Check all complete
        if (completed.All(f => f == true))
            Complete = true;
    }


    private void OnDisable()
    {
        //Retain all states
        if (!ChangeState)
            for (int i = 0; i < texts.Count; i++)
                texts[i].interactable = interactStates[i];
    }
}
