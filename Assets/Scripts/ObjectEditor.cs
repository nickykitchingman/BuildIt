using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectEditor : MonoBehaviour
{
    [Header("Group Object")]
    public GameObject GroupObject;
    public TMP_InputField NameField;
    [Header("Limits")]
    public float MaxZoom;
    public float MinZoom;
    [Header("Shapes")]
    public Transform ShapeContent;
    public GameObject ShapePrefab;
    [Header("Inspector")]
    public Inspector inspector;
    public TMP_Dropdown ElementDropdown;
    [Header("Notification")]
    public Notification notification;
    [Header("Scripts")]
    public Shapes ShapesScript;
    public PreRenderObject preRenderObject;
    [Header("Tutorial")]
    public bool IsTutorial;

    private string Shape;
    private string ShapeElement;
    private List<string> ElementList;
    private GameObject CurrentObject;


    void Start()
    {
        //Set name
        if (NameField)
            NameField.text = "Object";

        //Reset object in GameData and undo stack
        GameData.CurrentChildObjects = new List<GameObject>();
        GameData.EditorActions = new ActionStack();

        //Add elements into list
        ElementList = new List<string>();
        FieldInfo[] ElementInfos = typeof(Elements).GetFields();
        foreach (FieldInfo property in ElementInfos)
        {
            ElementList.Add(property.Name);
        }

        //Add elements into dropdown
        if (!IsTutorial)
            LoadElementsIntoDropdown();

        //Load shapes into inspector
        if (!IsTutorial)
            LoadShapesIntoInspector();

        //Reset gamedata variables
        GameData.IsMoving = false;
        GameData.IsRenderingNew = false;
        GameData.IsRenderingOld = false;

        //Set mode
        GameData.Mode = "editor";

        //Stop physics engine
        PhysicsControl.Stop(true);
    }

    void Update()
    {
        if (GameData.IsRenderingNew)
        {
            //Confirm object load
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmObject();
            }

            //Cancel object load
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(CurrentObject);
                preRenderObject.Activate(false);
                GameData.IsRenderingNew = false;
            }
        }

        //Zoom
        else if (Input.GetAxis("Mouse ScrollWheel") != 0f && !GameData.IsRenderingOld)
        {
            //Scale up if positive, down if negative
            float scale = Input.GetAxis("Mouse ScrollWheel") > 0f ? -1.1f : 1.1f;
            Vector3 position = Camera.main.transform.position;
            if (GameData.CanSelect)
                if (Mathf.Abs(position.magnitude) < MaxZoom || scale < 1f)
                    if (Mathf.Abs(position.magnitude) > MinZoom || scale > 1f)
                    {
                        Vector3 direction = position.normalized;
                        Camera.main.transform.Translate(direction * scale, Space.World);
                    }
        }

        //Set can select to opposite of mouse over ui
        GameData.CanSelect = !EventSystem.current.IsPointerOverGameObject();
    }

    public void Undo()
    {
        GameData.EditorActions.Undo();
    }

    public void Redo()
    {
        GameData.EditorActions.Redo();
    }

    public void LoadElementsIntoDropdown()
    {
        ElementDropdown.ClearOptions();
        ElementDropdown.AddOptions(ElementList);
    }

    public void SelectShape(string Name)
    {
        Shape = Name;
        inspector.AddLowerMenu("Materials");
    }

    public void SaveMaterial()
    {
        ShapeElement = ElementList[ElementDropdown.value];
    }

    public void LoadShape()
    {
        //Destroy non-confirmed object
        if (GameData.IsRenderingNew)
            Destroy(CurrentObject);


        //Load in object
        Element element = (Element)typeof(Elements).GetField(ShapeElement).GetValue(null);
        CurrentObject = ShapesScript.LoadShape(Shape, element);

        preRenderObject.UpdateObject(CurrentObject, true);

        //Set new object to inactive, then render it
        CurrentObject.SetActive(false);
        preRenderObject.Activate(true);
        GameData.IsRenderingNew = true;
    }

    private void LoadShapesIntoInspector()
    {
        //Load shapes into inpsector from my assets
        foreach (string shape in MyAssets.CustomObjectsDict.Keys)
        {
            //Create new button
            GameObject newShape = Instantiate(ShapePrefab);
            //Set text and listener
            newShape.GetComponentInChildren<TextMeshProUGUI>().text = shape;
            newShape.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                SelectShape(shape);
            });
            //Set name and parent
            newShape.transform.SetParent(ShapeContent);
            newShape.name = shape;
            //Scale proportionally
            newShape.transform.localScale = ShapeContent.parent.localScale;
        }
    }

    private void ConfirmObject()
    {
        //Update position of actual object
        CurrentObject.SetActive(true);
        CurrentObject.transform.position = preRenderObject.GetPosition();
        CurrentObject.transform.rotation = preRenderObject.GetRotation();
        //Update scale
        Vector3 scale = preRenderObject.GetScale();
        CurrentObject.transform.localScale = scale;
        CurrentObject.GetComponent<Properties>().Scale = scale; 

        //Stop pre-rendering
        preRenderObject.Activate(false);
        GameData.IsRenderingNew = false;

        //Add to editor actions
        ObjectReference reference = new ObjectReference("Shape", GameData.CurrentChildObjects.Count);
        GameData.EditorActions.NewAction(new Create(reference));

        //Add to GameData
        GameData.CurrentChildObjects.Add(CurrentObject);
    }

    public void LoadWorld()
    {
        //Check not rendering
        if (GameData.IsRenderingNew || GameData.IsRenderingOld)
        {
            notification.Notify("Cannot exit editor whilst object is selected!", Color.red);
            return;
        }
        //Save object
        if (GroupObject.transform.childCount > 0)
            GameData.NewObject = new ParentData(GroupObject);
        //Switch scene
        SceneSwitch.LoadWorld();
    }

    public void LoadTutorial()
    {
        //Save object
        if (GroupObject.transform.childCount > 0)
            GameData.NewObject = new ParentData(GroupObject);

        //Update to nice position
        GameData.CurrentScene.PlayerPosX = -112;
        GameData.CurrentScene.PlayerPosY = 3;
        GameData.CurrentScene.PlayerPosZ = 246;
        //Update to nice rotation
        Quaternion rotation = Quaternion.Euler(22, 300, 0);
        GameData.CurrentScene.PlayerRotX = rotation.x;
        GameData.CurrentScene.PlayerRotY = rotation.y;
        GameData.CurrentScene.PlayerRotZ = rotation.z;
        GameData.CurrentScene.PlayerRotW = rotation.w;

        //Switch scene
        SceneSwitch.LoadPlayTutorial();
    }
}
