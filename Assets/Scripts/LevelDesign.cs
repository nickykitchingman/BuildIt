using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Dropdown;

public class LevelDesign : MonoBehaviour
{
    public SelectName selectName;
    public Notification notification;
    public float SpawnDistance = 5f;
    public Button SelectButton;
    [Header("Objects")]
    public Transform LevelObjects;
    public Transform Triggers;

    [Header("Dropdowns")]
    public Dropdown ObjectDropdown; 
    public Dropdown ElementDropdown;
    public Dropdown TagDropdown;

    private List<string> Options;
    private Dictionary<string, List<string>> OptionLists;

    private string CurrentOption;
    private string CurrentObjectType;
    private int CurrentTypeIndex = 0;
    private GameObject CurrentObject;
    private Trigger CurrentTrigger;
    private string ElementName, Tag;
    private string ObjectName;
    private string Name;
    private CurrentAssets currentAssets;
    
    [Header("Scripts")]
    public PreRenderObject preRenderObject;
    public PlayerControls playerControls;
    
    void Awake()
    {
        //Add lists of options into dictionary
        OptionLists = new Dictionary<string, List<string>>();
        OptionLists.Add("Objects", new List<string>());
        OptionLists.Add("Triggers", new List<string>() { "CollisionPlane" });
        OptionLists.Add("Elements", new List<string>());
        //Options list
        Options = new List<string>() { "Object", "Trigger" };        

        //Initialise variables
        CurrentOption = Options[CurrentTypeIndex];
        CurrentObjectType = Options[CurrentTypeIndex];
    }

    void Start()
    {
        //Add reference to the current assets
        currentAssets = SceneAssets.currentAssets;

        //Initially set selectname input field to inactive
        selectName.gameObject.SetActive(true);
        selectName.gameObject.SetActive(false);

        //Initiate objects
        foreach(KeyValuePair<string, GameObject> LevelObject in LoadBuiltInObjects.CustomObjectsDict)
        {
            //Add each name of custom objects to list
            OptionLists["Objects"].Add(LevelObject.Key);
        }

        //Initialise the dropdown for built-in objects
        SelectButton.GetComponentInChildren<TextMeshProUGUI>().text = CurrentObjectType;
        ObjectDropdown.AddOptions(OptionLists["Objects"]);
        ObjectDropdown.AddOptions(new List<string>() { "none" });


        //Add all elements to element dictionary
        FieldInfo[] ListOfElements = typeof(Elements).GetFields();
        foreach (FieldInfo property in ListOfElements)
        {
            OptionLists["Elements"].Add(property.Name);
        }
        ChangeDropdown(ElementDropdown, OptionLists["Elements"]);

        //Set mode
        GameData.Mode = "leveldesign";

        ResetDropDownValue(TagDropdown);
        ResetDropDownValue(ObjectDropdown);

        PhysicsControl.Play();
    }

    void Update()
    {
        if (GameData.IsRenderingNew && GameData.CanConfirm)
        {
            //Confirm object load
            if (Input.GetKeyDown(KeyCode.Return) && !selectName.gameObject.activeSelf)
            {
                if (!preRenderObject.IsValid)
                {
                    //Error if invalid placement
                    notification.Notify("Invalid placement", Color.red);
                }
                else
                {

                    //Set to interact mode to enter trigger name etc
                    playerControls.InteractMode();

                    if (CurrentObjectType == "Trigger")
                    {
                        //Choose name if trigger
                        selectName.gameObject.SetActive(true);
                        GameData.ControlsEnabled = false;
                        preRenderObject.SetControls(false);
                    }
                    else
                        ConfirmObject();
                }
            }

            //Cancel object load
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(CurrentObject);
                preRenderObject.Activate(false);
                GameData.IsRenderingNew = false;
            }
        }

        //Set can select to opposite of mouse over ui
        GameData.CanSelect = !EventSystem.current.IsPointerOverGameObject();
    }

    public void Undo()
    {
        GameData.DesignActions.Undo();
    }

    public void Redo()
    {
        GameData.DesignActions.Redo();
    }

    public void CycleDropdown()
    {
        //Switch between types of objects
        SelectNextOption();
        SelectButton.GetComponentInChildren<TextMeshProUGUI>().text = CurrentOption;
        ChangeDropdown(ObjectDropdown, OptionLists[CurrentObjectType + "s"]);
    }

    public void ChangeDropdown(Dropdown dropdown, List<string> Options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(Options);
        dropdown.AddOptions(new List<string>() { "none" });

        ResetDropDownValue(dropdown);
    }

    public void SelectTriggerName(string name)
    {
        //Confirm
        if (CheckTriggerNameExists(name))
        {
            //Error if trigger name already exists
            notification.Notify("Trigger already exists", Color.red);
            return;
        }
        if (name == "")
        {
            //Error if nothing entered
            notification.Notify("Name must contain at least one character", Color.red);
            return;
        }
        //Enable player controls, update name and confirm object load
        Name = name;
        selectName.gameObject.SetActive(false);
        GameData.ControlsEnabled = true;
        preRenderObject.SetControls(true);
        ConfirmObject();
    }

    private void LoadTagsIntoDropdown(Dropdown dropdown)
    {
        List<string> CurrentTags = new List<string>();
        CurrentTags.AddRange(currentAssets.Tags);
        CurrentTags.Add("No Tag");
        ChangeDropdown(dropdown, CurrentTags);
    }

    private void ConfirmObject()
    {
        int index = 0;
        string type = "";
        //Update position, rotation, scale, isStatic of actual object
        CurrentObject.SetActive(true);
        CurrentObject.transform.position = preRenderObject.GetPosition();
        CurrentObject.transform.rotation = preRenderObject.GetRotation();
        //Update scale
        Vector3 scale = preRenderObject.GetScale();
        CurrentObject.transform.localScale = scale;
        CurrentObject.GetComponent<Properties>().Scale = scale;

        CurrentObject.GetComponent<Properties>().IsStatic = preRenderObject.RenderOptions.IsStatic;

        //Stop pre-rendering
        preRenderObject.Activate(false);
        GameData.IsRenderingNew = false;

        //Update name and add to current assets if object is a trigger
        if (CurrentObjectType == "Trigger")
        {
            CurrentTrigger.Name = Name;
            CurrentTrigger.ThisPlane.name = Name;
            index = currentAssets.TriggersInScene.Count;
            type = "Trigger";
            currentAssets.AddTrigger(CurrentTrigger);   
        }
        //Add object to current assets if not trigger
        else if (CurrentObjectType == "Object")
        {
            index = currentAssets.LevelObjectsInScene.Count;
            type = "LevelObject";
            currentAssets.AddLevelObject(CurrentObject);
        }

        //Add to editor actions
        ObjectReference reference = new ObjectReference(type, index);
        GameData.DesignActions.NewAction(new Create(reference));
    }

    private void SelectNextOption()
    {
        //Update current option
        if (++CurrentTypeIndex >= Options.Count)
        {
            CurrentOption = Options[0];
            CurrentObjectType = Options[0];
            CurrentTypeIndex = 0;
        }
        else
        {
            CurrentObjectType = Options[CurrentTypeIndex];
            CurrentOption = Options[CurrentTypeIndex];
        }
    }

    public void ChooseOption(int value)
    {
        //Change dropdown values and update dropdown variables accordingly
        switch (CurrentOption)
        {
            case "Object":
                //Select element of object
                ElementDropdown.Show();
                ObjectName = OptionLists["Objects"][value];
                CurrentOption = "Element";
                break;
            case "Trigger":
                //Load trigger
                LoadObject(value);
                break;
            case "Element":
                //Load object and return dropdown options to objects
                ElementName = ElementDropdown.options[value].text;
                TagDropdown.Show();
                CurrentOption = "Tag";
                break;
            case "Tag":
                //Add tag
                Tag = TagDropdown.options[value].text;
                LoadObject(value);
                CurrentOption = "Object";                
                break;
        }
        ResetDropDownValue(ObjectDropdown);
        ResetDropDownValue(ElementDropdown);
        ResetDropDownValue(TagDropdown);
    }

    public void LoadObject(int value = 0)
    {
        Vector2 ScreenCentre;

        //Destroy non-confirmed object
        if (GameData.IsRenderingNew)
            Destroy(CurrentObject);

        switch (CurrentObjectType)
        {
            case "Object":
                //Load object with element
                Element element = (Element)typeof(Elements).GetField(ElementName).GetValue(null);
                CurrentObject = LoadBuiltInObjects.LoadCustom(ObjectName, element);
                //Set tag in object's properties
                CurrentObject.GetComponent<Properties>().Tag = Tag;
                //Set object position
                ScreenCentre = new Vector2(Screen.width / 2, Screen.height / 2 - 1);
                CurrentObject.transform.position = Camera.main.ScreenPointToRay(ScreenCentre).GetPoint(SpawnDistance);
                //Add to level objects in heirachy
                CurrentObject.transform.parent = LevelObjects;
                //Update render script
                preRenderObject.UpdateObject(CurrentObject, true);
                break;
            case "Trigger":
                //Load trigger then pre-render before setting position
                CurrentTrigger = LoadBuiltInObjects.LoadTrigger(OptionLists["Triggers"][value]);
                CurrentObject = CurrentTrigger.ThisPlane;
                //Set trigger position
                ScreenCentre = new Vector2(Screen.width / 2, Screen.height / 2 - 1);
                CurrentObject.transform.position = Camera.main.ScreenPointToRay(ScreenCentre).GetPoint(SpawnDistance);
                //Add to triggers in heirachy
                CurrentObject.transform.parent = Triggers;
                //Update render script
                preRenderObject.UpdateObject(CurrentObject, true);
                break;
        }
        //Set new object to inactive, then render it
        CurrentObject.SetActive(false);
        preRenderObject.Activate(true);
        GameData.IsRenderingNew = true;
    }

    public void OpenObjectDropdown()
    {
        CurrentOption = "Object";
        ObjectDropdown.Show();
    }

    public void ChooseTriggerOrObject(int value)
    {
        //Either load trigger or select element of object
        switch (CurrentObjectType)
        {
            case "Object":
                //Record object name and show elements
                ObjectName = OptionLists["Objects"][value];
                ElementDropdown.Show();
                break;
            case "Trigger":
                //Load trigger
                LoadObject(value);
                break;
        }
        ResetDropDownValue(ObjectDropdown);
    }

    public void SelectElement(int value)
    {
        //Record element and show tags
        CurrentOption = "Element";
        ElementName = OptionLists["Elements"][value];
        LoadTagsIntoDropdown(TagDropdown);
        TagDropdown.Show();
        ResetDropDownValue(ElementDropdown);
    }

    public void SelectTag(int value)
    {
        //Record tag and load object
        CurrentOption = "Tag";
        if (value == TagDropdown.options.Count - 2)
            Tag = "";
        else
            Tag = currentAssets.Tags[value];

        LoadObject();
        ResetDropDownValue(TagDropdown);
    }

    private void ResetDropDownValue(Dropdown dropdown)
    {
        //Reset value to none without interfering with listeners
        DropdownEvent events = dropdown.onValueChanged;
        dropdown.onValueChanged.RemoveListener(ChooseTriggerOrObject);
        dropdown.onValueChanged.RemoveListener(SelectElement);
        dropdown.onValueChanged.RemoveListener(SelectTag);

        dropdown.value = dropdown.options.Count - 1;
        //Add relevant listener to deal with option select
        switch (dropdown.name)
        {
            case "ObjectSelect":                
                dropdown.onValueChanged.AddListener(ChooseTriggerOrObject);
                break;
            case "ElementSelect":
                dropdown.onValueChanged.AddListener(SelectElement);
                break;
            case "TagSelect":
                dropdown.onValueChanged.AddListener(SelectTag);
                break;
        }
    }

    private bool CheckTriggerNameExists(string name)
    {
        foreach (Trigger trigger in currentAssets.TriggersInScene)
            if (trigger.Name == name)
                return true;
        return false;
    }

    private void SetCanSelect(bool value)
    {
        GameData.CanSelect = value;
    }
}