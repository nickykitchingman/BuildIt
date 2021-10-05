using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;

public class Conditions : MonoBehaviour
{
    public Notification notification;
    [Header("Inspector")]
    public Inspector inspector;
    [Header("Edit Condition")]
    public TMP_Dropdown TriggerDropdown;
    public TMP_Dropdown CollisionTypeDropdown;
    public TMP_Dropdown ColliderDropdown;
    public TextMeshProUGUI TriggerNameText;
    public TextMeshProUGUI ColliderText;
    [Header("All Conditions")]
    public Transform AllConditionsContent;
    public Button ConditionPrefab;
    [Header("Scripts")]

    private CurrentAssets currentAssets;

    private List<string> TriggerNames;
    private List<string> ElementNames;

    void Awake()
    {
        TriggerNames = new List<string>();
        ElementNames = new List<string>();
    }

    void Start()
    {
        //Add all elements to element dictionary
        FieldInfo[] ListOfElements = typeof(Elements).GetFields();
        foreach (FieldInfo property in ListOfElements)
        {
            ElementNames.Add(property.Name);
        }

        currentAssets = SceneAssets.currentAssets;

        //Change collider list when collider type changed
        CollisionTypeDropdown.onValueChanged.AddListener(ChangeColliderType);

        //Add elements to dropdown
        LoadElementsIntoDropdown();
    }

    public void OpenConditionsMenu()
    {        
        inspector.Activate(true);
        inspector.OpenMenu("All Conditions");
        LoadConditionsIntoPanel();
        LoadTriggersIntoDropdown();
    }


    public void SaveCondition()
    {
        //Create new condition from dropdown
        Condition NewCondition = new Condition();
        try
        {
            int index;
            int i = 0;
            for (index = 0; index < currentAssets.TriggersInScene.Count; index++)
            {
                if (i == TriggerDropdown.value)
                    break;
                if (currentAssets.TriggersInScene[index].ThisPlane.activeSelf)
                    i++;
            }

            NewCondition.trigger = currentAssets.TriggersInScene[index];
            NewCondition.collisionType = CollisionTypeDropdown.options[CollisionTypeDropdown.value].text;
            NewCondition.collider = ColliderDropdown.options[ColliderDropdown.value].text;
        }
        catch (System.ArgumentOutOfRangeException)
        {
            //Notify user if a field is blank
            StartCoroutine(inspector.Notification("All fields must be filled"));
            return;
        }

        if (CheckConditionExists(NewCondition))
        {
            //Notify user if condition already exists
            StartCoroutine(inspector.Notification("Condition already exists"));
            return;
        }

        //Save condition to currentassets
        currentAssets.LevelConditions.Add(NewCondition);

        //Add new condition to inspector
        AddConditionToPanel(NewCondition);

        //Confirm to user
        notification.Notify("Condition Saved", Color.green);

        //Return to all conditions menu
        inspector.Back();
    }

    public void UpdateName()
    {
        //Change text holding currently selected trigger
        int value = TriggerDropdown.value;
        TriggerNameText.text = TriggerDropdown.options[value].text;
    }

    public void LoadTriggersIntoDropdown()
    {
        TriggerNames.Clear();
        //Load trigger names form current triggers in scene to TriggerNames list
        foreach (Trigger trigger in currentAssets.TriggersInScene)
        {
            if (trigger.ThisPlane.activeSelf)
                TriggerNames.Add(trigger.Name);
        }

        //Add names to TriggerDropdown
        TriggerDropdown.ClearOptions();
        TriggerDropdown.AddOptions(TriggerNames);
    }


    public void ChangeColliderType(int value)
    {
        switch (value)
        {
            case 0:
                LoadElementsIntoDropdown();
                ColliderText.text = "Element";
                break;
            case 1:
                LoadTagsIntoDropdown();
                ColliderText.text = "Tag";
                break;
        }
    }

    public void DeleteCondition(Condition condition, GameObject conditionbtn)
    {
        int index = FindCondition(condition);
        currentAssets.LevelConditions.RemoveAt(index);
        Destroy(conditionbtn);
    }

    public void LoadConditionsIntoPanel()
    {
        ClearPanel();
        foreach (Condition condition in currentAssets.LevelConditions)
            AddConditionToPanel(condition);
    }


    private void LoadElementsIntoDropdown()
    {
        ColliderDropdown.ClearOptions();
        ColliderDropdown.AddOptions(ElementNames);
    }

    private void LoadTagsIntoDropdown()
    {
        ColliderDropdown.ClearOptions();
        ColliderDropdown.AddOptions(currentAssets.Tags);
    }

    private bool CheckConditionExists(Condition condition)
    {
        //Check if condition is in current assets
        if (FindCondition(condition) == -1)
            return false;
        return true;
    }

    private void ClearPanel()
    {
        //Destroy each child of panel content but add new button
        foreach (Transform conditionButton in AllConditionsContent)
            if (conditionButton.name != "NewCondition")
                Destroy(conditionButton.gameObject);
    }

    private void AddConditionToPanel(Condition condition)
    {
        //Create new condition button
        Button NewConditionButton = Instantiate(ConditionPrefab);
        //Add to panel
        NewConditionButton.transform.SetParent(AllConditionsContent);
        NewConditionButton.transform.localScale = AllConditionsContent.parent.localScale;
        //Update fields
        TextMeshProUGUI[] texts = NewConditionButton.GetComponentsInChildren<TextMeshProUGUI>();
        texts[1].text = condition.trigger.Name;
        texts[3].text = condition.collisionType;
        texts[5].text = condition.collider;
        if (condition.objective != null)
            NewConditionButton.GetComponentInChildren<TMP_InputField>().text = condition.objective;

        //Remove condition with delete button
        NewConditionButton.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
        {
            DeleteCondition(condition, NewConditionButton.gameObject);
        });

        //Save objective with save button
        TMP_InputField objectiveField = NewConditionButton.GetComponentInChildren<TMP_InputField>();
        NewConditionButton.GetComponentsInChildren<Button>()[3].onClick.AddListener(() =>
        {
            SaveObjective(condition, objectiveField);
        });

        //Set to condition initially
        NewConditionButton.GetComponent<ConditionButton>().OpenCondition();
    }

    public void SaveObjective(Condition condition, TMP_InputField ObjectiveField)
    {
        //Update objective of condition in current assets
        condition.objective = ObjectiveField.text;
        currentAssets.LevelConditions[FindCondition(condition)] = condition;
        //Confirm to user
        notification.Notify("Objective Saved", Color.green);
    }

    private int FindCondition(Condition condition)
    {
        //Check all properties match but objective
        foreach (Condition item in currentAssets.LevelConditions)
            if (item.collider == condition.collider)
                if (item.collisionType == condition.collisionType)
                    if (item.trigger == condition.trigger)
                        return currentAssets.LevelConditions.IndexOf(item);
        return -1;
    }
}