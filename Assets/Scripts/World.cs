using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public GameObject Ball;
    public bool LevelPlay;
    public Dropdown ObjectDropDown;
    public Material DefaultMaterial;
    [Header("Objects")]
    public Transform SceneObjects;
    [Header("Conditions")]
    public GameObject ConditionsPanel;
    public Button ConditionButton;
    [Range(0, 30)]
    public int OpenTime = 10;
    [Header("Notification")]
    public Notification notification;
    [Header("Scripts")]
    public PreRenderObject preRenderObject;
    public SelectObjects selectObjects;

    private GameObject CurrentObject;
    private CurrentAssets currentAssets;

    private void Start()
    {
        //Add reference to the current assets
        currentAssets = SceneAssets.currentAssets;

        //Add starting ball to current assets
        if (!LevelPlay && GameData.FirstLoad)
            currentAssets.AddUserObject(Ball);

        ObjectDropDown.AddOptions((List<string>)HandleUserObjects.ObjectNames);
        ObjectDropDown.options.Add(new Dropdown.OptionData("none"));

        //Set mode
        GameData.Mode = "world";
        if (LevelPlay)
        {
            GameData.Mode = "levelplay";
            //Show conditions at start of level
            if (GameData.FirstLoad)
                ShowConditions();
        }

        ResetDropDownValue();

        //Play physics
        if (GameData.Mode == "levelplay")
            PhysicsControl.Stop(true);
        else
            PhysicsControl.Play();

        //Render new object
        if (GameData.NewObject != null)
            ChooseObject(HandleUserObjects.LoadObject(GameData.NewObject));
        GameData.NewObject = null;
    }

    void Update()
    {
        if (GameData.IsRenderingNew && GameData.CanConfirm)
        {
            //Confirm object load
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!preRenderObject.IsValid)
                {
                    //Error if invalid placement
                    notification.Notify("Invalid placement", Color.red);
                }
                else
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


        //Set can select to opposite of mouse over UI
        GameData.CanSelect = !EventSystem.current.IsPointerOverGameObject();
    }
    
    private void ChooseObject(int value)
    {
        ChooseObject(HandleUserObjects.LoadObject(HandleUserObjects.ObjectDatas[value]));
        ResetDropDownValue();
    }

    private void ChooseObject(GameObject newObject)
    {
        //Destroy old object choice
        if (GameData.IsRenderingNew)
            Destroy(CurrentObject);

        //Load new object
        CurrentObject = newObject;
        //Group in hierachy
        CurrentObject.transform.parent = SceneObjects;

        //Update position to in front of player
        Vector2 ScreenCentre = new Vector2(Screen.width / 2, Screen.height / 2 - 1);
        CurrentObject.transform.position = Camera.main.ScreenPointToRay(ScreenCentre).GetPoint(5f);
        //Render object
        preRenderObject.UpdateObject(CurrentObject);

        //Set new object to inactive, then render it
        CurrentObject.SetActive(false);
        preRenderObject.Activate(true);
        GameData.IsRenderingNew = true;
    }

    /// <summary>
    /// Resets value of dropdown to none
    /// </summary>
    void ResetDropDownValue()
    {
        //Remove listeners, reset value, add listener
        ObjectDropDown.onValueChanged.RemoveAllListeners();
        ObjectDropDown.value = ObjectDropDown.options.Count - 1;
        ObjectDropDown.onValueChanged.AddListener(ChooseObject);
    }

    void ConfirmObject()
    {
        //Update position and rotation
        CurrentObject.SetActive(true);
        CurrentObject.transform.position = preRenderObject.GetPosition();
        CurrentObject.transform.rotation = preRenderObject.GetRotation();
        //Update scale
        Vector3 scale = preRenderObject.GetScale();
        CurrentObject.GetComponent<Properties>().Scale = scale;
        CurrentObject.transform.localScale = scale;
        //Update mass
        //Utilities.UpdateMasses(CurrentObject.transform);

        //Add to currentassets
        currentAssets.AddUserObject(CurrentObject);
        
        //Deactivate rendering
        preRenderObject.Activate(false);
        GameData.IsRenderingNew = false;

        //Add to level actions
        string type = "UserObject";
        int index = currentAssets.UserObjectsInScene.Count - 1;
        ObjectReference reference = new ObjectReference(type, index);
        GameData.LevelActions.NewAction(new Create(reference));
    }

    public void ShowConditions()
    {
        //Show condition panel then close after 10 seconds
        ConditionsPanel.SetActive(true);
        ConditionButton.gameObject.SetActive(false);
        StartCoroutine(CloseConditions(OpenTime));
    }

    public void CloseConditions()
    {
        //Close conditions panel immediately
        ConditionsPanel.SetActive(false);
        ConditionButton.gameObject.SetActive(true);
    }

    public IEnumerator CloseConditions(float time)
    {
        //Close conditions panel after time seconds
        yield return new WaitForSecondsRealtime(time);
        ConditionsPanel.SetActive(false);
        ConditionButton.gameObject.SetActive(true);
    }

    public void Undo()
    {
        GameData.LevelActions.Undo();
    }

    public void Redo()
    {
        GameData.LevelActions.Redo();
    }

    float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        { 
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }

        return Mathf.Abs(volume);
    }

    float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }
}