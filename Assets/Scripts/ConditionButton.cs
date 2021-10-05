using UnityEngine;
using TMPro;

public class ConditionButton : MonoBehaviour
{
    public GameObject EditObjective;
    public GameObject Fields;

    public TMP_InputField ObjectiveField;

    public void OpenObjective()
    {
        //Switch icon
        Fields.SetActive(false);
        EditObjective.SetActive(true);
    }

    public void OpenCondition()
    {
        //Switch icon
        EditObjective.SetActive(false);
        Fields.SetActive(true);
    }
}
