using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLevel : MonoBehaviour
{
    public Transform Player;
    public TMP_InputField NameField;
    public Notification notification;
    [Header("Menu Switching")]
    public GameObject SaveMenu;
    public GameObject InGameElements;
    [Header("Conditions")]
    public Button ConditionPrefab;
    public Transform AllConditionsContent;
    [Header("Scripts")]
    public PlayerControls playerControls;

    private CurrentAssets currentAssets;
    private SceneData Level;

    void Start()
    {
        //Add reference to the current assets
        currentAssets = SceneAssets.currentAssets;
    }

    public void Save()
    {
        //Check level name does not already exist
        if (LevelNameExists(GameData.LevelName))
        {
            notification.Notify("Level name already exists!", Color.red);
            NameField.text = GameData.LevelName;
            return;
        }

        //Check for missing name
        if (GameData.LevelName == null || GameData.LevelName.Length == 0)
        {
            notification.Notify("Level must have a name", Color.red);
            return;
        }

        //Check there is at least one level condition
        if (currentAssets.LevelConditions.Count == 0)
        {
            notification.Notify("Must be at least one level condition to save", Color.red);
            return;
        }

        //Check objectives are not null
        foreach (Condition condition in currentAssets.LevelConditions)
            if (condition.objective == null)
            {
                notification.Notify("All objectives must be filled to save", Color.red); 
                return;
            }

        //Check if no conditions made
        if (currentAssets.LevelConditions.Count == 0)
        {
            notification.Notify("Level must have at least one condition", Color.red);
            return;
        }


        //Save level
        FileHandler.SaveLevel(currentAssets, Player);

        //Notify confirmation
        notification.Notify("Level saved", Color.green);
    }

    public void OpenSaveMenu()
    {
        //Change menu
        InGameElements.SetActive(false);
        SaveMenu.SetActive(true);

        //Disable movement
        GameData.ControlsEnabled = false;

        //Load conditions into view
        ClearPanel();
        foreach (Condition condition in currentAssets.LevelConditions)
            AddConditionToPanel(condition);

        //Set level name field
        if (GameData.LevelName != null)
            NameField.text = GameData.LevelName;
    }

    public void CloseSaveMenu()
    {
        //Change menu
        SaveMenu.SetActive(false);
        InGameElements.SetActive(true);

        //Enable movement
        GameData.ControlsEnabled = true;
    }

    private void ClearPanel()
    {
        //Destroy each child of panel content
        foreach (Transform conditionButton in AllConditionsContent)
            Destroy(conditionButton.gameObject);
    }

    private void AddConditionToPanel(Condition condition)
    {
        //Create new condition button
        Button NewConditionButton = Instantiate(ConditionPrefab);        
        //Update fields
        TextMeshProUGUI[] texts = NewConditionButton.GetComponentsInChildren<TextMeshProUGUI>();
        texts[1].text = condition.trigger.Name;
        texts[3].text = condition.collisionType;
        texts[5].text = condition.collider;
        if (condition.objective != null)
            NewConditionButton.GetComponentInChildren<TMP_InputField>().text = condition.objective;
        //Add condition to panel
        NewConditionButton.transform.SetParent(AllConditionsContent, false);

        //Save objective with save button
        TMP_InputField objectiveField = NewConditionButton.GetComponentInChildren<TMP_InputField>();
        NewConditionButton.GetComponentsInChildren<Button>()[2].onClick.AddListener(() =>
        {
            SaveObjective(condition, objectiveField);
        });

        //Set to objective state
        NewConditionButton.GetComponent<ConditionButton>().OpenObjective();
    }

    public void SaveObjective(Condition condition, TMP_InputField ObjectiveField)
    {
        //Update objective of condition in current assets
        condition.objective = ObjectiveField.text;
        currentAssets.LevelConditions[FindCondition(condition)] = condition;
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

    public void UpdateName()
    {
        string Name = NameField.text;

        if (Name.Length > 0)
            GameData.LevelName = Name;
    }

    /// <summary>
    /// Checks if level name exists
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool LevelNameExists(string name)
    {
        if (LevelLoader.UserLevelNames.Contains(name) && name != GameData.LevelFileName)
            return true;

        return false;
    }
}
