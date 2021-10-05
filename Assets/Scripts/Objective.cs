using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective : MonoBehaviour
{
    public TextMeshProUGUI ObjectiveText;
    public Image Check;
    [Header("Sprites")]
    public Sprite Complete;
    public Sprite Incomplete;

    public Condition condition { get; set; }
    public int conditionIndex { get; set; }

    private string _text;
    public string Text
    {
        get {
            return _text;
        }
        set {
            _text = value;
            ObjectiveText.text = value;
        }
    }

    public void SetCondition(Condition newCondition)
    {
        //Set index of condition and update graphic
        conditionIndex = SceneAssets.currentAssets.LevelConditions.IndexOf(newCondition);
        UpdateObjective();
    }

    public void UpdateObjective()
    {
        //Update condition from current assets
        condition = SceneAssets.currentAssets.LevelConditions[conditionIndex];
        //Update check from the condition
        CheckComplete(condition.complete);
        //Update objective text
        Text = condition.objective;
    }

    private void CheckComplete(bool value)
    {
        //Set graphic to incomplete or complete
        if (value)
            Check.sprite = Complete;
        else
            Check.sprite = Incomplete;
    }
}
