using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ButtonsAction : UserAction
{
    public List<Button> buttons;
    public bool ChangeState;

    private List<bool> interactStates;
    private bool[] completed;

    private int count = 0;

    private void OnEnable()
    {
        interactStates = new List<bool>();
        completed = new bool[buttons.Count];

        //Reacord all states and disable all
        if (buttons != null)
            buttons.ForEach(f =>
            {
                interactStates.Add(f.interactable);
                f.interactable = true;
            });

        //Add every button
        for (int i = 0; i < completed.Length; i++)
            buttons[i].onClick.AddListener(SetComplete);
    }

    private void Update()
    {

        //Check all complete
        if (completed.All(f => f == true))
            Complete = true;
    }


    private void OnDisable()
    {
        //Retain all states
        if (!ChangeState)
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].interactable = interactStates[i];
    }

    private void SetComplete()
    {
        //Count the button clicks...
        if (count < buttons.Count)
        {
            completed[count] = true;
            count++;
        }
    }
}
